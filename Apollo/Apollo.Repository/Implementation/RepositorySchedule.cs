using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Apollo.Domain.Entity;
using Apollo.Persistence.Dao.Interfaces;
using Apollo.Persistence.Util;
using Apollo.Repository.Interfaces;
using Apollo.Util;
using Apollo.Util.Logger;

namespace Apollo.Repository.Implementation
{
    public class RepositorySchedule : Repository, IRepositorySchedule
    {
        private static readonly IApolloLogger<RepositorySchedule> Logger =
            LoggerFactory.CreateLogger<RepositorySchedule>();

        private const int OneDay = 1;
        private const int BatchSize = 250;
        private const int BreakTime = 6;
        private readonly IScheduleDao _scheduleDao;
        private readonly ICinemaHallDao _cinemaHallDao;
        private readonly IMovieDao _movieDao;
        public decimal MinPriceWeekday { get; set; }
        public decimal MaxPriceWeekday { get; set; }
        public decimal MinPriceWeekend { get; set; }
        public decimal MaxPriceWeekend { get; set; }
        public int BufferTime { get; set; }
        public int MinSchedulesPerDay { get; set; }
        public int MaxSchedulesPerDay { get; set; }

        public RepositorySchedule(IConnectionFactory connectionFactory) : base(connectionFactory)
        {
            _scheduleDao = DaoFactory.CreateScheduleDao();
            _cinemaHallDao = DaoFactory.CreateCinemaHallDao();
            _movieDao = DaoFactory.CreateMovieDao();
            MinPriceWeekday = 7.0M;
            MaxPriceWeekday = 12.5M;
            MinPriceWeekend = 9.5M;
            MaxPriceWeekend = 15.5M;
            MinSchedulesPerDay = 6;
            MaxSchedulesPerDay = 10;
        }

        public async Task AddSchedulesForMoviesAsync(DateTime from, DateTime to, IEnumerable<Movie> movies)
        {
            ValidateDateRange(from, to);
            ValidateCollection(movies);
            var moviesList = movies.ToList();
            var schedules = new List<Schedule>();
            var cinemaHalls = (await _cinemaHallDao.FluentSelectAll().QueryAsync()).ToList();
            var cinemaHallId = cinemaHalls.Any()
                ? cinemaHalls.First().Id
                : throw new ArgumentNullException(nameof(AddSchedulesForMoviesAsync));
            var current = from;
            var batchSize = 0;
            while (current != to)
            {
                var scheduleThisDay = RandomGenerator.GenerateRandomNumber(MinSchedulesPerDay, MaxSchedulesPerDay);
                var lastStartTime = new DateTime(current.Year, current.Month, current.Day, 0, 0, 0, 0);
                var movie = GetRandomMovie(moviesList);
                for (var i = 0; i < scheduleThisDay; ++i)
                {
                    Logger.Here().Info("{StartTime} {scheduleThisDay}", lastStartTime, scheduleThisDay);
                    if (batchSize == BatchSize) //to avoid huge memory consumption flush after batch size is reached
                    {
                        Logger.Here().Info("Processing batch with {currentSchedules} {batchSize}", schedules.Count, batchSize);
                        await AddSchedulesAsync(schedules);
                        schedules.Clear();
                        batchSize = 0;
                        Logger.Here().Info("Batch processed - Reset batch count {currentSchedules} {batchSize}", schedules.Count, batchSize);
                    }

                    movie = NewRandomMovie(moviesList, i, movie);

                    schedules.Add(new Schedule
                    {
                        CinemaHallId = cinemaHallId,
                        MovieId = movie.Id,
                        Price = GetRandomPrice(lastStartTime.IsWeekend()),
                        StartTime = lastStartTime
                    });

                    if (lastStartTime.WouldExceedDay(movie.Duration + BufferTime)) break;
                    lastStartTime = IsFirstStartTime(lastStartTime) ?
                        lastStartTime.AddHours(BreakTime) :
                        lastStartTime.AddMinutes(movie.Duration).AddMinutes(BufferTime);
                    ++batchSize;
                }
                current = current.AddDays(OneDay);
            }
            if (schedules.Count > 0)
            {
                await AddSchedulesAsync(schedules);
            }
        }

        private Movie NewRandomMovie(IEnumerable<Movie> movies, int i, Movie movie)
        {
            if (i % 2 == 1)
            {
                movie = GetRandomMovie(movies);
            }
            return movie;
        }

        private static bool IsFirstStartTime(DateTime lastStartTime)
        {
            return lastStartTime.Hour == 0 && lastStartTime.Minute == 0 && lastStartTime.Second == 0;
        }

