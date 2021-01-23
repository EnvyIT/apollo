using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Apollo.Domain.Entity;
using Apollo.Persistence.Exception;
using Apollo.Util;
using FluentAssertions;
using NUnit.Framework;

namespace Apollo.Repository.Test
{
    public class RepositoryScheduleTest
    {
        private RepositoryHelper _repositoryHelper;

        [SetUp]
        public async Task Setup()
        {
            _repositoryHelper = new RepositoryHelper();
            _repositoryHelper.FillTypes.AddAll(new List<EntityType> {EntityType.Movie, EntityType.CinemaHall});
            await _repositoryHelper.SeedDatabase();
        }

        [TearDown]
        public async Task TearDown()
        {
            await _repositoryHelper.TearDown();
        }

        [Test]
        
        public async Task CreateRandomSchedules_ShouldThrowArgumentNullExceptionIfNoneExists()
        {
            await _repositoryHelper.FluentEntity.Delete<CinemaHall>().ExecuteAsync();

            var movies = (await _repositoryHelper.FluentEntity.SelectAll<Movie>().QueryAsync()).ToList();
            var scheduleRepository = _repositoryHelper.RepositoryFactory.CreateRepositorySchedule();
            var from = new DateTime(2020, 5, 1);
            var to = new DateTime(2020, 5, 31);
            Func<Task> addSchedules = async () => await scheduleRepository.AddSchedulesForMoviesAsync(from, to, movies);
            await addSchedules.Should().ThrowExactlyAsync<ArgumentNullException>();
        }


        [Test]
        public async Task CreateRandomSchedulesWithMovies_ShouldInsertRandomSchedules()
        {
            var movies = (await _repositoryHelper.FluentEntity.SelectAll<Movie>().QueryAsync()).ToList();
            var scheduleRepository = _repositoryHelper.RepositoryFactory.CreateRepositorySchedule();
            var from = new DateTime(2020, 5, 1);
            var to = new DateTime(2020, 5, 31);
            Func<Task> createSchedules = async () => await scheduleRepository.AddSchedulesForMoviesAsync(from, to, movies);
            await createSchedules.Should().NotThrowAsync();

            var schedules = (await _repositoryHelper.FluentEntity.SelectAll<Schedule>().QueryAsync()).ToList();
            schedules.Count.Should().BeGreaterThan(0);
       

            var minPrice = schedules.Min(s => (s.Price, s.StartTime));
            var maxPrice = schedules.Max(s => (s.Price, s.StartTime));
            minPrice.StartTime.IsWeekend().Should().BeFalse();
            maxPrice.StartTime.IsWeekend().Should().BeTrue();

            minPrice.Price.Should().BeGreaterOrEqualTo(scheduleRepository.MinPriceWeekday);

            maxPrice.Price.Should().BeLessOrEqualTo(scheduleRepository.MaxPriceWeekend);
        }

        [Test]
        public async Task ScheduleCreationWithoutAnyMovies_ShouldThrowIllegalArgumentException()
        {
            var scheduleRepository = _repositoryHelper.RepositoryFactory.CreateRepositorySchedule();
            var from = new DateTime(2020, 5, 1);
            var to = new DateTime(2020, 5, 31);
            Func<Task> createSchedules = async () => await scheduleRepository.AddSchedulesForMoviesAsync(from, to, new List<Movie>()); 
            await createSchedules.Should().ThrowExactlyAsync<ArgumentException>();
        }


        [Test]
        public async Task ScheduleCreationWithFromBeforeTo_ShouldThrowIllegalArgumentException()
        {
            var movies = (await _repositoryHelper.FluentEntity.SelectAll<Movie>().QueryAsync()).ToList();
            var scheduleRepository = _repositoryHelper.RepositoryFactory.CreateRepositorySchedule();
            var from = new DateTime(2020, 5, 1);
            var to = new DateTime(2020, 5, 31);
            Func<Task> createSchedules = async () => await scheduleRepository.AddSchedulesForMoviesAsync(to, from, movies);
            await createSchedules.Should().ThrowExactlyAsync<ArgumentException>();
        }

        [Test]
        public async Task GetScheduleById_InvalidId_Should_ThrowException()
        {
            var scheduleRepository = _repositoryHelper.RepositoryFactory.CreateRepositorySchedule();
            Func<Task> createSchedules = async () => await scheduleRepository.GetScheduleByIdAsync(500L);
            await createSchedules.Should().ThrowAsync<InvalidEntityIdException>();
        }

