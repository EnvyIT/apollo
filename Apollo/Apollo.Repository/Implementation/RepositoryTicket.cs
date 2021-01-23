using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Apollo.Domain.Entity;
using Apollo.Persistence.Dao.Interfaces;
using Apollo.Persistence.Util;
using Apollo.Repository.Interfaces;

namespace Apollo.Repository.Implementation
{
    public class RepositoryTicket : Repository, IRepositoryTicket
    {
        private readonly IReservationDao _reservationDao;
        private readonly ISeatReservationDao _seatReservationDao;
        private readonly ISeatDao _seatDao;
        private readonly ITicketDao _ticketDao;
        private readonly IScheduleDao _scheduleDao;
        private readonly IUserDao _userDao;
        private readonly ICinemaHallDao _cinemaHallDao;

        public RepositoryTicket(IConnectionFactory connectionFactory) : base(connectionFactory)
        {
            _reservationDao = DaoFactory.CreateReservationDao();
            _seatReservationDao = DaoFactory.CreateSeatReservationDao();
            _seatDao = DaoFactory.CreateSeatDao();
            _ticketDao = DaoFactory.CreateTicketDao();
            _scheduleDao = DaoFactory.CreateScheduleDao();
            _userDao = DaoFactory.CreateUserDao();
            _cinemaHallDao = DaoFactory.CreateCinemaHallDao();
        }


        public async Task<long> AddReservationAsync(IEnumerable<Seat> seats, long scheduleId, long userId)
        {
            ValidateCollection(seats);
            await ValidateIds(_seatDao, seats.Select(s => s.Id));
            await ValidateId(_scheduleDao, scheduleId);
            await ValidateId(_userDao, userId);
            var reservationId = 0L;
            await _reservationDao.FluentTransaction().PerformBlock(async () =>
            {
                reservationId = (await _reservationDao
                        .FluentInsert(new Reservation { ScheduleId = scheduleId, UserId = userId }).ExecuteAsync())
                        .First();
                var seatReservations = seats.Select(seat => new SeatReservation
                {
                    ReservationId = reservationId,
                    SeatId = seat.Id
                }).ToList();
                await _seatReservationDao.FluentInsert(seatReservations).ExecuteAsync();
            }).Commit();
            return reservationId;
        }

        public async Task<IEnumerable<Reservation>> GetReservationsAsync()
        {
            return await _reservationDao.SelectActiveAsync();
        }

        public async Task<int> DeleteReservationsAsync(IEnumerable<Reservation> reservations)
        {
            ValidateCollection(reservations);
            return await DeleteReservationsAsync(reservations.Select(r => r.Id));
        }

        public async Task<int> DeleteReservationsAsync(IEnumerable<long> reservationIds)
        {
            await ValidateIds(_reservationDao, reservationIds);
            var deleted = 0;
            foreach (var reservationId in reservationIds)
            {
                deleted += await DeleteReservationAsync(reservationId);
            }
            return deleted;
        }

        public async Task<int> DeleteReservationAsync(Reservation reservation)
        {
            ValidateNotNull(reservation);
            return await DeleteReservationAsync(reservation.Id);
        }

        public async Task<int> DeleteReservationAsync(long reservationId)
        {
            await ValidateId(_reservationDao, reservationId);
            var deleted = 0;
            await _reservationDao.FluentTransaction().PerformBlock(async () =>
            {
                await _seatReservationDao.SoftDeleteByReservationIdAsync(reservationId);
                deleted = await _reservationDao.FluentSoftDeleteById(reservationId);

            }).Commit();
            return deleted;
        }

        public async Task<long> AddTicketAsync(long reservationId)
        {
            await ValidateId(_reservationDao, reservationId);
            var reservation = await _reservationDao.SelectSingleByIdAsync(reservationId);
            return await AddTicketAsync(reservation);
        }

        public async Task<long> AddTicketAsync(Reservation reservation)
        {
            ValidateNotNull(reservation);
            await ValidateId(_reservationDao, reservation.Id);
            var ticketId = await _ticketDao.FluentInsert(new Ticket { Printed = DateTime.UtcNow }).ExecuteAsync();
            reservation.TicketId = ticketId.First();
            await _reservationDao.FluentUpdate(reservation).ExecuteAsync();
            return reservation.TicketId;
        }

