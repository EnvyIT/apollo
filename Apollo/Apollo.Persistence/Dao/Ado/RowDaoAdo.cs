using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.ComTypes;
using System.Threading.Tasks;
using Apollo.Domain.Entity;
using Apollo.Persistence.Dao.Interfaces;
using Apollo.Persistence.FluentEntity.Ado;
using Apollo.Persistence.Util;

namespace Apollo.Persistence.Dao.Ado
{
    public class RowDaoAdo : BaseDao<Row>, IRowDao
    {
        public RowDaoAdo(IConnectionFactory connectionFactory) : base(connectionFactory)
        {
        }

        public Task<IEnumerable<Row>> SelectRowsFromCinemaHallAsync(long cinemaHallId)
        {
            return FluentSelectAll()
                .InnerJoin<Row, CinemaHall, long, long>(null, _ => _.CinemaHallId, _ => _.Id)
                .Where(_ => _.CinemaHallId)
                .Equal(cinemaHallId)
                .QueryAsync();
        }

        public Task<IEnumerable<Row>> SelectActiveRowsFromCinemaHallAsync(long cinemaHallId)
        {
            return FluentSelectAll()
                .InnerJoin<Row, CinemaHall, long, long>(null, _ => _.CinemaHallId, _ => _.Id)
                .WhereActive()
                .And(_ => _.CinemaHallId)
                .Equal(cinemaHallId)
                .QueryAsync();
        }

        public async Task<IEnumerable<RowCategory>> SelectActiveRowCategoriesFromCinemaHallAsync(long cinemaHallId)
        {
            return (await FluentSelectAll()
                    .InnerJoin<Row, RowCategory, long, long>(_ => _.Category, _ => _.CategoryId, _ => _.Id)
                    .InnerJoin<Row, CinemaHall, long, long>(null, _ => _.CinemaHallId, _ => _.Id)
                    .WhereActive()
                    .And(_ => _.CinemaHall.Deleted)
                    .Equal(false)
                    .And(_ => _.Category.Deleted)
                    .Equal(false)
                    .And(_ => _.CinemaHallId)
                    .Equal(cinemaHallId)
                    .QueryAsync())
                ?.Select(_ => _.Category);
        }

        public async Task<Row> SelectSingleRowWithCategoryAsync(long rowId)
        {
            return await FluentSelectAll()
                .InnerJoin<Row, RowCategory, long, long>(_ => _.Category, _ => _.CategoryId, _ => _.Id)
                .WhereActive()
                .And(_ => _.Id)
                .Equal(rowId)
                .And(_ => _.Category.Deleted)
                .Equal(false)
                .QuerySingleAsync();
        }

        public async Task<CinemaHall> GetCinemaHallByRowIdAsync(long rowId)
        {
            return (await FluentSelectAll()
                .InnerJoin<Row, CinemaHall, long, long>(_ => _.CinemaHall, _ => _.CinemaHallId, _ => _.Id)
                .Where(_ => _.Id)
                .Equal(rowId)
                .QuerySingleAsync()).CinemaHall;
        }
    }
}