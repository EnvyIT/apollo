using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Apollo.Core.Dto;
using Apollo.Domain.Entity;
using Apollo.Util;
using Apollo.Util.Logger;
using FluentAssertions;
using Newtonsoft.Json;
using NUnit.Framework;
using static Apollo.Core.Dto.Mapper;

namespace Apollo.Api.Test
{
    public class ManageSchedulesTest : BaseApiTest
    {

        private static readonly IApolloLogger<ManageSchedulesTest> Logger =
            LoggerFactory.CreateLogger<ManageSchedulesTest>();

        private const string BaseUrl = "schedule";
        private const string EndpointDay = "day";
        private const string EndpointGenre = "genre";
        private const string EndpointTitle = "title";
        private const string EndpointLoadFactor = "loadfactor";
        private const string EndpointSeatLayout = "seatlayout";

        [Test]
        public async Task GetSchedulesByDay_ShouldReturnFilteredSchedules()
        {

            Logger.Info("Load schedules by day ...");
            var day = DataSeeder.Schedules.First().StartTime;
            var endpointParams = new List<KeyValuePair<string, string>>
                {
                    new KeyValuePair<string, string>(null,  $"{day:u}")
                };

            var response = await Get(BaseUrl, EndpointDay, endpointParams);
            await CheckResponseAndLogStatusCodeAsync(Logger, response);
          
            var schedules = JsonConvert.DeserializeObject<IEnumerable<ScheduleDto>>(await response.Content.ReadAsStringAsync()).ToList();
            schedules.Should().HaveCount(DataSeeder.Schedules.Count(s => s.StartTime.IsSameDay(day)));
            schedules.All(s => s.StartTime.IsSameDay(day)).Should().BeTrue();
            Logger.Info(schedules.ToPrettyString());

        }

        [Test]
        public async Task GetSchedulesByGenres_ShouldReturnFilteredSchedules()
        {

            Logger.Info("Load schedules by day and genre ...");
            var day = DataSeeder.Schedules.First().StartTime;
            var genre = DataSeeder.Genres.Skip(6).First(); //Adventure
            var endpointParams = new List<KeyValuePair<string, string>>
                {
                    new KeyValuePair<string, string>(null,  $"{day:u}"),
                    new KeyValuePair<string, string>(EndpointGenre,  genre.Name),
                };

            var response = await Get(BaseUrl, endpointParams);
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            await CheckResponseAndLogStatusCodeAsync(Logger, response);

            var schedules = JsonConvert.DeserializeObject<IEnumerable<ScheduleDto>>(await response.Content.ReadAsStringAsync()).ToList();
            schedules.Should().HaveCount(DataSeeder.Schedules.Count(s => s.StartTime.IsSameDay(day) && s.MovieId == 3L));
            schedules.All(s => s.StartTime.IsSameDay(day)).Should().BeTrue();
            Logger.Info(schedules.ToPrettyString());

        }

        [Test]
        public async Task GetSchedulesByTitle_ShouldReturnFilteredSchedules()
        {
            Logger.Info("Load schedules by day and title ...");
            var day = DataSeeder.Schedules.First().StartTime;
            var movie = DataSeeder.Movies.Skip(1).First(); //Movie with title 'The Gold Rush'
            var endpointParams = new List<KeyValuePair<string, string>>
                {
                    new KeyValuePair<string, string>(null,  $"{day:u}"),
                    new KeyValuePair<string, string>(EndpointTitle,  movie.Title),
                };

            var response = await Get(BaseUrl, endpointParams);
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            await CheckResponseAndLogStatusCodeAsync(Logger, response);

            var schedules = JsonConvert.DeserializeObject<IEnumerable<ScheduleDto>>(await response.Content.ReadAsStringAsync()).ToList();
            schedules.Should().HaveCount(DataSeeder.Schedules.Count(s => s.StartTime.IsSameDay(day) && s.MovieId == movie.Id));
            schedules.All(s => s.StartTime.IsSameDay(day)).Should().BeTrue();
            Logger.Info(schedules.ToPrettyString());

        }

