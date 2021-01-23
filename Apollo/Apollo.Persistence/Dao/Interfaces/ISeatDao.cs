using System.Collections.Generic;
using System.Threading.Tasks;
using Apollo.Domain.Entity;

namespace Apollo.Persistence.Dao.Interfaces
{
    public interface ISeatDao : IBaseDao<Seat>
    {
        Task<IEnumerable<Seat>> SelectFreeSeatsByIdAsync(IEnumerable<long> seatIds);
        Task<IEnumerable<Seat>> SelectWithRowAndCategoryByCinemaHallAsync(long cinemaHall);

        Task<IEnumerable<Seat>> SelectActiveWithRowAndCategoryByCinemaHallAsync(long cinemaHall);

        Task<IEnumerable<Seat>> SelectActiveSeatsFromCinemaHallByCategoryAsync(long cinemaHallId, long categoryId);

        Task<IEnumerable<Seat>> SelectActiveSeatsFromCinemaHallByCategoryAsync(long cinemaHallId, IEnumerable<long> categoryIds);
        Task<int> UpdateSeatLockStateAsync(long seatId, bool locked);
        Task<IEnumerable<Seat>> SelectSeatsFromCinemaHallAsync(long cinemaHallId);
        Task<IEnumerable<Seat>> SelectActiveSeatsWithRowByRowCategoryAsync(long rowCategoryId);
        Task<IEnumerable<Seat>> SelectActiveSeatsByIdAsync(long rowId);
    }
}