        public async Task<IEnumerable<Ticket>> GetTicketsAsync()
        {
            return await _ticketDao.SelectActiveAsync();
        }

        public async Task<Ticket> GetTicketByIdAsync(long id)
        {
            return await _ticketDao.SelectSingleByIdAsync(id);
        }

        public async Task<int> DeleteTicketAsync(Ticket ticket)
        {
            ValidateNotNull(ticket);
            await ValidateId(_ticketDao, ticket.Id);
            return await DeleteTicketAsync(ticket.Id);
        }

        public async Task<int> DeleteTicketAsync(long ticketId)
        {
            await ValidateId(_ticketDao, ticketId);
            return await _ticketDao.FluentSoftDeleteById(ticketId);
        }

        public async Task<IEnumerable<Seat>> GetFreeSeatsAsync(Schedule schedule)
        {
            ValidateNotNull(schedule);
            await ValidateId(_scheduleDao, schedule.Id);
            var reservations = await _reservationDao.SelectReservationsByScheduleId(schedule.Id);
            if (!reservations.Any())
            {
                return await _seatDao.SelectActiveWithRowAndCategoryByCinemaHallAsync(schedule.CinemaHallId);
            }
            var occupiedSeats = await _seatReservationDao.SelectAllSeatIdsByReservationIdsAsync(reservations.Select(reservation => reservation.Id));
            return await _seatDao.SelectFreeSeatsByIdAsync(occupiedSeats);
        }

        public async Task<int> GetCountReservationsByCinemaHallAsync(long cinemaHallId)
        {
            return await _reservationDao.CountReservationsByCinemaHallIdAsync(cinemaHallId);
        }

        public async Task<IEnumerable<SeatReservation>> GetSeatReservationsAsync()
        {
            return await _seatReservationDao.SelectActiveAsync();
        }

        public async Task<Reservation> GetReservationByIdAsync(long reservationId)
        {
            await ValidateId(_reservationDao, reservationId);
            return await _reservationDao.SelectSingleByIdAsync(reservationId);
        }

        public async Task<IEnumerable<Reservation>> GetReservationsByScheduleIdAsync(long scheduleId)
        {
            await ValidateId(_scheduleDao, scheduleId);
            return await _reservationDao.SelectReservationsByScheduleId(scheduleId);
        }

        public async Task<IEnumerable<Seat>> GetSeatsWithRowAndCategoryByIdAsync(long reservationId)
        {
            await ValidateId(_reservationDao, reservationId);
            return await _seatReservationDao.SelectAllSeatsWithRowAndCategoryByIdAsync(reservationId);
        }

        public async Task<IEnumerable<SeatReservation>> GetSeatReservationsByIds(IEnumerable<long> reservationIds)
        {
            await ValidateIds(_reservationDao, reservationIds);
            return await _seatReservationDao.SelectSeatReservationsByReservationIds(reservationIds);

        }

        public async Task<IEnumerable<SeatReservation>> GetSeatReservationsByReservationIdAsync(long reservationId)
        {
            await ValidateId(_reservationDao, reservationId);
            return await _seatReservationDao.SelectSeatReservationsByReservationId(reservationId);
        }

        public async Task<IEnumerable<Ticket>> GetTicketsByUserIdAsync(long userId)
        {
            await ValidateId(_userDao, userId);
            var reservations = await _reservationDao.SelectReservationsWithTicketsByUserId(userId);
            return reservations.Select(r => r.Ticket).Distinct();
        }

        public async Task<IEnumerable<Reservation>> GetReservationsByUserIdAsync(long userId)
        {
            await ValidateId(_userDao, userId);
            return await _reservationDao.SelectReservationsByUserId(userId);
        }

        public async Task<bool> DeleteSeatReservationAsync(long seatReservationId)
        {
            await ValidateId(_seatReservationDao, seatReservationId);
            return await _seatReservationDao.SoftDeleteBySeatReservationIdAsync(seatReservationId) > 0;
        }

        public async Task<bool> AddSeatsToReservationAsync(long reservationDtoId, List<Seat> addedSeatReservations)
        {
            await ValidateId(_reservationDao, reservationDtoId);
            return await _seatReservationDao.AddSeatsToReservationAsync(reservationDtoId, addedSeatReservations);
        }

    }
}
