using System.Collections.Generic;
using System.Threading.Tasks;
using Apollo.Domain.Entity;

namespace Apollo.Persistence.Dao.Interfaces
{
    public interface IRowDao : IBaseDao<Row>
    {
        Task<IEnumerable<Row>> SelectRowsFromCinemaHallAsync(long cinemaHallId);
        Task<IEnumerable<Row>> SelectActiveRowsFromCinemaHallAsync(long cinemaHallId);

        Task<IEnumerable<RowCategory>> SelectActiveRowCategoriesFromCinemaHallAsync(long cinemaHallId);

        Task<Row> SelectSingleRowWithCategoryAsync(long rowId);

        Task<CinemaHall> GetCinemaHallByRowIdAsync(long rowId);
    }
}
