using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Apollo.Domain.Entity;
using Apollo.Persistence.Dao.Interfaces;
using Apollo.Persistence.FluentEntity.Interfaces.Select;
using Apollo.Persistence.Util;

namespace Apollo.Persistence.Dao.Ado
{
    public class ScheduleDaoAdo: BaseDao<Schedule>, IScheduleDao {
        public ScheduleDaoAdo(IConnectionFactory connectionFactory) : base(connectionFactory)
        {
        }

        public async Task<IEnumerable<Schedule>> SelectByMovieTitleAsync(string title)
        {
            var schedules = await FluentSelectAll()
                .InnerJoin<Schedule, CinemaHall, long, long>(s => s.CinemaHall, s => s.CinemaHallId, c => c.Id)
                .WhereActive()
                .QueryAsync();
            var scheduleList = schedules.ToList();
            scheduleList.ForEach(async s => s.Movie = await FetchMovieWithoutImage(s.MovieId));
            return scheduleList.Where(schedule => schedule.Movie.Title == title);
        }

        public async Task<IEnumerable<Schedule>> SelectByPriceAsync(decimal price)
        {
            var schedules = await FluentSelectAll()
                .InnerJoin<Schedule, CinemaHall, long, long>(s => s.CinemaHall, s => s.CinemaHallId, c => c.Id)
                .WhereActive()
                .And(schedule => schedule.Price)
                .Equal(price)
                .QueryAsync();
            var scheduleList = schedules.ToList();
            scheduleList.ForEach(async s => s.Movie = await FetchMovieWithoutImage(s.MovieId));
            return scheduleList;
        }

        public async Task<IEnumerable<Schedule>> SelectByStartTimeAsync(DateTime startTime)
        {
            var schedules = await FluentSelectAll()
                .InnerJoin<Schedule, CinemaHall, long, long>(s => s.CinemaHall, s => s.CinemaHallId, c => c.Id)
                .WhereActive()
                .And(schedule => schedule.StartTime)
                .Equal(startTime)
                .QueryAsync();
            var scheduleList = schedules.ToList();
            scheduleList.ForEach(async s => s.Movie = await FetchMovieWithoutImage(s.MovieId));
            return scheduleList;
        }

        public async Task<IEnumerable<Schedule>> SelectInTimeRangeAsync(DateTime from, DateTime to)
        {
            var schedules = await FluentSelectAll()
                .InnerJoin<Schedule, CinemaHall, long, long>(s => s.CinemaHall, s => s.CinemaHallId, c => c.Id)
                .WhereActive()
                .And(schedule => schedule.StartTime)
                .GreaterThanEquals(from)
                .And(schedule => schedule.StartTime)
                .LowerThanEquals(to)
                .QueryAsync();
            var scheduleList = schedules.ToList();
            scheduleList.ForEach(async s =>
            {
                s.Movie = await FetchMovieWithoutImageAndGenre(s.MovieId);
            });
            return scheduleList;
        }

        public async Task<IEnumerable<Schedule>> SelectByTitleAndPriceAsync(string title, decimal price)
        {
            var schedules = await FluentSelectAll()
                .InnerJoin<Schedule, CinemaHall, long, long>(s => s.CinemaHall, s => s.CinemaHallId, c => c.Id)
                .WhereActive()
                .And(schedule => schedule.Price)
                .Equal(price)
                .QueryAsync();
            var scheduleList = schedules.ToList();
            scheduleList.ForEach(async s => s.Movie = await FetchMovieWithoutImage(s.MovieId));
            return scheduleList.Where(schedule => schedule.Movie.Title == title);
        }

        public async Task<IEnumerable<Schedule>> SelectByPriceAndStartTimeAsync(decimal price, DateTime startTime)
        {
            var schedules = await FluentSelectAll()
                .InnerJoin<Schedule, CinemaHall, long, long>(s => s.CinemaHall, s => s.CinemaHallId, c => c.Id)
                .WhereActive()
                .And(schedule => schedule.StartTime)
                .Equal(startTime)
                .And(schedule => schedule.Price)
                .Equal(price)
                .QueryAsync();
            var scheduleList = schedules.ToList();
            scheduleList.ForEach(async s => s.Movie = await FetchMovieWithoutImage(s.MovieId));
            return scheduleList;
        }

        public async Task<IEnumerable<Schedule>> SelectByTitleAndStartTimeAsync(string title, DateTime startTime)
        {
            var schedules = await FluentSelectAll()
                .InnerJoin<Schedule, Movie, long, long>(s => s.Movie, s => s.MovieId, m => m.Id)
                .InnerJoin<Schedule, CinemaHall, long, long>(s => s.CinemaHall, s => s.CinemaHallId, c => c.Id)
                .WhereActive()
                .And(schedule => schedule.StartTime)
                .Equal(startTime)
                .QueryAsync();
            var scheduleList = schedules.ToList();
            scheduleList.ForEach(async s => s.Movie = await FetchMovieWithoutImage(s.MovieId));
            return scheduleList.Where(schedule => schedule.Movie.Title == title);
        }

        public  async Task<IEnumerable<Schedule>> SelectByTitlePriceAndStartTimeAsync(string title, decimal price, DateTime startTime)
        {
            var schedules = await FluentSelectAll()
                .InnerJoin<Schedule, CinemaHall, long, long>(s => s.CinemaHall, s => s.CinemaHallId, c => c.Id)
                .WhereActive()
                .And(schedule => schedule.Price)
                .Equal(price)
                .And(schedule => schedule.StartTime)
                .Equal(startTime)
                .QueryAsync();
            var scheduleList = schedules.ToList();
            scheduleList.ForEach(async s => s.Movie = await FetchMovieWithoutImage(s.MovieId));
            return scheduleList.Where(schedule => schedule.Movie.Title == title);
        }

        private async Task<Movie> FetchMovieWithoutImage(long id)
        {
            return await _fluentEntity.Select<Movie>()
                .Column(_ => _.Id)
                .Column(_ => _.RowVersion)
                .Column(_ => _.Title)
                .Column(_ => _.Description)
                .Column(_ => _.GenreId)
                .Column(_ => _.Duration)
                .Column(_ => _.Trailer)
                .Column(_ => _.Rating)
                .Column(_ => _.Deleted)
                .Where(_ => _.Id)
                .Equal(id)
                .QuerySingleAsync();
        }


        private async Task<Movie> FetchMovieWithoutImageAndGenre(long id)
        {
            return await _fluentEntity.Select<Movie>()
                .Column(_ => _.Id)
                .Column(_ => _.RowVersion)
                .Column(_ => _.Title)
                .Column(_ => _.Description)
                .Column(_ => _.GenreId)
                .Column(_ => _.Duration)
                .Column(_ => _.Trailer)
                .Column(_ => _.Rating)
                .Column(_ => _.Deleted)
                .ColumnRef<Genre, long>(_ => _.Id)
                .ColumnRef<Genre, string>(_ => _.Name)
                .ColumnRef<Genre, bool>(_ => _.Deleted)
                .ColumnRef<Genre, DateTime>(_ => _.RowVersion)
                .InnerJoin<Movie, Genre, long, long>(s => s.Genre, m => m.GenreId, g => g.Id)
                .Where(_ => _.Id)
                .Equal(id)
                .QuerySingleAsync();
        }
    }
}