        [Test]
        public async Task GetScheduleById_ShouldReturnEntity()
        {
            var startTime = new DateTime(2020, 12, 24, 17, 45, 00);
            var scheduleRepository = _repositoryHelper.RepositoryFactory.CreateRepositorySchedule();
            var newSchedule = new Schedule { CinemaHallId = 1L, MovieId = 1L, Price = 15, StartTime = startTime };

            var scheduleId = await scheduleRepository.AddScheduleAsync(newSchedule);
            newSchedule.Id = scheduleId;

            var schedule = await scheduleRepository.GetScheduleByIdAsync(scheduleId);

            schedule.Should().Be(newSchedule);
        }

        [Test]
        public async Task AddNewSchedule_ShouldReturnNewScheduleInDatabase()
        {
            var startTime = new DateTime(2020, 12, 24, 17, 45, 00);
            const long cinemaHallId = 1L;
            const long movieId = 1L;
            const decimal price = 18.50M;
            var newSchedule = new Schedule
                {CinemaHallId = cinemaHallId, MovieId = movieId, Price = price, StartTime = startTime};

            var scheduleRepository = _repositoryHelper.RepositoryFactory.CreateRepositorySchedule();

            var scheduleId = await scheduleRepository.AddScheduleAsync(newSchedule);
            scheduleId.Should().NotBe(0L);

            var schedules = await scheduleRepository.GetSchedulesAsync();
            schedules.Should().HaveCount(1);
            schedules.ElementAt(0).Price.Should().Be(price);
            schedules.ElementAt(0).StartTime.Should().Be(startTime);
            schedules.ElementAt(0).MovieId.Should().Be(movieId);
            schedules.ElementAt(0).CinemaHallId.Should().Be(cinemaHallId);
        }

        [Test]
        public async Task AddMultipleSchedules_ShouldReturnMultipleSchedules()
        {
            var startTime = new DateTime(2020, 12, 24, 17, 00, 00);
            const long cinemaHallId = 1L;
            const decimal price = 18.50M;
            var newSchedules = new List<Schedule>
            {
                new Schedule {CinemaHallId = cinemaHallId, MovieId = 1L, Price = price, StartTime = startTime},
                new Schedule {CinemaHallId = cinemaHallId, MovieId = 2L, Price = price, StartTime = startTime.AddSeconds(1337)},
                new Schedule {CinemaHallId = cinemaHallId, MovieId = 3L, Price = price, StartTime = startTime.AddSeconds(4242)},
                new Schedule {CinemaHallId = cinemaHallId, MovieId = 4L, Price = price, StartTime = startTime.AddSeconds(9120)},
                new Schedule {CinemaHallId = cinemaHallId, MovieId = 4L, Price = price, StartTime = startTime.AddSeconds(9120)},
            };
            var scheduleRepository = _repositoryHelper.RepositoryFactory.CreateRepositorySchedule();

            var scheduleIds = await scheduleRepository.AddSchedulesAsync(newSchedules);
            scheduleIds.Should().HaveCount(5);
            scheduleIds.Any(id => id == 0L).Should().BeFalse();
        }

     
        [Test]
        public async Task SoftDeleteSchedule_ShouldReturnSchedulesWithoutDeletedFlag()
        {
            var startTime = new DateTime(2020, 12, 24, 17, 45, 00);
            const long cinemaHallId = 1L;
            const long movieId = 1L;
            const decimal price = 18.50M;
            var newSchedule = new Schedule
                { CinemaHallId = cinemaHallId, MovieId = movieId, Price = price, StartTime = startTime };

            var scheduleRepository = _repositoryHelper.RepositoryFactory.CreateRepositorySchedule();

            var scheduleId = await scheduleRepository.AddScheduleAsync(newSchedule);
            newSchedule.Id = scheduleId;

            var deletCount = await scheduleRepository.DeleteScheduleAsync(newSchedule);
            deletCount.Should().Be(1);
            var schedules = await scheduleRepository.GetSchedulesAsync();
            schedules.Should().HaveCount(0);

        }

