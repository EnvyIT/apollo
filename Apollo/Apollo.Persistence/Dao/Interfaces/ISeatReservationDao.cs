using System.Collections.Generic;
using System.Threading.Tasks;
using Apollo.Domain.Entity;

namespace Apollo.Persistence.Dao.Interfaces
{
    public interface ISeatReservationDao : IBaseDao<SeatReservation>
    {
        Task<IEnumerable<long>> SelectAllSeatIdsByReservationIdsAsync(IEnumerable<long> reservationIds);
        Task<int> SoftDeleteBySeatReservationIdAsync(long seatReservationId);
        Task<IEnumerable<long>> SelectAllSeatIdsByReservationIdAsync(long reservationId);
        Task<IEnumerable<Seat>> SelectAllSeatsWithRowAndCategoryByIdAsync(long reservationId);
        Task<IEnumerable<SeatReservation>> SelectSeatReservationsByReservationIds(IEnumerable<long> reservationIds);
        Task<IEnumerable<SeatReservation>> SelectSeatReservationsByReservationId(long reservationId);
        Task<bool> AddSeatsToReservationAsync(long reservationDtoId, List<Seat> addedSeatReservations);
        Task<int> SoftDeleteByReservationIdAsync(long reservationId);
    }
}