        public async Task<long> AddScheduleAsync(Schedule newSchedule)
        {
            await ValidateId(_cinemaHallDao, newSchedule.CinemaHallId);
            await ValidateId(_movieDao, newSchedule.MovieId);
            var result = await _scheduleDao.FluentInsert(newSchedule).ExecuteAsync();
            return result.Single();
        }

        public async Task<Schedule> GetScheduleByIdAsync(long id)
        {
            await ValidateId(_scheduleDao, id);
            return await _scheduleDao.SelectSingleByIdAsync(id);
        }

        public async Task<bool> IdExistAsync(long id)
        {
            return await _scheduleDao.ExistAndNotDeletedByIdAsync(id);
        }

        public async Task<IEnumerable<Schedule>> GetSchedulesAsync()
        {
            return await _scheduleDao.SelectActiveAsync();
        }

        public async Task<int> DeleteScheduleAsync(Schedule newSchedule)
        {
            ValidateNotNull(newSchedule);
            await ValidateId(_scheduleDao, newSchedule.Id);
            return await _scheduleDao.FluentSoftDeleteById(newSchedule.Id);
        }
        public async Task<bool> UpdateScheduleAsync(Schedule schedule)
        {
            ValidateNotNull(schedule);
            await ValidateId(_scheduleDao, schedule.Id);
            return await _scheduleDao.FluentUpdate(schedule).ExecuteAsync() > 0;
        }

        public async Task<IEnumerable<long>> AddSchedulesAsync(IEnumerable<Schedule> newSchedules)
        {
            ValidateCollection(newSchedules);
            await ValidateIds(_movieDao, newSchedules.Select(s => s.MovieId));
            await ValidateIds(_cinemaHallDao, newSchedules.Select(s => s.CinemaHallId));
            return await _scheduleDao.FluentInsert(newSchedules).ExecuteAsync();
        }

        public async Task<IEnumerable<Schedule>> GetByMovieTitleAsync(string title)
        {
            ValidateNotNull(title);
            return await _scheduleDao.SelectByMovieTitleAsync(title);
        }

        public async Task<IEnumerable<Schedule>> GetByPriceAsync(decimal price)
        {
            return await _scheduleDao.SelectByPriceAsync(price);
        }

        public async Task<IEnumerable<Schedule>> GetStartTimeAsync(DateTime startTime)
        {
            ValidateNotNull(startTime);
            return await _scheduleDao.SelectByStartTimeAsync(startTime);
        }

        public async Task<IEnumerable<Schedule>> GetInTimeRangeAsync(DateTime from, DateTime to)
        {
            ValidateNotNull(from);
            ValidateNotNull(to);
            ValidateDateRange(from, to);
            return await _scheduleDao.SelectInTimeRangeAsync(from, to);
        }

        public async Task<IEnumerable<Schedule>> GetByTitleAndPriceAsync(string title, decimal price)
        {
            ValidateNotNull(title);
            return await _scheduleDao.SelectByTitleAndPriceAsync(title, price);
        }

        public async Task<IEnumerable<Schedule>> GetByPriceAndStartTimeAsync(decimal price, DateTime startTime)
        {
            ValidateNotNull(startTime);
            return await _scheduleDao.SelectByPriceAndStartTimeAsync(price, startTime);
        }

        public async Task<IEnumerable<Schedule>> GetByTitleAndStartTimeAsync(string title, DateTime startTime)
        {
            ValidateNotNull(startTime);
            return await _scheduleDao.SelectByTitleAndStartTimeAsync(title, startTime);
        }

        public async Task<IEnumerable<Schedule>> GetByTitlePriceAndStartTimeAsync(string title, decimal price, DateTime startTime)
        {

            ValidateNotNull(title);
            ValidateNotNull(startTime);
            return await _scheduleDao.SelectByTitlePriceAndStartTimeAsync(title, price, startTime);
        }


        private Movie GetRandomMovie(IEnumerable<Movie> movies)
        {
            ValidateCollection(movies);
            return movies.ElementAt(RandomGenerator.GenerateRandomNumber(0, movies.Count() - 1));
        }


        private async Task<long> CreateCinemaHall(string label)
        {
            ValidateNotNull(label);
            var ids = await _cinemaHallDao.FluentInsert(new CinemaHall { Label = label }).ExecuteAsync();
            return ids.Single();
        }

        private decimal GetRandomPrice(bool isWeekend)
        {
            return RandomGenerator.GenerateRandomNumber(
                isWeekend ? MinPriceWeekend : MinPriceWeekday,
                isWeekend ? MaxPriceWeekend : MaxPriceWeekday
            );
        }
    }
}