        [Test]
        public async Task FilterSchedulesByMovieTitle_ShouldReturnCorrectSchedules()
        {
            var startTime = new DateTime(2020, 12, 24, 17, 45, 00);
            const long cinemaHallId = 1L;
            const decimal price = 18.50M;
            var newSchedules = new List<Schedule>
            {
                new Schedule {CinemaHallId = cinemaHallId, MovieId = 1L, Price = price, StartTime = startTime},
                new Schedule {CinemaHallId = cinemaHallId, MovieId = 2L, Price = price, StartTime = startTime},
                new Schedule {CinemaHallId = cinemaHallId, MovieId = 3L, Price = price, StartTime = startTime},
                new Schedule {CinemaHallId = cinemaHallId, MovieId = 4L, Price = price, StartTime = startTime},
                new Schedule {CinemaHallId = cinemaHallId, MovieId = 4L, Price = price, StartTime = startTime},
            };
            
            var scheduleRepository = _repositoryHelper.RepositoryFactory.CreateRepositorySchedule();

            var scheduleIds = await scheduleRepository.AddSchedulesAsync(newSchedules);
            scheduleIds.Should().HaveCount(5);

            var filteredSchedules = await scheduleRepository.GetByMovieTitleAsync("Salt");
            filteredSchedules.Should().HaveCount(1);
            filteredSchedules.ElementAt(0).Movie.Should().NotBeNull();
            filteredSchedules.ElementAt(0).Movie.Title.Should().Be("Salt");
        }

        [Test]
        public async Task FilterSchedulesByStartTime_ShouldReturnCorrectSchedules()
        {
            var startTime = new DateTime(2020, 12, 24, 17, 00, 00);
            const long cinemaHallId = 1L;
            const decimal price = 18.50M;
            const double bufferTime = 900;
            var newSchedules = new List<Schedule>
            {
                new Schedule {CinemaHallId = cinemaHallId, MovieId = 1L, Price = price, StartTime = startTime},
                new Schedule {CinemaHallId = cinemaHallId, MovieId = 2L, Price = price, StartTime = startTime.AddSeconds(1200).AddSeconds(bufferTime)},
                new Schedule {CinemaHallId = cinemaHallId, MovieId = 3L, Price = price, StartTime = startTime.AddSeconds(1337).AddSeconds(bufferTime)},
                new Schedule {CinemaHallId = cinemaHallId, MovieId = 4L, Price = price, StartTime = startTime.AddSeconds(4242).AddSeconds(bufferTime)},
                new Schedule {CinemaHallId = cinemaHallId, MovieId = 4L, Price = price, StartTime = startTime.AddSeconds(9120).AddSeconds(bufferTime)}
            };

            var scheduleRepository = _repositoryHelper.RepositoryFactory.CreateRepositorySchedule();

            var scheduleIds = await scheduleRepository.AddSchedulesAsync(newSchedules);
            scheduleIds.Should().HaveCount(5);

            var filteredSchedules = await scheduleRepository.GetStartTimeAsync(startTime);
            filteredSchedules.Should().HaveCount(1);
            filteredSchedules.ElementAt(0).MovieId.Should().Be(1L);
        }

