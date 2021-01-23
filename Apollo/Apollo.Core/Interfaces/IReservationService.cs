using System.Collections.Generic;
using System.Threading.Tasks;
using Apollo.Core.Dto;
using Apollo.Core.Validation;

namespace Apollo.Core.Interfaces
{
    public interface IReservationService
    {
        void AddRule(ISeatValidation validationRule);
        void UseDefaultRules();
        void ClearRules();

        Task<ReservationDto> AddReservationAsync(IEnumerable<SeatDto> seatDtos, long scheduleId, long userId);

        Task<IEnumerable<ReservationDto>> GetReservationsAsync();
        Task<IEnumerable<ReservationDto>> GetReservationsByUserIdAsync(long userId);
        Task<IEnumerable<ReservationDto>> GetReservationsByScheduleIdAsync(long scheduleId);
        Task<int> GetCountReservationsByCinemaHallAsync(long cinemaHallId);

        Task<bool> DeleteReservationsAsync(IEnumerable<ReservationDto> reservations);
        Task<bool> DeleteReservationsAsync(IEnumerable<long> reservationIds);
        Task<bool> DeleteReservationAsync(ReservationDto reservation);
        Task<bool> DeleteReservationAsync(long reservationId);
        void ApplyValidation(IEnumerable<SeatDto> seatDtos, IEnumerable<SeatDto> seatLayout);

        Task<IEnumerable<TicketDto>> GetTicketsAsync();
        Task<IEnumerable<TicketDto>> GetTicketsByUserIdAsync(long userId);

        Task<IEnumerable<SeatReservationDto>> GetSeatReservationsAsync();
        Task<ReservationDto> GetReservationByIdAsync(long reservationId);

        Task<IEnumerable<SeatReservationDto>> GetSeatReservationsByUserIdAsync(long userId);

        Task UpdateReservation(ReservationDto reservationDto, IList<SeatDto> selectedSeats);
    }
}
