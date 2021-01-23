using System.Collections.Generic;
using System.Threading.Tasks;
using Apollo.Domain.Entity;

namespace Apollo.Repository.Interfaces
{
    public interface IRepositoryTicket : IRepository
    {

        Task<long> AddReservationAsync(IEnumerable<Seat> seats, long scheduleId, long userId);

        Task<IEnumerable<Reservation>> GetReservationsAsync();
 
        Task<int> DeleteReservationsAsync (IEnumerable<Reservation> reservations);
        Task<int> DeleteReservationsAsync (IEnumerable<long> reservationIds);
        Task<int> DeleteReservationAsync (Reservation reservation);
        Task<int> DeleteReservationAsync (long reservationId);

        Task<long> AddTicketAsync(long reservationId);
        Task<long> AddTicketAsync(Reservation reservation);
 
        Task<IEnumerable<Ticket>> GetTicketsAsync();
        Task<Ticket> GetTicketByIdAsync(long id);

        Task<int> DeleteTicketAsync(Ticket ticket);
        Task<int> DeleteTicketAsync(long ticketId);

        Task<IEnumerable<Seat>> GetFreeSeatsAsync(Schedule schedule);
        Task<int> GetCountReservationsByCinemaHallAsync(long cinemaHallId);

        Task<IEnumerable<SeatReservation>> GetSeatReservationsAsync();
        Task<Reservation> GetReservationByIdAsync(long reservationId);
        Task<IEnumerable<Reservation>> GetReservationsByScheduleIdAsync(long schedule);
        Task<IEnumerable<Seat>> GetSeatsWithRowAndCategoryByIdAsync(long reservationId);
        Task<IEnumerable<SeatReservation>> GetSeatReservationsByIds(IEnumerable<long> reservationIds);
        Task<IEnumerable<SeatReservation>> GetSeatReservationsByReservationIdAsync(long reservationId);

        Task<IEnumerable<Ticket>> GetTicketsByUserIdAsync(long userId);
        Task<IEnumerable<Reservation>> GetReservationsByUserIdAsync(long userId);

        Task<bool> DeleteSeatReservationAsync(long seatReservationId);
        Task<bool> AddSeatsToReservationAsync(long reservationDtoId, List<Seat> addedSeatReservations);
    }
}
