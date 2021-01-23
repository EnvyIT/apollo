using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Apollo.Core.Dto;
using Apollo.Core.Interfaces;
using Apollo.Core.Validation;
using Apollo.Domain.Entity;
using Apollo.UnitOfWork.Interfaces;
using Apollo.Util.Logger;
using static Apollo.Core.Dto.Mapper;
using static Apollo.Util.ValidationHelper;

namespace Apollo.Core.Implementation
{
    public class ReservationService : IReservationService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IInfrastructureService _infrastructureService;
        private readonly IList<ISeatValidation> _validationRules;
        private static readonly SemaphoreSlim SemaphoreSlim = new SemaphoreSlim(1);

        private static readonly IApolloLogger<ReservationService> Logger = LoggerFactory.CreateLogger<ReservationService>();

        public ReservationService(IUnitOfWork unitOfWork, IInfrastructureService infrastructureService)
        {
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
            _infrastructureService = infrastructureService ?? throw new ArgumentNullException(nameof(infrastructureService));
            _validationRules = new List<ISeatValidation>();
            UseDefaultRules();
        }

        public void AddRule(ISeatValidation validationRule)
        {
            _validationRules.Add(validationRule);
        }

        public void UseDefaultRules()
        {
            _validationRules.Add(new SeatAvailableValidation());
            _validationRules.Add(new SeatBlockValidation());
        }

        public void ClearRules()
        {
            _validationRules.Clear();
        }


        public async Task<ReservationDto> AddReservationAsync(IEnumerable<SeatDto> seatDtos, long scheduleId, long userId)
        {
            await SemaphoreSlim.WaitAsync();
            try
            {
                return await TryAddReservationAsync(seatDtos, scheduleId, userId);
            }
            finally
            {
                SemaphoreSlim.Release();
            }
        }

        private async Task<ReservationDto> TryAddReservationAsync(IEnumerable<SeatDto> seatDtos, long scheduleId, long userId)
        {
            ValidateEnumerableIsNotNullOrEmpty(Logger, seatDtos, nameof(seatDtos));
            var schedule = await _unitOfWork.RepositorySchedule.GetScheduleByIdAsync(scheduleId);

            ValidateNull(Logger.Here(), schedule);

            var dateTimeUtcNow = DateTime.UtcNow;
            ValidateLessOrEquals(Logger.Here(), dateTimeUtcNow, schedule.StartTime);

            var user = await _unitOfWork.RepositoryUser.GetUserWithAllReferencesByIdAsync(userId);
            ValidateLessOrEquals(Logger.Here(), seatDtos.Count(), user.Role.MaxReservations);

            var seatLayout = (await _infrastructureService.GetLayout(Map(schedule)));
            var flatLayout = seatLayout.Cast<SeatDto>()
                .Where(s => s != null)
                .ToList();


            ApplyValidation(seatDtos, flatLayout);

            var seats = seatDtos.Select(Map).ToList();
            var reservationId = await _unitOfWork.RepositoryTicket.AddReservationAsync(seats, scheduleId, userId);
            ValidateId<ReservationService, long>(Logger.Here(), reservationId);
            return Map(await _unitOfWork.RepositoryTicket.GetReservationByIdAsync(reservationId));
        }

        public void ApplyValidation(IEnumerable<SeatDto> seatDtos, IEnumerable<SeatDto> seatLayout)
        {
            _validationRules.ToList().ForEach(r =>
            {
                if (!r.IsValid(seatLayout, seatDtos))
                {
                    var exception = new SeatValidationException(nameof(AddReservationAsync));
                    Logger.Here().Error(exception, "{ValidationRule} failed", r.GetType());
                    throw exception;
                }
            });
        }

        public async Task<IEnumerable<ReservationDto>> GetReservationsAsync()
        {
            return (await _unitOfWork.RepositoryTicket.GetReservationsAsync()).Select(Map);
        }

        public async Task<bool> DeleteReservationsAsync(IEnumerable<ReservationDto> reservations)
        {
            ValidateEnumerableIsNotNullOrEmpty(Logger, reservations, nameof(reservations));
            var result = true;
            var reservationIds = reservations.Select(r => r.Id).ToList();
            var seatReservations = (await _unitOfWork.RepositoryTicket.GetSeatReservationsByIds(reservationIds)).ToList(); 
            if (!ValidateSeatReservations(seatReservations))
            {
                return false;
            }
            await _unitOfWork.Transaction().PerformBlock(async () =>
            {
                result = result && await DeleteAllReservationsAsync(seatReservations);
            }).Commit();

            return result;

        }

        private async Task<bool> DeleteAllReservationsAsync(IEnumerable<SeatReservation> seatReservations)
        {
            ValidateEnumerableIsNotNullOrEmpty(Logger, seatReservations, nameof(seatReservations));
            var deleted = true;
            foreach (var seatReservation in seatReservations)
            {
                deleted = deleted && await _unitOfWork.RepositoryTicket.DeleteSeatReservationAsync(seatReservation.Id);
            }
            var reservationIds = seatReservations.Select(sr => sr.ReservationId).Distinct().ToList();
            foreach (var reservationId in reservationIds)
            {
                deleted = deleted && await _unitOfWork.RepositoryTicket.DeleteReservationAsync(reservationId) > 0;
            }
            return deleted;
        }