        [Test]
        public async Task FilterSchedulesByTimeRange_ShouldReturnCorrectSchedules()
        {
            var startTime = new DateTime(2020, 12, 24, 17, 00, 00);
            var nextDayStartTime = startTime.AddDays(1);
            var dayBeforeStartTime = startTime.AddDays(-1);
            var nextStartTime = startTime.AddSeconds(4337);
            const long cinemaHallId = 1L;
            const decimal price = 18.50M;
            const double bufferTime = 900;
            var newSchedules = new List<Schedule>
            {
                new Schedule {CinemaHallId = cinemaHallId, MovieId = 1L, Price = price, StartTime = dayBeforeStartTime},
                new Schedule {CinemaHallId = cinemaHallId, MovieId = 2L, Price = price, StartTime = dayBeforeStartTime.AddSeconds(1200).AddSeconds(bufferTime)},
                new Schedule {CinemaHallId = cinemaHallId, MovieId = 3L, Price = price, StartTime = dayBeforeStartTime.AddSeconds(1337).AddSeconds(bufferTime)},
                new Schedule {CinemaHallId = cinemaHallId, MovieId = 4L, Price = price, StartTime = dayBeforeStartTime.AddSeconds(4242).AddSeconds(bufferTime)},
                new Schedule {CinemaHallId = cinemaHallId, MovieId = 1L, Price = price, StartTime = dayBeforeStartTime.AddSeconds(9120).AddSeconds(bufferTime)},
                new Schedule {CinemaHallId = cinemaHallId, MovieId = 1L, Price = price, StartTime = startTime},
                new Schedule {CinemaHallId = cinemaHallId, MovieId = 2L, Price = price, StartTime = startTime.AddSeconds(1200).AddSeconds(bufferTime)},
                new Schedule {CinemaHallId = cinemaHallId, MovieId = 3L, Price = price, StartTime = startTime.AddSeconds(1337).AddSeconds(bufferTime)},
                new Schedule {CinemaHallId = cinemaHallId, MovieId = 4L, Price = price, StartTime = startTime.AddSeconds(4242).AddSeconds(bufferTime)},
                new Schedule {CinemaHallId = cinemaHallId, MovieId = 1L, Price = price, StartTime = startTime.AddSeconds(9120).AddSeconds(bufferTime)},
                new Schedule {CinemaHallId = cinemaHallId, MovieId = 2L, Price = price, StartTime = nextDayStartTime},
                new Schedule {CinemaHallId = cinemaHallId, MovieId = 3L, Price = price, StartTime = nextDayStartTime.AddSeconds(1200).AddSeconds(bufferTime)},
                new Schedule {CinemaHallId = cinemaHallId, MovieId = 4L, Price = price, StartTime = nextDayStartTime.AddSeconds(4000).AddSeconds(bufferTime)},
                new Schedule {CinemaHallId = cinemaHallId, MovieId = 1L, Price = price, StartTime = nextDayStartTime.AddSeconds(5600).AddSeconds(bufferTime)},
                new Schedule {CinemaHallId = cinemaHallId, MovieId = 3L, Price = price, StartTime = nextDayStartTime.AddSeconds(9120).AddSeconds(bufferTime)}
            };

            var scheduleRepository = _repositoryHelper.RepositoryFactory.CreateRepositorySchedule();

            var scheduleIds = await scheduleRepository.AddSchedulesAsync(newSchedules);
            scheduleIds.Should().HaveCount(15);

            var filteredSchedules = await scheduleRepository.GetInTimeRangeAsync(startTime, nextStartTime);
            filteredSchedules.Should().HaveCount(3);
            filteredSchedules.ElementAt(0).MovieId.Should().Be(1L);
            filteredSchedules.ElementAt(1).MovieId.Should().Be(2L);
            filteredSchedules.ElementAt(2).MovieId.Should().Be(3L);
        }


        [Test]
        public async Task FilterSchedulesByPrice_ShouldReturnNoScheduleFound()
        {
            var startTime = new DateTime(2020, 12, 24, 17, 00, 00);
            const long cinemaHallId = 1L;
            const decimal price = 18.50M;
            const double bufferTime = 900;
            var newSchedules = new List<Schedule>
            {
                new Schedule {CinemaHallId = cinemaHallId, MovieId = 1L, Price = 12.5M, StartTime = startTime},
                new Schedule {CinemaHallId = cinemaHallId, MovieId = 2L, Price = 17.9M, StartTime = startTime.AddSeconds(1200).AddSeconds(bufferTime)},
                new Schedule {CinemaHallId = cinemaHallId, MovieId = 3L, Price = 9.8M, StartTime = startTime.AddSeconds(1337).AddSeconds(bufferTime)},
                new Schedule {CinemaHallId = cinemaHallId, MovieId = 4L, Price = 12.5M, StartTime = startTime.AddSeconds(4242).AddSeconds(bufferTime)},
                new Schedule {CinemaHallId = cinemaHallId, MovieId = 4L, Price = 22.75M, StartTime = startTime.AddSeconds(9120).AddSeconds(bufferTime)}
            };

            var scheduleRepository = _repositoryHelper.RepositoryFactory.CreateRepositorySchedule();

            var scheduleIds = await scheduleRepository.AddSchedulesAsync(newSchedules);
            scheduleIds.Should().HaveCount(5);

            var filteredSchedules = await scheduleRepository.GetByPriceAsync(price);
            filteredSchedules.Should().HaveCount(0);
        }