        [Test]
        public async Task GetScheduleById_ShouldReturnSchedule()
        {

            Logger.Info("Load schedule by id ...");

            const long id = 7L;
            var schedule = DataSeeder.Schedules.Single(s => s.Id == id);

            var endpointParams = new List<KeyValuePair<string, string>>
                {
                    new KeyValuePair<string, string>(null,  $"{schedule.Id}"),
                };

            var response = await Get(BaseUrl, endpointParams);
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            await CheckResponseAndLogStatusCodeAsync(Logger, response);

            var scheduleDto = JsonConvert.DeserializeObject<ScheduleDto>(await response.Content.ReadAsStringAsync());
            scheduleDto.Should().NotBeNull();
            scheduleDto.Id.Should().Be(schedule.Id);
            scheduleDto.CinemaHallId.Should().Be(schedule.CinemaHallId);
            scheduleDto.MovieId.Should().Be(schedule.MovieId);
            scheduleDto.StartTime.Should().Be(schedule.StartTime);
            scheduleDto.Price.Should().Be(schedule.Price);
            Logger.Info(scheduleDto.ToPrettyString());
        }

        [Test]
        public async Task GetLoadFactorByScheduleId_ShouldReturnFilteredSchedules()
        {

            Logger.Info("Load load factor  of schedule by id ...");
            var schedule = DataSeeder.Schedules.Single(s => s.Id == 15L);
            var endpointParams = new List<KeyValuePair<string, string>>
                {
                    new KeyValuePair<string, string>(null,  $"{schedule.Id}"),
                };
            var response = await Get(BaseUrl, EndpointLoadFactor, endpointParams);
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            await CheckResponseAndLogStatusCodeAsync(Logger, response);

            var loadFactor = JsonConvert.DeserializeObject<double>(await response.Content.ReadAsStringAsync());
            loadFactor.Should().Be(0.0);
            Logger.Info(loadFactor.ToPrettyString());
        }

        [Test]
        public async Task GetSeatLayoutById_ShouldReturnFullLayout()
        {
            Logger.Info("Load cinema layout by schedule id ...");
            var schedule = DataSeeder.Schedules.Single(s => s.Id == 21L);
            var cinemaHall = DataSeeder.CinemaHalls.Single(c => c.Id == schedule.CinemaHallId);

            var endpointParams = new List<KeyValuePair<string, string>>
                {
                    new KeyValuePair<string, string>(null,  $"{schedule.Id}"),
                };

            var response = await Get(BaseUrl, EndpointSeatLayout, endpointParams);
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            await CheckResponseAndLogStatusCodeAsync(Logger, response);

            var seatLayout = JsonConvert.DeserializeObject<IList<SeatDto>>(await response.Content.ReadAsStringAsync());

            var seatsInLayout = GetExpectedLayoutSeats(seatLayout, cinemaHall);
            seatLayout.Should().HaveCount(seatsInLayout);
            Logger.Info(seatLayout.ToPrettyString());
        }

        private static int GetExpectedLayoutSeats(IList<SeatDto> seatLayout, CinemaHall cinemaHall)
        {
            var seatsInLayout = 0;
            for (var row = 0; row < cinemaHall.SizeRow; ++row)
            {
                for (var column = 0; column < cinemaHall.SizeColumn; ++column)
                {
                    var seat = seatLayout.SingleOrDefault(s => s.LayoutColumn == column && s.LayoutRow == row);
                    if (seat != null)
                    {
                        ++seatsInLayout;
                    }
                }
            }

            return seatsInLayout;
        }

