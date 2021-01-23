using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Apollo.Domain.Entity;
using Apollo.Persistence.Dao.Interfaces;
using Apollo.Persistence.Util;

namespace Apollo.Persistence.Dao.Ado
{
    public class ReservationDaoAdo : BaseDao<Reservation>, IReservationDao
    {
        public ReservationDaoAdo(IConnectionFactory connectionFactory) : base(connectionFactory)
        {
        }

        public async Task<IEnumerable<Reservation>> SelectReservationsByScheduleId(long scheduleId)
        {
            return await FluentSelectAll()
                .WhereActive()
                .And(r => r.ScheduleId)
                .Equal(scheduleId)
                .QueryAsync();
        }

        public async Task<IEnumerable<Reservation>> SelectReservationsByUserId(long userId)
        {
            return await FluentSelectAll()
                .Where(r => r.UserId)
                .Equal(userId)
                .QueryAsync();
        }

        public async Task<IEnumerable<Reservation>> SelectReservationsWithTicketsByUserId(long userId)
        {
            return await FluentSelectAll()
                .InnerJoin<Reservation, Ticket, long, long>(r => r.Ticket, r => r.TicketId, t => t.Id)
                .Where(reservation => reservation.UserId)
                .Equal(userId)
                .QueryAsync();
        }

        public async Task<int> CountReservationsByCinemaHallIdAsync(long cinemaHallId)
        {
            return (await FluentSelect()
                .Column(_ => _.Id)
                .InnerJoin<Reservation, Schedule, long, long>(r => r.Schedule, r => r.ScheduleId, s => s.Id)
                .InnerJoin<Schedule, CinemaHall, long, long>(r => r.Schedule.CinemaHall, s => s.CinemaHallId, c => c.Id)
                .Where(r => r.Schedule.CinemaHallId)
                .Equal(cinemaHallId)
                .QueryAsync()).Count();
        }
    }
}