        [Test]
        public async Task FilterSchedulesByPrice_ShouldReturn2SchedulesWithPrice()
        {
            var startTime = new DateTime(2020, 12, 24, 17, 00, 00);
            const long cinemaHallId = 1L;
            const decimal price = 12.5M;
            const double bufferTime = 900;
            var newSchedules = new List<Schedule>
            {
                new Schedule {CinemaHallId = cinemaHallId, MovieId = 1L, Price = 12.5M, StartTime = startTime},
                new Schedule {CinemaHallId = cinemaHallId, MovieId = 2L, Price = 17.9M, StartTime = startTime.AddSeconds(1200).AddSeconds(bufferTime)},
                new Schedule {CinemaHallId = cinemaHallId, MovieId = 3L, Price = 9.8M, StartTime = startTime.AddSeconds(1337).AddSeconds(bufferTime)},
                new Schedule {CinemaHallId = cinemaHallId, MovieId = 4L, Price = 12.5M, StartTime = startTime.AddSeconds(4242).AddSeconds(bufferTime)},
                new Schedule {CinemaHallId = cinemaHallId, MovieId = 4L, Price = 22.75M, StartTime = startTime.AddSeconds(9120).AddSeconds(bufferTime)}
            };

            var scheduleRepository = _repositoryHelper.RepositoryFactory.CreateRepositorySchedule();

            var scheduleIds = await scheduleRepository.AddSchedulesAsync(newSchedules);
            scheduleIds.Should().HaveCount(5);

            var filteredSchedules = await scheduleRepository.GetByPriceAsync(price);
            filteredSchedules.Should().HaveCount(2);
            filteredSchedules.ElementAt(0).Price.Should().Be(price);
            filteredSchedules.ElementAt(1).Price.Should().Be(price);
            filteredSchedules.ElementAt(0).MovieId.Should().Be(1L);
            filteredSchedules.ElementAt(1).MovieId.Should().Be(4L);
        }

        [Test]
        public async Task FilterByTitleAndPrice_ShouldReturn2SchedulesWithPrice()
        {
            var startTime = new DateTime(2020, 12, 24, 17, 00, 00);
            const long cinemaHallId = 1L;
            const decimal price = 12.5M;
            const double bufferTime = 900;
            var newSchedules = new List<Schedule>
            {
                new Schedule {CinemaHallId = cinemaHallId, MovieId = 1L, Price = 12.5M, StartTime = startTime},
                new Schedule {CinemaHallId = cinemaHallId, MovieId = 2L, Price = 17.9M, StartTime = startTime.AddSeconds(1200).AddSeconds(bufferTime)},
                new Schedule {CinemaHallId = cinemaHallId, MovieId = 1L, Price = 9.8M, StartTime = startTime.AddSeconds(1337).AddSeconds(bufferTime)},
                new Schedule {CinemaHallId = cinemaHallId, MovieId = 3L, Price = 12.5M, StartTime = startTime.AddSeconds(4242).AddSeconds(bufferTime)},
                new Schedule {CinemaHallId = cinemaHallId, MovieId = 1L, Price = 22.75M, StartTime = startTime.AddSeconds(9120).AddSeconds(bufferTime)}
            };

            var scheduleRepository = _repositoryHelper.RepositoryFactory.CreateRepositorySchedule();

            var scheduleIds = await scheduleRepository.AddSchedulesAsync(newSchedules);
            scheduleIds.Should().HaveCount(5);

            var filteredSchedules = await scheduleRepository.GetByTitleAndPriceAsync("Salt", price);
            filteredSchedules.Should().HaveCount(1);
            filteredSchedules.ElementAt(0).Price.Should().Be(price);
            filteredSchedules.ElementAt(0).MovieId.Should().Be(1L);
        }

