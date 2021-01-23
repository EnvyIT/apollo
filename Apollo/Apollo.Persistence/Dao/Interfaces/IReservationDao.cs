using System.Collections.Generic;
using System.Threading.Tasks;
using Apollo.Domain.Entity;

namespace Apollo.Persistence.Dao.Interfaces
{
    public interface IReservationDao : IBaseDao<Reservation>
    {
        Task<IEnumerable<Reservation>> SelectReservationsByScheduleId(long scheduleId);

        Task<IEnumerable<Reservation>> SelectReservationsByUserId(long userId);

        Task<IEnumerable<Reservation>> SelectReservationsWithTicketsByUserId(long userId);

        Task<int> CountReservationsByCinemaHallIdAsync(long cinemaHallId);
    }
}