        [Test]
        public async Task ManageSchedule_ValidWorkflow()
        {

            Logger.Info("Admin login to perform crud operations on schedules...");
            await Authenticate();

            Logger.Info("Load all schedules by certain day...");
            var day = DataSeeder.Schedules.First().StartTime.AddDays(1);
            var schedulesCount = DataSeeder.Schedules.Count(s => s.StartTime.IsSameDay(day));
            var endpointParams = new List<KeyValuePair<string, string>>
            {
                new KeyValuePair<string, string>(null, $"{day:u}")
            };
            //Get schedules
            var response = await Get(BaseUrl, EndpointDay, endpointParams);
            response.StatusCode.Should().Be(HttpStatusCode.OK);

            var schedules = JsonConvert.DeserializeObject<List<ScheduleDto>>(await response.Content.ReadAsStringAsync());
            schedules.Should().HaveCount(schedulesCount);
            schedules.All(s => s.StartTime.IsSameDay(day)).Should().BeTrue();
            Logger.Info(schedules.ToPrettyString());

            Logger.Info("Create a new schedule...");
            //Create schedule
            var utcNow = DateTime.UtcNow;
            var startTime = new DateTime(utcNow.Year, utcNow.Month, utcNow.Day, 12, 30, 0).AddDays(1);
            var newSchedule = new ScheduleDto
            {
                StartTime = startTime,
                MovieId = 4L,
                CinemaHallId = 1L,
                Price = 9.99M
            };
            response = await Post(BaseUrl, newSchedule);
            await CheckResponseAndLogStatusCodeAsync(Logger, response, HttpStatusCode.Created);
            var scheduleId = await GetCreatedIdAsync(response);

            //Get all schedules and verify that schedule is added
            endpointParams = new List<KeyValuePair<string, string>>
            {
                new KeyValuePair<string, string>(null, $"{day:u}")
            };
            response = await Get(BaseUrl, EndpointDay, endpointParams);
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            schedules = JsonConvert.DeserializeObject<List<ScheduleDto>>(await response.Content.ReadAsStringAsync());
            schedules.Should().HaveCount(schedulesCount + 1);
            Logger.Info(schedules.ToPrettyString());

            //Get newly created schedule
            Logger.Info("Load new schedule...");
            endpointParams = new List<KeyValuePair<string, string>>
            {
                new KeyValuePair<string, string>(null, $"{scheduleId}"),
            };
            response = await Get(BaseUrl, endpointParams);
            await CheckResponseAndLogStatusCodeAsync(Logger, response);
            var createdSchedule = JsonConvert.DeserializeObject<ScheduleDto>(await response.Content.ReadAsStringAsync());

            createdSchedule.CinemaHallId.Should().NotBe(0L);
            ValidateDateTimes(createdSchedule.StartTime, newSchedule.StartTime);
            createdSchedule.MovieId.Should().Be(newSchedule.MovieId);
            createdSchedule.CinemaHallId.Should().Be(newSchedule.CinemaHallId);
            createdSchedule.Price.Should().Be(newSchedule.Price);
            Logger.Info(createdSchedule.ToPrettyString());

            //Update schedule
            Logger.Info("Update new schedules price...");
            createdSchedule.Price = 18.5M;
            response = await Put(BaseUrl, createdSchedule);
            await CheckResponseAndLogStatusCodeAsync(Logger, response, HttpStatusCode.NoContent);

            //Get updated schedule
            Logger.Info("Get updated schedule...");
            endpointParams = new List<KeyValuePair<string, string>>
            {
                new KeyValuePair<string, string>(null, $"{createdSchedule.Id}"),
            };

            response = await Get(BaseUrl, endpointParams);
            await CheckResponseAndLogStatusCodeAsync(Logger, response);
            var updatedSchedule = JsonConvert.DeserializeObject<ScheduleDto>(await response.Content.ReadAsStringAsync());
            updatedSchedule.Price.Should().Be(createdSchedule.Price);
            Logger.Info(updatedSchedule.ToPrettyString());

            //Delete schedule
            Logger.Info("Delete created/updated schedule...");
            response = await DeleteSchedule(updatedSchedule, $"Delete schedule with id {updatedSchedule.Id} ...");
            await CheckResponseAndLogStatusCodeAsync(Logger, response, HttpStatusCode.NoContent);

            //Get schedules
            endpointParams = new List<KeyValuePair<string, string>>
            {
                new KeyValuePair<string, string>(null, $"{day:u}")
            };
            response = await Get(BaseUrl, EndpointDay, endpointParams);
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            schedules = JsonConvert.DeserializeObject<List<ScheduleDto>>(await response.Content.ReadAsStringAsync());
            schedules.Should().HaveCount(schedulesCount);
            schedules.All(s => s.StartTime.IsSameDay(day)).Should().BeTrue();
            schedules.Any(s => s.Id == updatedSchedule.Id).Should().BeFalse();
            Logger.Info(schedules.ToPrettyString());
        }