        [Test]
        public async Task FilterByPriceAndStarTime_ShouldReturn1Schedules()
        {
            var startTime = new DateTime(2020, 12, 24, 17, 00, 00);
            const long cinemaHallId = 1L;
            const decimal price = 12.5M;
            const double bufferTime = 900;
            var newSchedules = new List<Schedule>
            {
                new Schedule {CinemaHallId = cinemaHallId, MovieId = 1L, Price = price, StartTime = startTime},
                new Schedule {CinemaHallId = cinemaHallId, MovieId = 2L, Price = price, StartTime = startTime.AddDays(2)},
                new Schedule {CinemaHallId = cinemaHallId, MovieId = 3L, Price = price, StartTime = startTime.AddDays(1)},
                new Schedule {CinemaHallId = cinemaHallId, MovieId = 4L, Price = price, StartTime = startTime.AddSeconds(4242).AddSeconds(bufferTime)},
                new Schedule {CinemaHallId = cinemaHallId, MovieId = 4L, Price = price, StartTime = startTime.AddSeconds(9120).AddSeconds(bufferTime)}
            };

            var scheduleRepository = _repositoryHelper.RepositoryFactory.CreateRepositorySchedule();

            var scheduleIds = await scheduleRepository.AddSchedulesAsync(newSchedules);
            scheduleIds.Should().HaveCount(5);

            var filteredSchedules = await scheduleRepository.GetByPriceAndStartTimeAsync(price, startTime);
            filteredSchedules.Should().HaveCount(1);
            filteredSchedules.ElementAt(0).Price.Should().Be(price);
            filteredSchedules.ElementAt(0).MovieId.Should().Be(1L);
        }

        [Test]
        public async Task FilterByTitleAndStartTime_ShouldReturn1Schedules()
        {
            var startTime = new DateTime(2020, 12, 24, 17, 00, 00);
            const long cinemaHallId = 1L;
            const decimal price = 12.5M;
            const double bufferTime = 900;
            var newSchedules = new List<Schedule>
            {
                new Schedule {CinemaHallId = cinemaHallId, MovieId = 1L, Price = price, StartTime = startTime},
                new Schedule {CinemaHallId = cinemaHallId, MovieId = 2L, Price = price, StartTime = startTime.AddDays(2)},
                new Schedule {CinemaHallId = cinemaHallId, MovieId = 3L, Price = price, StartTime = startTime.AddDays(1)},
                new Schedule {CinemaHallId = cinemaHallId, MovieId = 4L, Price = price, StartTime = startTime.AddSeconds(4242).AddSeconds(bufferTime)},
                new Schedule {CinemaHallId = cinemaHallId, MovieId = 4L, Price = price, StartTime = startTime.AddSeconds(9120).AddSeconds(bufferTime)}
            };

            var scheduleRepository = _repositoryHelper.RepositoryFactory.CreateRepositorySchedule();

            var scheduleIds = await scheduleRepository.AddSchedulesAsync(newSchedules);
            scheduleIds.Should().HaveCount(5);

            var filteredSchedules = await scheduleRepository.GetByTitleAndStartTimeAsync("Salt", startTime);
            filteredSchedules.Should().HaveCount(1);
            filteredSchedules.ElementAt(0).Price.Should().Be(price);
            filteredSchedules.ElementAt(0).MovieId.Should().Be(1L);
        }

        [Test]
        public async Task FilterByAllCriterias_ShouldReturn1Schedules()
        {
            var startTime = new DateTime(2020, 12, 24, 17, 00, 00);
            const long cinemaHallId = 1L;
            const decimal price = 12.5M;
            const double bufferTime = 900;
            var newSchedules = new List<Schedule>
            {
                new Schedule {CinemaHallId = cinemaHallId, MovieId = 1L, Price = price, StartTime = startTime},
                new Schedule {CinemaHallId = cinemaHallId, MovieId = 2L, Price = price, StartTime = startTime.AddDays(2)},
                new Schedule {CinemaHallId = cinemaHallId, MovieId = 3L, Price = price, StartTime = startTime.AddDays(1)},
                new Schedule {CinemaHallId = cinemaHallId, MovieId = 4L, Price = price, StartTime = startTime.AddSeconds(4242).AddSeconds(bufferTime)},
                new Schedule {CinemaHallId = cinemaHallId, MovieId = 4L, Price = price, StartTime = startTime.AddSeconds(9120).AddSeconds(bufferTime)}
            };

            var scheduleRepository = _repositoryHelper.RepositoryFactory.CreateRepositorySchedule();

            var scheduleIds = await scheduleRepository.AddSchedulesAsync(newSchedules);
            scheduleIds.Should().HaveCount(5);

            var filteredSchedules = await scheduleRepository.GetByTitlePriceAndStartTimeAsync("Salt", price, startTime);
            filteredSchedules.Should().HaveCount(1);
            filteredSchedules.ElementAt(0).Price.Should().Be(price);
            filteredSchedules.ElementAt(0).MovieId.Should().Be(1L);
        }

    }
}
