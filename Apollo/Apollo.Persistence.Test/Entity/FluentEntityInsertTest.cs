using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Apollo.Persistence.Test.Entity.Mock;
using FluentAssertions;
using NUnit.Framework;

namespace Apollo.Persistence.Test.Entity
{
    public class FluentEntityInsertTest : FluentEntityBaseTest
    {
        [TearDown]
        public async Task TearDown()
        {
            await _insertHelper.TearDown();
        }

        [Test]
        public async Task InsertNewEntities_ShouldReturnGeneratedIds()
        {
            var cinemaHallIds = await _fluentEntity.InsertInto(_insertHelper.CinemaHallMocks).ExecuteAsync();
            var rowCategoryIds = await _fluentEntity.InsertInto(_insertHelper.RowCategoryMocks).ExecuteAsync();
            var rowId = await _fluentEntity.InsertInto(_insertHelper.RowMock).ExecuteAsync();
            var seatIds = await _fluentEntity.InsertInto(_insertHelper.SeatMocks).ExecuteAsync();

            AssertGeneratedIds(cinemaHallIds);
            AssertGeneratedIds(rowCategoryIds); 
            AssertGeneratedIds(rowId);
            AssertGeneratedIds(seatIds);

            var seatsWithRows = await _fluentEntity.SelectAll<SeatMock>()
                .InnerJoin<SeatMock, RowMock, long, long>(sm => sm.RowMock, sm => sm.RowId, r => r.Id)
                .QueryAsync();

            seatsWithRows.ToList().ForEach(seat =>
            {
                seat.RowMock.Should().NotBeNull();
            });
        }

        private static void AssertGeneratedIds(IEnumerable<long> ids)
        {
            var currentId = 1L;

            foreach (var id in ids)
            {
                id.Should().Be(currentId);
                ++currentId;
            }
        }


        [Test]
        public async Task InsertNewEntitiesWithNullIds_ShouldReturnGeneratedEntity()
        {
             await _fluentEntity.InsertInto(_insertHelper.CityMocks).ExecuteAsync();
             await _fluentEntity.InsertInto(_insertHelper.AddressMocks).ExecuteAsync();
             await _fluentEntity.InsertInto(_insertHelper.RoleMocks).ExecuteAsync();
             await _fluentEntity.InsertInto(_insertHelper.UserMocks).ExecuteAsync();


             await _fluentEntity.InsertInto(_insertHelper.GenreMocks).ExecuteAsync();
             await _fluentEntity.InsertInto(_insertHelper.ActorMocks).ExecuteAsync();
             await _fluentEntity.InsertInto(_insertHelper.MovieMocks).ExecuteAsync();
             await _fluentEntity.InsertInto(_insertHelper.MovieActorMocks).ExecuteAsync();

             await _fluentEntity.InsertInto(_insertHelper.CinemaHallMocks).ExecuteAsync();

             await _fluentEntity.InsertInto(_insertHelper.ScheduleMocks).ExecuteAsync();

            var reservationIds = await _fluentEntity.InsertInto(_insertHelper.ReservationMocks).ExecuteAsync();

            AssertGeneratedIds(reservationIds);
            var reservationMocks = await _fluentEntity.SelectAll<ReservationMock>().QueryAsync();

            reservationMocks.ToList().ForEach(reservation =>
            {
                reservation.TicketId.Should().Be(0L);
                reservation.Ticket.Should().BeNull();
            });
        }
    }

}