        [Test]
        public async Task ManageSchedule_Unauthorized()
        {
            //Delete schedule unauthorized
            var schedules = DataSeeder.Schedules.ToList();
            var scheduleForDeletion = schedules.Select(Map).Single(s => s.Id == 1L);
            var response = await DeleteSchedule(scheduleForDeletion, $"Try delete schedule with id {scheduleForDeletion.Id} as unauthorized user...");
            response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);

            //Create schedule unauthorized
            var utcNow = DateTime.UtcNow;
            var startTime = new DateTime(utcNow.Year, utcNow.Month, utcNow.Day, 12, 30, 0).AddDays(1);
            var newSchedule = new ScheduleDto
            {
                StartTime = startTime,
                MovieId = 4L,
                CinemaHallId = 1L,
                Price = 9.99M
            };
            response = await Post(BaseUrl, newSchedule);
            response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);

            //Update schedule unauthorized
            response = await Put(BaseUrl, newSchedule);
            response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);

            //Delete schedule authenticated
            await Authenticate();
            Logger.Info($"Try delete schedule with id {-1} as authorized user...");
            response = await Delete(BaseUrl, $"{-1}");
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        }

        [Test]
        public async Task GetScheduleByDay_InvalidParameters()
        {
            var endpointParams = new List<KeyValuePair<string, string>>
            {
                new KeyValuePair<string, string>(EndpointDay,  $"{null}")
            };

            var response = await Get(BaseUrl, EndpointDay, endpointParams);
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);

            endpointParams = new List<KeyValuePair<string, string>>
            {
                new KeyValuePair<string, string>(EndpointDay,  $"{new DateTime(1200, 1, 1)}")
            };
            response = await Get(BaseUrl, EndpointDay, endpointParams);
            response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }

        [Test]
        public async Task GetScheduleByGenre_InvalidParameters()
        {
            var day = DataSeeder.Schedules.First().StartTime;
            var endpointParams = new List<KeyValuePair<string, string>>
            {
                new KeyValuePair<string, string>(EndpointDay,  $"{day:u}"),
                new KeyValuePair<string, string>(EndpointGenre, null),
            };

            var response = await Get(BaseUrl, EndpointGenre, endpointParams);
            response.StatusCode.Should().Be(HttpStatusCode.NotFound);

            endpointParams = new List<KeyValuePair<string, string>>
            {
                new KeyValuePair<string, string>(EndpointDay,  null),
                new KeyValuePair<string, string>(EndpointGenre, "Horror"),
            };
            response = await Get(BaseUrl, EndpointGenre, endpointParams);
            response.StatusCode.Should().Be(HttpStatusCode.NotFound);


            endpointParams = new List<KeyValuePair<string, string>>
            {
                new KeyValuePair<string, string>(EndpointDay,  null),
                new KeyValuePair<string, string>(EndpointGenre, null),
            };
            response = await Get(BaseUrl, EndpointGenre, endpointParams);
            response.StatusCode.Should().Be(HttpStatusCode.NotFound);

            endpointParams = new List<KeyValuePair<string, string>>
            {
                new KeyValuePair<string, string>(EndpointDay,  $"{new DateTime(1600, 1, 1)}"),
                new KeyValuePair<string, string>(EndpointGenre, null),
            };
            response = await Get(BaseUrl, EndpointGenre, endpointParams);
            response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }

        [Test]
        public async Task GetScheduleByTitle_InvalidParameters()
        {
            var endpointParams = new List<KeyValuePair<string, string>>
            {
                new KeyValuePair<string, string>(EndpointDay,  null),
                new KeyValuePair<string, string>(EndpointTitle,  "B"),
            };

            var response = await Get(BaseUrl, EndpointTitle, endpointParams);
            response.StatusCode.Should().Be(HttpStatusCode.NotFound);

            endpointParams = new List<KeyValuePair<string, string>>
            {
                new KeyValuePair<string, string>(EndpointDay,  $"{new DateTime(1600, 1, 1)}"),
                new KeyValuePair<string, string>(EndpointTitle,  null),
            };

            response = await Get(BaseUrl, EndpointTitle, endpointParams);
            response.StatusCode.Should().Be(HttpStatusCode.NotFound);

            endpointParams = new List<KeyValuePair<string, string>>
            {
                new KeyValuePair<string, string>(EndpointDay,  null),
                new KeyValuePair<string, string>(EndpointTitle,  null),
            };

            response = await Get(BaseUrl, EndpointTitle, endpointParams);
            response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }

        [Test]
        public async Task GetScheduleById_InvalidParameters()
        {
            var endpointParams = new List<KeyValuePair<string, string>>
            {
                new KeyValuePair<string, string>(null,  $"{-99}"),
            };

            var response = await Get(BaseUrl, endpointParams);
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);

            endpointParams = new List<KeyValuePair<string, string>>
            {
                new KeyValuePair<string, string>(null,  $"{long.MaxValue}"),
            };

            response = await Get(BaseUrl, endpointParams);
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        }

        [Test]
        public async Task GetSeatLayout_InvalidRequests()
        {
            var endpointParams = new List<KeyValuePair<string, string>>
            {
                new KeyValuePair<string, string>(null,  $"{-99L}"),
            };
            var response = await Get(BaseUrl, EndpointSeatLayout, endpointParams);
            await CheckResponseAndLogStatusCodeAsync(Logger, response, HttpStatusCode.BadRequest);
        }


        [Test]
        public async Task PostSchedule_InvalidRequests()
        {
            await Authenticate();
            var utcNow = DateTime.UtcNow;
            var startTime = new DateTime(utcNow.Year, utcNow.Month, utcNow.Day, 12, 30, 0).AddDays(1);
            //Required MovieId
            var newSchedule = new ScheduleDto
            {
                StartTime = startTime,
                CinemaHallId = 1L,
                Price = 9.99M
            };
            var response = await Post(BaseUrl, newSchedule);
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
            await CheckResponseAndLogStatusCodeAsync(Logger, response, HttpStatusCode.BadRequest);
            
            //Required CinemaHallId
            newSchedule = new ScheduleDto
            {
                MovieId = 3L,
                StartTime = startTime,
                Price = 9.99M
            };
            response = await Post(BaseUrl, newSchedule);
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
            await CheckResponseAndLogStatusCodeAsync(Logger, response, HttpStatusCode.BadRequest);

            //Wrong CinemaHallId range
            newSchedule = new ScheduleDto
            {
                CinemaHallId = -99L,
                MovieId = 3L,
                StartTime = startTime,
                Price = 9.99M
            };
            response = await Post(BaseUrl, newSchedule);
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
            await CheckResponseAndLogStatusCodeAsync(Logger, response, HttpStatusCode.BadRequest);

            //Wrong MovieId range
            newSchedule = new ScheduleDto
            {
                CinemaHallId = 1L,
                MovieId = -3L,
                StartTime = startTime,
                Price = 9.99M
            };
            response = await Post(BaseUrl, newSchedule);
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
            await CheckResponseAndLogStatusCodeAsync(Logger, response, HttpStatusCode.BadRequest);


            //CinemaHallId does not match CinemaHall.Id 
            newSchedule = new ScheduleDto
            {
                CinemaHallId = 1L,
                CinemaHall = new CinemaHallDto { Id = 3L, Label= "Another Hall", SizeRow = 10, SizeColumn = 5},
                MovieId = -3L,
                StartTime = startTime,
                Price = 9.99M
            };
            response = await Post(BaseUrl, newSchedule);
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
            await CheckResponseAndLogStatusCodeAsync(Logger, response, HttpStatusCode.BadRequest);

            //Price falls below 5.0M
            newSchedule = new ScheduleDto
            {
                CinemaHallId = 1L,
                CinemaHall = new CinemaHallDto { Id = 1L, Label = "Another Hall", SizeRow = 10, SizeColumn = 5 },
                MovieId = 3L,
                StartTime = startTime,
                Price = 4.99M
            };
            response = await Post(BaseUrl, newSchedule);
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
            await CheckResponseAndLogStatusCodeAsync(Logger, response, HttpStatusCode.BadRequest);


            //Price exceeds limit of 50.0M
            newSchedule = new ScheduleDto
            {
                CinemaHallId = 1L,
                CinemaHall = new CinemaHallDto { Id = 1L, Label = "Another Hall", SizeRow = 10, SizeColumn = 5 },
                MovieId = 3L,
                StartTime = startTime,
                Price = 50.01M
            };
            response = await Post(BaseUrl, newSchedule);
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
            await CheckResponseAndLogStatusCodeAsync(Logger, response, HttpStatusCode.BadRequest);

            //Starttime is not 4 hours in future
            newSchedule = new ScheduleDto
            {
                CinemaHallId = 1L,
                CinemaHall = new CinemaHallDto { Id = 1L, Label = "Another Hall", SizeRow = 10, SizeColumn = 5 },
                MovieId = 3L,
                StartTime = DateTime.UtcNow.AddHours(3).AddMinutes(59).AddSeconds(59),
                Price = 14.25M
            };
            response = await Post(BaseUrl, newSchedule);
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
            await CheckResponseAndLogStatusCodeAsync(Logger, response, HttpStatusCode.BadRequest);

            //Starttime is not 4 hours in future
            newSchedule = new ScheduleDto
            {
                CinemaHallId = 1L,
                CinemaHall = new CinemaHallDto { Id = 1L, Label = "Another Hall", SizeRow = 10, SizeColumn = 5 },
                MovieId = 3L,
                StartTime = DateTime.UtcNow.AddHours(3).AddMinutes(59).AddSeconds(59),
                Price = 14.25M
            };
            response = await Post(BaseUrl, newSchedule);
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
            await CheckResponseAndLogStatusCodeAsync(Logger, response, HttpStatusCode.BadRequest);

            //Empty request body
            response = await Post(BaseUrl, ((ScheduleDto)null));
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
            await CheckResponseAndLogStatusCodeAsync(Logger, response, HttpStatusCode.BadRequest);
        }

        [Test]
        public async Task DeleteSchedule_InvalidRequests()
        {
            await Authenticate();

            //Schedule contains references on reservations
            var schedule = new ScheduleDto
            {
                Id = 1L
            };
            var response = await DeleteSchedule(schedule);
            await CheckResponseAndLogStatusCodeAsync(Logger, response, HttpStatusCode.BadRequest);

            //Already deleted schedule
            schedule = DataSeeder.Schedules.Select(Map).Last();
            response = await DeleteSchedule(schedule);
            await CheckResponseAndLogStatusCodeAsync(Logger, response, HttpStatusCode.BadRequest);
        }


        [Test]
        public async Task UpdateSchedule_InvalidRequests()
        {
            await Authenticate();

            //Schedule contains reference on reservation
            var schedule = new ScheduleDto
            {
                Id = 1L,
                StartTime = DateTime.UtcNow.AddDays(4),
                MovieId = 6L,
                CinemaHallId = 1L,
                Price = 8.13M
            };

            var response = await Put(BaseUrl, schedule);
            await CheckResponseAndLogStatusCodeAsync(Logger, response, HttpStatusCode.BadRequest);

            //DateTime is not valid for update
            schedule = new ScheduleDto
            {
                Id = 2L,
                StartTime = DateTime.UtcNow,
                MovieId = 6L,
                CinemaHallId = 1L,
                Price = 8.13M

            };
            response = await Put(BaseUrl, schedule);
            await CheckResponseAndLogStatusCodeAsync(Logger, response, HttpStatusCode.BadRequest);

            //Schedule is already deleted
            schedule = new ScheduleDto
            {
                Id = 26L,
                StartTime = DateTime.UtcNow.AddDays(4),
                MovieId = 6L,
                CinemaHallId = 1L,
                Price = 8.13M

            };
            response = await Put(BaseUrl, schedule);
            await CheckResponseAndLogStatusCodeAsync(Logger, response, HttpStatusCode.BadRequest);

        }


        private async Task<HttpResponseMessage> DeleteSchedule(BaseDto scheduleForDeletion, string message = null)
        {
            if (message != null)
            {
                Logger.Info(message);
            }
            return await Delete(BaseUrl, $"{scheduleForDeletion.Id}");
        }
    }
}