        public async Task<bool> DeleteReservationsAsync(IEnumerable<long> reservationIds)
        {
            ValidateEnumerableIsNotNullOrEmpty(Logger, reservationIds, nameof(reservationIds));
            var result = true;
            var seatReservations = (await _unitOfWork.RepositoryTicket.GetSeatReservationsByIds(reservationIds)).ToList();
            if (!ValidateSeatReservations(seatReservations))
            {
                return false;
            }
            await _unitOfWork.Transaction().PerformBlock(async () =>
            {
                result = result && await DeleteAllReservationsAsync(seatReservations);
            }).Commit();

            return result;
        }

        public async Task<bool> DeleteReservationAsync(ReservationDto reservation)
        {
            ValidateNull(Logger, reservation);
            var result = true;
            var seatReservations = (await _unitOfWork.RepositoryTicket.GetSeatReservationsByReservationIdAsync(reservation.Id)).ToList();
            if (!ValidateSeatReservations(seatReservations))
            {
                return false;
            }
            await _unitOfWork.Transaction().PerformBlock(async () =>
            {
                result = result && await DeleteAllReservationsAsync(seatReservations);
            }).Commit();

            return result;
        }

        private static bool ValidateSeatReservations(List<SeatReservation> seatReservations)
        {
            if(!seatReservations.Any())
            {
                Logger.Here().Warning("{SeatReservations} are empty", nameof(seatReservations));
                return false;
            }
            return true;
        }

        public async Task<bool> DeleteReservationAsync(long reservationId)
        {
            var result = true;
            var seatReservations = (await _unitOfWork.RepositoryTicket.GetSeatReservationsByReservationIdAsync(reservationId)).ToList();
            if (!seatReservations.Any())
            {
                return false;
            }
            await _unitOfWork.Transaction().PerformBlock(async () =>
            {
                result = result && await DeleteAllReservationsAsync(seatReservations);
            }).Commit();

            return result;
        }

        public async Task<IEnumerable<TicketDto>> GetTicketsAsync()
        {
            return (await _unitOfWork.RepositoryTicket.GetTicketsAsync()).Select(Map);
        }

        public async Task<IEnumerable<TicketDto>> GetTicketsByUserIdAsync(long userId)
        {
            return (await _unitOfWork.RepositoryTicket.GetTicketsByUserIdAsync(userId)).Select(Map);
        }

        public async Task<IEnumerable<SeatReservationDto>> GetSeatReservationsAsync()
        {
            return (await _unitOfWork.RepositoryTicket.GetSeatReservationsAsync()).Select(Map);
        }

        public async Task<IEnumerable<ReservationDto>> GetReservationsByUserIdAsync(long userId)
        {
            return (await _unitOfWork.RepositoryTicket.GetReservationsByUserIdAsync(userId)).Select(Map);
        }

        public async Task<IEnumerable<ReservationDto>> GetReservationsByScheduleIdAsync(long scheduleId)
        {
            return (await _unitOfWork.RepositoryTicket.GetReservationsByScheduleIdAsync(scheduleId)).Select(Map);
        }

        public async Task<int> GetCountReservationsByCinemaHallAsync(long cinemaHallId)
        {
            return await _unitOfWork.RepositoryTicket.GetCountReservationsByCinemaHallAsync(cinemaHallId);
        }

        public async Task<ReservationDto> GetReservationByIdAsync(long reservationId)
        {
            return Map(await _unitOfWork.RepositoryTicket.GetReservationByIdAsync(reservationId));
        }

        public async Task<IEnumerable<SeatReservationDto>> GetSeatReservationsByUserIdAsync(long userId)
        {
            var reservationIds = (await _unitOfWork.RepositoryTicket.GetReservationsByUserIdAsync(userId))
                .Select(r => r.Id).ToList();
            ValidateNull(Logger, reservationIds);

            return (await _unitOfWork.RepositoryTicket.GetSeatReservationsByIds(reservationIds))
                .OrderBy(r => r.Id)
                .Select(Map);
        }

        public async Task UpdateReservation(ReservationDto reservationDto, IList<SeatDto> selectedSeats)
        {
            var seatReservations = (await _unitOfWork.RepositoryTicket.GetSeatReservationsByReservationIdAsync(reservationDto.Id)).ToList();
            var removedSeatReservations = seatReservations
                .Where(sr => !selectedSeats.Select(s => s.Id).Contains(sr.SeatId))
                .ToList();
            var addedSeatReservations = selectedSeats.Where(s => !seatReservations.Select(sr => sr.SeatId).Contains(s.Id))
                .ToList();
            await _unitOfWork.Transaction()
                .PerformBlock(async () =>
                {
                    foreach (var seatReservation in removedSeatReservations)
                    {
                         await _unitOfWork.RepositoryTicket.DeleteSeatReservationAsync(seatReservation.Id);
                    }

                    if (addedSeatReservations.Any())
                    {
                        var seatLayout = await _infrastructureService.GetLayout(reservationDto.Schedule);
                        var flatLayout = seatLayout.Cast<SeatDto>()
                            .Where(s => s != null)
                            .ToList();
                        flatLayout.ForEach(s =>
                        {
                            if (seatReservations.Select(sr => sr.SeatId).Contains(s.Id))
                            {
                                s.State = SeatState.Free; //seats which are already reserved by the user should be marked as free for validation purposes
                            }
                        });
                        var desiredSeats = seatReservations.Select(sr => Map(sr.Seat))
                            .Union(addedSeatReservations)
                            .ToList();
                        ApplyValidation(desiredSeats, flatLayout);
                        await _unitOfWork.RepositoryTicket.AddSeatsToReservationAsync(reservationDto.Id, addedSeatReservations.Select(Map).ToList());
                    }
                }).Commit();
        }
    }
}
