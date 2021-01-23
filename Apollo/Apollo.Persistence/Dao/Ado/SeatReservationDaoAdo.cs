using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Apollo.Domain.Entity;
using Apollo.Persistence.Dao.Interfaces;
using Apollo.Persistence.Util;

namespace Apollo.Persistence.Dao.Ado
{
    public class SeatReservationDaoAdo : BaseDao<SeatReservation>, ISeatReservationDao
    {
        public SeatReservationDaoAdo(IConnectionFactory connectionFactory) : base(connectionFactory)
        {
        }

        public async Task<IEnumerable<long>> SelectAllSeatIdsByReservationIdsAsync(IEnumerable<long> reservationIds)
        {
            var seatReservations = await FluentSelectAll()
                .WhereActive()
                .And(sr => sr.ReservationId)
                .In(reservationIds)
                .QueryAsync();

            return seatReservations.Select(sr => sr.SeatId).ToList();
        }

        public async Task<int> SoftDeleteBySeatReservationIdAsync(long seatReservationId)
        {
            return await FluentSoftDeleteById(seatReservationId);
        }

        public async Task<IEnumerable<long>> SelectAllSeatIdsByReservationIdAsync(long reservationId)
        {
            var seatReservations = await FluentSelectAll()
                .WhereActive()
                .And(sr => sr.ReservationId)
                .Equal(reservationId)
                .QueryAsync();

            return seatReservations.Select(sr => sr.SeatId).ToList();
        }

        public async Task<IEnumerable<Seat>> SelectAllSeatsWithRowAndCategoryByIdAsync(long reservationId)
        {
            return (await FluentSelectAll()
                .InnerJoin<SeatReservation, Seat, long, long>(sr => sr.Seat, sr => sr.SeatId, s => s.Id)
                .InnerJoin<Seat, Row, long, long>(sr => sr.Seat.Row, s => s.RowId, r => r.Id)
                .InnerJoin<Row, RowCategory, long, long>(sr => sr.Seat.Row.Category, r => r.CategoryId, rc => rc.Id)
                .WhereActive()
                .And(sr => sr.ReservationId)
                .Equal(reservationId)
                .QueryAsync()).Select(row => row.Seat);
        }

        public async Task<IEnumerable<SeatReservation>> SelectSeatReservationsByReservationIds(IEnumerable<long> reservationIds)
        {
             return await FluentSelectAll()
                 .InnerJoin<SeatReservation, Reservation, long, long>(sr => sr.Reservation, sr => sr.ReservationId, r => r.Id)
                 .InnerJoin<SeatReservation, Seat, long, long>(sr => sr.Seat, sr => sr.SeatId, s => s.Id)
                 .Where(sr => sr.ReservationId)
                 .In(reservationIds)
                 .QueryAsync();
        }

        public async Task<IEnumerable<SeatReservation>> SelectSeatReservationsByReservationId(long reservationId)
        {
            return await FluentSelectAll()
                .InnerJoin<SeatReservation, Reservation, long, long>(sr => sr.Reservation, sr => sr.ReservationId, r => r.Id)
                .InnerJoin<SeatReservation, Seat, long, long>(sr => sr.Seat, sr => sr.SeatId, s => s.Id)
                .WhereActive()
                .And(sr => sr.ReservationId)
                .Equal(reservationId)
                .QueryAsync();
        }

        public async Task<bool> AddSeatsToReservationAsync(long reservationDtoId, List<Seat> addedSeatReservations)
        {
            var seatReservations = addedSeatReservations.Select(sr => new SeatReservation
            {
                ReservationId = reservationDtoId,
                SeatId = sr.Id
            }).ToList();
            var addedRows = await FluentInsert(seatReservations).ExecuteAsync();
            return addedRows.Count() == seatReservations.Count();
        }

        public async Task<int> SoftDeleteByReservationIdAsync(long reservationId)
        {
            var seatReservations = (await FluentSelectAll()
                .Where(sr => sr.ReservationId)
                .Equal(reservationId)
                .QueryAsync()).ToList();
            var deleted = 0;
            await FluentTransaction().PerformBlock(async () =>
            {
                foreach (var seatReservation in seatReservations)
                {
                    deleted += await FluentSoftDeleteById(seatReservation.Id);
                }
            }).Commit();
            
            return deleted;
        }
    }
}