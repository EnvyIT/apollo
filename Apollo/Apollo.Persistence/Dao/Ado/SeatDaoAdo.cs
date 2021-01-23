using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Apollo.Domain.Entity;
using Apollo.Persistence.Dao.Interfaces;
using Apollo.Persistence.FluentEntity.Interfaces.Select;
using Apollo.Persistence.Util;

namespace Apollo.Persistence.Dao.Ado
{
    public class SeatDaoAdo : BaseDao<Seat>, ISeatDao
    {
        public SeatDaoAdo(IConnectionFactory connectionFactory) : base(connectionFactory)
        {
        }

        public Task<IEnumerable<Seat>> SelectWithRowAndCategoryByCinemaHallAsync(long cinemaHallId)
        {
            return SelectWithRowAndCategoryJoin(FluentSelectAll())
                .Where(_ => _.Row.CinemaHallId)
                .Equal(cinemaHallId)
                .QueryAsync();
        }

        public Task<IEnumerable<Seat>> SelectActiveWithRowAndCategoryByCinemaHallAsync(long cinemaHallId)
        {
            return SelectRowAndCategoryAndCinemaHallJoin(SelectWithRowAndCategoryJoin(FluentSelectAll()))
                .WhereActive()
                .And(_ => _.Row.Deleted)
                .Equal(false)
                .And(_ => _.Row.CinemaHall.Deleted)
                .Equal(false)
                .And(_ => _.Row.CinemaHallId)
                .Equal(cinemaHallId)
                .QueryAsync();
        }

        public async Task<IEnumerable<Seat>> SelectSeatsFromCinemaHallAsync(long cinemaHallId)
        {
            return await FluentSelectAll()
                .InnerJoin<Seat, Row, long, long>(s => s.Row, s => s.RowId, r => r.Id)
                .Where(s => s.Row.CinemaHallId)
                .Equal(cinemaHallId)
                .QueryAsync();
        }

        public async Task<IEnumerable<Seat>> SelectActiveSeatsWithRowByRowCategoryAsync(long rowCategoryId)
        {
            return await FluentSelectAll()
                .InnerJoin<Seat, Row, long, long>(s => s.Row, s => s.RowId, r => r.Id)
                .Where(s => s.Row.CategoryId)
                .Equal(rowCategoryId)
                .QueryAsync();
        }

        public async Task<IEnumerable<Seat>> SelectActiveSeatsByIdAsync(long rowId)
        {
            return await FluentSelectAll()
                .Where(s => s.RowId)
                .Equal(rowId)
                .QueryAsync();
        }

        public Task<IEnumerable<Seat>> SelectActiveSeatsFromCinemaHallByCategoryAsync(long cinemaHallId,
            long categoryId)
        {
            return SelectActiveSeatsFromCinemaHallByCategoryAsync(cinemaHallId, new[] {categoryId});
        }

        public Task<IEnumerable<Seat>> SelectActiveSeatsFromCinemaHallByCategoryAsync(long cinemaHallId,
            IEnumerable<long> categoryIds)
        {
            return SelectRowAndCategoryAndCinemaHallJoin(SelectWithRowAndCategoryJoin(FluentSelectAll()))
                .WhereActive()
                .And(s => s.Locked)
                .Equal(false)
                .And(_ => _.Row.CinemaHallId)
                .Equal(cinemaHallId)
                .And(_ => _.Row.Deleted)
                .Equal(false)
                .And(_ => _.Row.CategoryId)
                .In(categoryIds)
                .QueryAsync();
        }

        public async Task<int> UpdateSeatLockStateAsync(long seatId, bool locked)
        {
            var seat = await SelectSingleByIdAsync(seatId);
            seat.Locked = locked;
            return await FluentUpdate(seat).ExecuteAsync();
        }

        private static IFluentEntityJoin<Seat> SelectWithRowAndCategoryJoin(IFluentEntityJoin<Seat> statement)
        {
            return statement
                .InnerJoin<Seat, Row, long, long>(_ => _.Row, _ => _.RowId, _ => _.Id)
                .InnerJoin<Row, RowCategory, long, long>(_ => _.Row.Category, _ => _.CategoryId, _ => _.Id);
        }

        private static IFluentEntityWhereSelect<Seat> SelectRowAndCategoryAndCinemaHallJoin(
            IFluentEntityJoin<Seat> statement)
        {
            return statement
                .InnerJoin<Row, CinemaHall, long, long>(_ => _.Row.CinemaHall, _ => _.CinemaHallId, _ => _.Id);
        }

        public async Task<IEnumerable<Seat>> SelectFreeSeatsByIdAsync(IEnumerable<long> seatIds)
        {
            return await FluentSelectAll()
                .WhereActive()
                .And(s => s.Locked)
                .Equal(false)
                .And(s => s.Id)
                .NotIn(seatIds)
                .QueryAsync();
        }
    }
}