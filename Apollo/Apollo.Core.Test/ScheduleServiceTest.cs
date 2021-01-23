using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Apollo.Core.Interfaces;
using Apollo.Domain.Entity;
using FluentAssertions;
using Moq;
using NUnit.Framework;
using static Apollo.Core.Dto.Mapper;

namespace Apollo.Core.Test
{
    public class ScheduleServiceTest
    {
        private MockingHelper _mockingHelper;
        private IScheduleService _scheduleService;

        [OneTimeSetUp]
        public void OneTimeSetup()
        {
            _mockingHelper = new MockingHelper();
            _scheduleService = _mockingHelper.ServiceFactory.Object.CreateScheduleService();
        }

        [Test]
        public async Task AddSchedulesForMoviesAsync_ShouldReturnMappedSchedule()
        {
            var scheduleDto = CreateSchedules().Select(Map).Single(m => m.Id == 1L);
            var schedule = Map(scheduleDto);
            _mockingHelper.RepositorySchedule
                .Setup(_ => _.AddScheduleAsync(It.IsAny<Schedule>()))
                .ReturnsAsync(1L);


            var result = await _scheduleService.AddScheduleAsync(scheduleDto);
            result.Should().BeEquivalentTo(scheduleDto);
        }


        [Test]
        public async Task GetByTitleAndStartTimeAsync_ShouldReturnSchedules()
        {
            //GetCapacity mock to fill load factor 
            _mockingHelper.RepositoryTicket
                .Setup(_ => _.GetReservationsAsync())
                .ReturnsAsync(CreateReservations);

            _mockingHelper.RepositorySchedule
                .Setup(_ => _.GetScheduleByIdAsync(It.IsAny<long>()))
                .ReturnsAsync(CreateSchedules().First());

            _mockingHelper.RepositoryInfrastructure
                .Setup(_ => _.GetSeatsWithRowAndCategoryAsync(It.IsAny<long>()))
                .ReturnsAsync(CreateSeats());
            //GetByTitleMock
            _mockingHelper.RepositorySchedule
                .Setup(_ => _.GetByTitleAndStartTimeAsync(It.IsAny<string>(), It.IsAny<DateTime>()))
                .ReturnsAsync(CreateSchedules);


            var result = (await _scheduleService.GetByTitleAndStartTimeAsync("Salt", DateTime.Now)).ToList();
            result.Should().HaveCount(6);
        }

        [Test]
        public async Task GetScheduleByIdAsync_ShouldReturnMappedSchedule()
        {

            //GetCapacity mock to fill load factor 
            _mockingHelper.RepositoryTicket
                .Setup(_ => _.GetReservationsAsync())
                .ReturnsAsync(CreateReservations);

            _mockingHelper.RepositorySchedule
                .Setup(_ => _.GetScheduleByIdAsync(It.IsAny<long>()))
                .ReturnsAsync(CreateSchedules().First());

            _mockingHelper.RepositoryInfrastructure
                .Setup(_ => _.GetSeatsWithRowAndCategoryAsync(It.IsAny<long>()))
                .ReturnsAsync(CreateSeats());

            var schedule = CreateSchedules().Single(m => m.Id == 1L);
            _mockingHelper.RepositorySchedule
                .Setup(_ => _.GetScheduleByIdAsync(It.IsAny<long>()))
                .ReturnsAsync(schedule);


            var result = await _scheduleService.GetScheduleByIdAsync(1L);
            result.Should().NotBeNull();
            result.StartTime.Should().Be(new DateTime(2020, 11, 4, 20, 15, 00));
            result.CinemaHall.Should().NotBeNull();
            result.CinemaHallId.Should().Be(1L);
            result.CinemaHall.Label.Should().Be("Apollo 1");
            result.Movie.Should().NotBeNull();
            result.Movie.Title.Should().Be("Salt");
            result.MovieId.Should().Be(1L);
            result.Price.Should().Be(12.5M);
        }

        [Test]
        public async Task GetSchedulesAsync_ShouldReturnMappedSchedule()
        {

            //GetCapacity mock to fill load factor 
            _mockingHelper.RepositoryTicket
                .Setup(_ => _.GetReservationsAsync())
                .ReturnsAsync(CreateReservations);

            _mockingHelper.RepositorySchedule
                .Setup(_ => _.GetScheduleByIdAsync(It.IsAny<long>()))
                .ReturnsAsync(CreateSchedules().First());

            _mockingHelper.RepositoryInfrastructure
                .Setup(_ => _.GetSeatsWithRowAndCategoryAsync(It.IsAny<long>()))
                .ReturnsAsync(CreateSeats());

            _mockingHelper.RepositorySchedule
                .Setup(_ => _.GetSchedulesAsync())
                .ReturnsAsync(CreateSchedules);


            var result = (await _scheduleService.GetSchedulesAsync()).ToList();
            result.Should().HaveCount(6);
            result.ElementAt(0).Id.Should().Be(1L);
            result.ElementAt(1).Id.Should().Be(2L);
            result.ElementAt(2).Id.Should().Be(3L);
            result.ElementAt(3).Id.Should().Be(4L);
            result.ElementAt(4).Id.Should().Be(5L);
            result.ElementAt(5).Id.Should().Be(6L);
        }

        [Test]
        public async Task DeleteScheduleAsync_ShouldReturnTrue()
        {

            _mockingHelper.RepositorySchedule
                .Setup(_ => _.DeleteScheduleAsync(It.IsAny<Schedule>()))
                .ReturnsAsync(1);

            _mockingHelper.RepositorySchedule
                .Setup(_ => _.IdExistAsync(It.IsAny<long>()))
                .ReturnsAsync(true);

            var scheduleDto = CreateSchedules().Select(Map).First();
            var result = (await _scheduleService.DeleteScheduleAsync(scheduleDto));
            result.Should().BeTrue();
        }

        [Test]
        public async Task AddSchedulesAsync_ShouldReturnSchedules()
        {

            _mockingHelper.RepositorySchedule
                .Setup(_ => _.AddSchedulesAsync(It.IsAny<IEnumerable<Schedule>>()))
                .ReturnsAsync(new List<long>{1L ,2L, 3L, 4L, 5L, 6L});

            var schedules = CreateSchedules().Select(Map);
            var result = (await _scheduleService.AddSchedulesAsync(schedules)).ToList();
            result.Should().HaveCount(6);
        }


        [Test]
        public async Task GetByMovieTitleAsync_ShouldReturnSchedules()
        {
            //GetCapacity mock to fill load factor 
            _mockingHelper.RepositoryTicket
                .Setup(_ => _.GetReservationsAsync())
                .ReturnsAsync(CreateReservations);

            _mockingHelper.RepositorySchedule
                .Setup(_ => _.GetScheduleByIdAsync(It.IsAny<long>()))
                .ReturnsAsync(CreateSchedules().First());

            _mockingHelper.RepositoryInfrastructure
                .Setup(_ => _.GetSeatsWithRowAndCategoryAsync(It.IsAny<long>()))
                .ReturnsAsync(CreateSeats());

            _mockingHelper.RepositorySchedule
                .Setup(_ => _.GetByMovieTitleAsync(It.IsAny<string>()))
                .ReturnsAsync(CreateSchedules);


            var result = (await _scheduleService.GetByMovieTitleAsync("Salt")).ToList();
            result.Should().HaveCount(6);
        }

        [Test]
        public async Task GetByPriceAsync_ShouldReturnSchedules()
        {
            //GetCapacity mock to fill load factor 
            _mockingHelper.RepositoryTicket
                .Setup(_ => _.GetReservationsAsync())
                .ReturnsAsync(CreateReservations);

            _mockingHelper.RepositorySchedule
                .Setup(_ => _.GetScheduleByIdAsync(It.IsAny<long>()))
                .ReturnsAsync(CreateSchedules().First());

            _mockingHelper.RepositoryInfrastructure
                .Setup(_ => _.GetSeatsWithRowAndCategoryAsync(It.IsAny<long>()))
                .ReturnsAsync(CreateSeats());

            _mockingHelper.RepositorySchedule
                .Setup(_ => _.GetByPriceAsync(It.IsAny<decimal>()))
                .ReturnsAsync(CreateSchedules);


            var result = (await _scheduleService.GetByPriceAsync(12.5M)).ToList();
            result.Should().HaveCount(6);
        }


        [Test]
        public async Task GetStartTimeAsync_ShouldReturnSchedules()
        {
            //GetCapacity mock to fill load factor 
            _mockingHelper.RepositoryTicket
                .Setup(_ => _.GetReservationsAsync())
                .ReturnsAsync(CreateReservations);

            _mockingHelper.RepositorySchedule
                .Setup(_ => _.GetScheduleByIdAsync(It.IsAny<long>()))
                .ReturnsAsync(CreateSchedules().First());

            _mockingHelper.RepositoryInfrastructure
                .Setup(_ => _.GetSeatsWithRowAndCategoryAsync(It.IsAny<long>()))
                .ReturnsAsync(CreateSeats());

            _mockingHelper.RepositorySchedule
                .Setup(_ => _.GetStartTimeAsync(It.IsAny<DateTime>()))
                .ReturnsAsync(CreateSchedules);


            var result = (await _scheduleService.GetStartTimeAsync(DateTime.UtcNow)).ToList();
            result.Should().HaveCount(6);
        }


        [Test]
        public async Task GetInTimeRangeAsync_ShouldReturnSchedules()
        {

            //GetCapacity mock to fill load factor 
            _mockingHelper.RepositoryTicket
                .Setup(_ => _.GetReservationsAsync())
                .ReturnsAsync(CreateReservations);

            _mockingHelper.RepositorySchedule
                .Setup(_ => _.GetScheduleByIdAsync(It.IsAny<long>()))
                .ReturnsAsync(CreateSchedules().First());

            _mockingHelper.RepositoryInfrastructure
                .Setup(_ => _.GetSeatsWithRowAndCategoryAsync(It.IsAny<long>()))

                .ReturnsAsync(CreateSeats());
            _mockingHelper.RepositorySchedule
                .Setup(_ => _.GetInTimeRangeAsync(It.IsAny<DateTime>(), It.IsAny<DateTime>()))
                .ReturnsAsync(CreateSchedules);


            var result = (await _scheduleService.GetInTimeRangeAsync(DateTime.UtcNow, DateTime.UtcNow.AddDays(2))).ToList();
            result.Should().HaveCount(6);
        }

        [Test]
        public async Task GetByTitleAndPriceAsync_ShouldReturnSchedules()
        {
            //GetCapacity mock to fill load factor 
            _mockingHelper.RepositoryTicket
                .Setup(_ => _.GetReservationsAsync())
                .ReturnsAsync(CreateReservations);

            _mockingHelper.RepositorySchedule
                .Setup(_ => _.GetScheduleByIdAsync(It.IsAny<long>()))
                .ReturnsAsync(CreateSchedules().First());

            _mockingHelper.RepositoryInfrastructure
                .Setup(_ => _.GetSeatsWithRowAndCategoryAsync(It.IsAny<long>()))
                .ReturnsAsync(CreateSeats());

            _mockingHelper.RepositorySchedule
                .Setup(_ => _.GetByTitleAndPriceAsync(It.IsAny<string>(), It.IsAny<decimal>()))
                .ReturnsAsync(CreateSchedules);


            var result = (await _scheduleService.GetByTitleAndPriceAsync("Salt", 12.50M)).ToList();
            result.Should().HaveCount(6);
        }

        [Test]
        public async Task GetByPriceAndStartTimeAsync_ShouldReturnSchedules()
        {
            //GetCapacity mock to fill load factor 
            _mockingHelper.RepositoryTicket
                .Setup(_ => _.GetReservationsAsync())
                .ReturnsAsync(CreateReservations);

            _mockingHelper.RepositorySchedule
                .Setup(_ => _.GetScheduleByIdAsync(It.IsAny<long>()))
                .ReturnsAsync(CreateSchedules().First());

            _mockingHelper.RepositoryInfrastructure
                .Setup(_ => _.GetSeatsWithRowAndCategoryAsync(It.IsAny<long>()))
                .ReturnsAsync(CreateSeats());

            _mockingHelper.RepositorySchedule
                .Setup(_ => _.GetByPriceAndStartTimeAsync(It.IsAny<decimal>(), It.IsAny<DateTime>()))
                .ReturnsAsync(CreateSchedules);


            var result = (await _scheduleService.GetByPriceAndStartTimeAsync(12.50M, DateTime.UtcNow)).ToList();
            result.Should().HaveCount(6);
        }



        [Test]
        public async Task GetByTitlePriceAndStartTimeAsync_ShouldReturnSchedules()
        {

            //GetCapacity mock to fill load factor 
            _mockingHelper.RepositoryTicket
                .Setup(_ => _.GetReservationsAsync())
                .ReturnsAsync(CreateReservations);

            _mockingHelper.RepositorySchedule
                .Setup(_ => _.GetScheduleByIdAsync(It.IsAny<long>()))
                .ReturnsAsync(CreateSchedules().First());

            _mockingHelper.RepositoryInfrastructure
                .Setup(_ => _.GetSeatsWithRowAndCategoryAsync(It.IsAny<long>()))
                .ReturnsAsync(CreateSeats());

            _mockingHelper.RepositorySchedule
                .Setup(_ => _.GetByTitlePriceAndStartTimeAsync(It.IsAny<string>(), It.IsAny<decimal>(), It.IsAny<DateTime>()))
                .ReturnsAsync(CreateSchedules);


            var result = (await _scheduleService.GetByTitlePriceAndStartTimeAsync("Salt", 12.5M, DateTime.Now)).ToList();
            result.Should().HaveCount(6);
        }


        [Test]
        public async Task GetCapacity_ShouldReturnSchedules()
        {
            _mockingHelper.RepositoryTicket
                .Setup(_ => _.GetReservationsAsync())
                .ReturnsAsync(CreateReservations());

            _mockingHelper.RepositorySchedule
                .Setup(_ => _.GetScheduleByIdAsync(It.IsAny<long>()))
                .ReturnsAsync(CreateSchedules().First());
            _mockingHelper.RepositorySchedule
                .Setup(_ => _.IdExistAsync(It.IsAny<long>()))
                .ReturnsAsync(true);

            var seats = CreateSeats().ToList();
            _mockingHelper.RepositoryInfrastructure
                .Setup(_ => _.GetSeatsWithRowAndCategoryAsync(It.IsAny<long>()))
                .ReturnsAsync(seats);

            _mockingHelper.RepositoryTicket
                .Setup(_ => _.GetSeatReservationsByIds(It.IsAny<IEnumerable<long>>()))
                .ReturnsAsync(seats.Where(seat => seat.Id % 2 != 0).Select(seat => new SeatReservation()));

            var result = await _scheduleService.GetCapacityAsync(1L);
            result.Should().BeApproximately(0.5, 0.001);
        }

        private static IEnumerable<Reservation> CreateReservations()
        {
            var startTime = new DateTime(2020, 11, 4, 20, 15, 00);
            return new List<Reservation>
            {
                new Reservation
                {
                    Id = 1L,
                    Schedule = new Schedule
                    {
                        Id = 1L,
                        StartTime = startTime,
                        CinemaHallId = 1L,
                        MovieId = 1L,
                        Price = 12.50M,
                        Movie = new Movie
                        {
                            Id = 1L,
                            Duration = 1200,
                            Description = "Pretty good movie...",
                            GenreId = 3L, Genre = new Genre
                            {
                                Id = 3L,
                                Name = "Action"
                            },
                            Title = "Salt",
                            Trailer = "youtube.com/salt",
                            Image = new byte[5]
                        },
                        CinemaHall = new CinemaHall
                        {
                            Id = 1L,
                            Label = "Apollo 1"
                        }
                    },
                    ScheduleId = 1L,
                    Ticket = new Ticket
                    {
                        Id = 1L,
                        Printed = DateTime.UtcNow
                    },
                    TicketId = 1L,
                    User = new User
                    {
                        Id = 1L,
                        Address = new Address
                        {
                            Id = 1L,
                            Number = 3,
                            City = new City()
                            {
                                Name = "Hagenberg",
                                PostalCode = "4232"
                            }
                        },
                        LastName = "TerminalUser",
                        FirstName = "TerminalUser",
                        AddressId = 1L,
                        Email = "apollo.office@gmail.com",
                        Phone = "04514/12351235",
                        Role = new Role
                        {
                            Id = 1L,
                            Label = "Terminal",
                            MaxReservations = 6 
                        },
                        RoleId = 1L
                    },
                    UserId = 1L
                },
                new Reservation
                {
                    Id = 2L,
                    Schedule = new Schedule
                    {
                        Id = 1L,
                        StartTime = startTime,
                        CinemaHallId = 1L,
                        MovieId = 1L,
                        Price = 12.50M,
                        Movie = new Movie
                        {
                            Id = 1L,
                            Duration = 1200,
                            Description = "Pretty good movie...",
                            GenreId = 3L, Genre = new Genre
                            {
                                Id = 3L,
                                Name = "Action"
                            },
                            Title = "Salt",
                            Trailer = "youtube.com/salt",
                            Image = new byte[5]
                        },
                        CinemaHall = new CinemaHall
                        {
                            Id = 1L,
                            Label = "Apollo 1"
                        }
                    },
                    ScheduleId = 1L,
                    Ticket = new Ticket
                    {
                        Id = 2L,
                        Printed = DateTime.UtcNow
                    },
                    TicketId = 2L,
                    User = new User
                    {
                        Id = 1L,
                        Address = new Address
                        {
                            Id = 1L,
                            Number = 3,
                            City = new City()
                            {
                                Name = "Hagenberg",
                                PostalCode = "4232"
                            }
                        },
                        LastName = "TerminalUser",
                        FirstName = "TerminalUser",
                        AddressId = 1L,
                        Email = "apollo.office@gmail.com",
                        Phone = "04514/12351235",
                        Role = new Role
                        {
                            Id = 1L,
                            Label = "Terminal",
                            MaxReservations = 6
                        },
                        RoleId = 1L
                    },
                    UserId = 1L
                },
                new Reservation
                {
                    Id = 3L,
                    Schedule = new Schedule
                    {
                        Id = 1L,
                        StartTime = startTime,
                        CinemaHallId = 1L,
                        MovieId = 1L,
                        Price = 12.50M,
                        Movie = new Movie
                        {
                            Id = 1L,
                            Duration = 1200,
                            Description = "Pretty good movie...",
                            GenreId = 3L, Genre = new Genre
                            {
                                Id = 3L,
                                Name = "Action"
                            },
                            Title = "Salt",
                            Trailer = "youtube.com/salt",
                            Image = new byte[5]
                        },
                        CinemaHall = new CinemaHall
                        {
                            Id = 1L,
                            Label = "Apollo 1"
                        }
                    },
                    ScheduleId = 1L,
                    Ticket = new Ticket
                    {
                        Id = 3L,
                        Printed = DateTime.UtcNow
                    },
                    TicketId = 3L,
                    User = new User
                    {
                        Id = 1L,
                        Address = new Address
                        {
                            Id = 1L,
                            Number = 3,
                            City = new City()
                            {
                                Name = "Hagenberg",
                                PostalCode = "4232"
                            }
                        },
                        LastName = "TerminalUser",
                        FirstName = "TerminalUser",
                        AddressId = 1L,
                        Email = "apollo.office@gmail.com",
                        Phone = "04514/12351235",
                        Role = new Role
                        {
                            Id = 1L,
                            Label = "Terminal",
                            MaxReservations = 6
                        },
                        RoleId = 1L
                    },
                    UserId = 1L
                },
                new Reservation
                {
                    Id = 4L,
                    Schedule = new Schedule
                    {
                        Id = 1L,
                        StartTime = startTime,
                        CinemaHallId = 1L,
                        MovieId = 1L,
                        Price = 12.50M,
                        Movie = new Movie
                        {
                            Id = 1L,
                            Duration = 1200,
                            Description = "Pretty good movie...",
                            GenreId = 3L, Genre = new Genre
                            {
                                Id = 3L,
                                Name = "Action"
                            },
                            Title = "Salt",
                            Trailer = "youtube.com/salt",
                            Image = new byte[5]
                        },
                        CinemaHall = new CinemaHall
                        {
                            Id = 1L,
                            Label = "Apollo 1"
                        }
                    },
                    ScheduleId = 1L,
                    Ticket = new Ticket
                    {
                        Id = 4L,
                        Printed = DateTime.UtcNow
                    },
                    TicketId = 4L,
                    User = new User
                    {
                        Id = 1L,
                        Address = new Address
                        {
                            Id = 1L,
                            Number = 3,
                            City = new City()
                            {
                                Name = "Hagenberg",
                                PostalCode = "4232"
                            }
                        },
                        LastName = "TerminalUser",
                        FirstName = "TerminalUser",
                        AddressId = 1L,
                        Email = "apollo.office@gmail.com",
                        Phone = "04514/12351235",
                        Role = new Role
                        {
                            Id = 1L,
                            Label = "Terminal",
                            MaxReservations = 6
                        },
                        RoleId = 1L
                    },
                    UserId = 1L
                },
                new Reservation
                {
                    Id = 5L,
                    Schedule = new Schedule
                    {
                        Id = 1L,
                        StartTime = startTime,
                        CinemaHallId = 1L,
                        MovieId = 1L,
                        Price = 12.50M,
                        Movie = new Movie
                        {
                            Id = 1L,
                            Duration = 1200,
                            Description = "Pretty good movie...",
                            GenreId = 3L, Genre = new Genre
                            {
                                Id = 3L,
                                Name = "Action"
                            },
                            Title = "Salt",
                            Trailer = "youtube.com/salt",
                            Image = new byte[5]
                        },
                        CinemaHall = new CinemaHall
                        {
                            Id = 1L,
                            Label = "Apollo 1"
                        }
                    },
                    ScheduleId = 1L,
                    Ticket = new Ticket
                    {
                        Id = 5L,
                        Printed = DateTime.UtcNow
                    },
                    TicketId = 5L,
                    User = new User
                    {
                        Id = 1L,
                        Address = new Address
                        {
                            Id = 1L,
                            Number = 3,
                            City = new City()
                            {
                                Name = "Hagenberg",
                                PostalCode = "4232"
                            }
                        },
                        LastName = "TerminalUser",
                        FirstName = "TerminalUser",
                        AddressId = 1L,
                        Email = "apollo.office@gmail.com",
                        Phone = "04514/12351235",
                        Role = new Role
                        {
                            Id = 1L,
                            Label = "Terminal",
                            MaxReservations = 6
                        },
                        RoleId = 1L
                    },
                    UserId = 1L
                },
                new Reservation
                {
                    Id = 6L,
                    Schedule = new Schedule
                    {
                        Id = 1L,
                        StartTime = startTime,
                        CinemaHallId = 1L,
                        MovieId = 1L,
                        Price = 12.50M,
                        Movie = new Movie
                        {
                            Id = 1L,
                            Duration = 1200,
                            Description = "Pretty good movie...",
                            GenreId = 3L, Genre = new Genre
                            {
                                Id = 3L,
                                Name = "Action"
                            },
                            Title = "Salt",
                            Trailer = "youtube.com/salt",
                            Image = new byte[5]
                        },
                        CinemaHall = new CinemaHall
                        {
                            Id = 1L,
                            Label = "Apollo 1"
                        }
                    },
                    ScheduleId = 1L,
                    Ticket = new Ticket
                    {
                        Id = 6L,
                        Printed = DateTime.UtcNow
                    },
                    TicketId = 6L,
                    User = new User
                    {
                        Id = 1L,
                        Address = new Address
                        {
                            Id = 1L,
                            Number = 3,
                            City = new City()
                            {
                                Name = "Hagenberg",
                                PostalCode = "4232"
                            }
                        },
                        LastName = "TerminalUser",
                        FirstName = "TerminalUser",
                        AddressId = 1L,
                        Email = "apollo.office@gmail.com",
                        Phone = "04514/12351235",
                        Role = new Role
                        {
                            Id = 1L,
                            Label = "Terminal",
                            MaxReservations = 6
                        },
                        RoleId = 1L
                    },
                    UserId = 1L
                },
                new Reservation
                {
                    Id = 7L,
                    Schedule = new Schedule
                    {
                        Id = 1L,
                        StartTime = startTime,
                        CinemaHallId = 1L,
                        MovieId = 1L,
                        Price = 12.50M,
                        Movie = new Movie
                        {
                            Id = 1L,
                            Duration = 1200,
                            Description = "Pretty good movie...",
                            GenreId = 3L, Genre = new Genre
                            {
                                Id = 3L,
                                Name = "Action"
                            },
                            Title = "Salt",
                            Trailer = "youtube.com/salt",
                            Image = new byte[5]
                        },
                        CinemaHall = new CinemaHall
                        {
                            Id = 1L,
                            Label = "Apollo 1"
                        }
                    },
                    ScheduleId = 1L,
                    Ticket = new Ticket
                    {
                        Id = 7L,
                        Printed = DateTime.UtcNow
                    },
                    TicketId = 7L,
                    User = new User
                    {
                        Id = 1L,
                        Address = new Address
                        {
                            Id = 1L,
                            Number = 3,
                            City = new City()
                            {
                                Name = "Hagenberg",
                                PostalCode = "4232"
                            }
                        },
                        LastName = "TerminalUser",
                        FirstName = "TerminalUser",
                        AddressId = 1L,
                        Email = "apollo.office@gmail.com",
                        Phone = "04514/12351235",
                        Role = new Role
                        {
                            Id = 1L,
                            Label = "Terminal",
                            MaxReservations = 6
                        },
                        RoleId = 1L
                    },
                    UserId = 1L
                },
                new Reservation
                {
                    Id = 8L,
                    Schedule = new Schedule
                    {
                        Id = 1L,
                        StartTime = startTime,
                        CinemaHallId = 1L,
                        MovieId = 1L,
                        Price = 12.50M,
                        Movie = new Movie
                        {
                            Id = 1L,
                            Duration = 1200,
                            Description = "Pretty good movie...",
                            GenreId = 3L, Genre = new Genre
                            {
                                Id = 3L,
                                Name = "Action"
                            },
                            Title = "Salt",
                            Trailer = "youtube.com/salt",
                            Image = new byte[5]
                        },
                        CinemaHall = new CinemaHall
                        {
                            Id = 1L,
                            Label = "Apollo 1"
                        }
                    },
                    ScheduleId = 1L,
                    Ticket = new Ticket
                    {
                        Id = 8L,
                        Printed = DateTime.UtcNow
                    },
                    TicketId = 8L,
                    User = new User
                    {
                        Id = 1L,
                        Address = new Address
                        {
                            Id = 1L,
                            Number = 3,
                            City = new City()
                            {
                                Name = "Hagenberg",
                                PostalCode = "4232"
                            }
                        },
                        LastName = "TerminalUser",
                        FirstName = "TerminalUser",
                        AddressId = 1L,
                        Email = "apollo.office@gmail.com",
                        Phone = "04514/12351235",
                        Role = new Role
                        {
                            Id = 1L,
                            Label = "Terminal",
                            MaxReservations = 6
                        },
                        RoleId = 1L
                    },
                    UserId = 1L
                },
            };
        }

        private static IEnumerable<Schedule> CreateSchedules()
        {
            var startTime = new DateTime(2020, 11, 4, 20, 15, 00);
            var secondStartTime = new DateTime(2020, 11, 5, 19, 35, 00);
            var thirdStartTime = new DateTime(2020, 11, 4, 23, 15, 00);
            return new List<Schedule>
            {
                new Schedule
                {
                    Id = 1L,
                    StartTime = startTime,
                    CinemaHallId = 1L,
                    MovieId = 1L,
                    Price = 12.50M,
                    Movie = new Movie
                    {
                        Id = 1L,
                        Duration = 1200,
                        Description = "Pretty good movie...",
                        GenreId = 3L, Genre = new Genre
                        {
                            Id = 3L,
                            Name = "Action"
                        },
                        Title = "Salt",
                        Trailer = "youtube.com/salt",
                        Image = new byte[5]
                    },
                    CinemaHall = new CinemaHall
                    {
                        Id = 1L,
                        Label = "Apollo 1"
                    }
                },
                new Schedule
                {
                    Id = 2L,
                    StartTime = startTime,
                    CinemaHallId = 2L,
                    MovieId = 1L,
                    Price = 12.50M,
                    Movie = new Movie
                    {
                        Id = 1L,
                        Duration = 1200,
                        Description = "Pretty good movie...",
                        GenreId = 3L,
                        Genre = new Genre
                        {
                            Id = 3L,
                            Name = "Action"
                        },
                        Title = "Salt",
                        Trailer = "youtube.com/salt",
                        Image = new byte[5]
                    },
                    CinemaHall = new CinemaHall
                    {
                        Id = 1L,
                        Label = "Apollo 1"
                    }
                },
                new Schedule
                {
                    Id = 3L,
                    StartTime = secondStartTime,
                    CinemaHallId = 1L,
                    MovieId = 1L,
                    Price = 12.50M,
                    Movie = new Movie
                    {
                        Id = 1L,
                        Duration = 1200,
                        Description = "Pretty good movie...",
                        GenreId = 3L,
                        Genre = new Genre
                        {
                            Id = 3L,
                            Name = "Action"
                        },
                        Title = "Salt",
                        Trailer = "youtube.com/salt",
                        Image = new byte[5]
                    },
                    CinemaHall = new CinemaHall
                    {
                        Id = 1L,
                        Label = "Apollo 1"
                    }
                },
                new Schedule
                {
                    Id = 4L,
                    StartTime = secondStartTime,
                    CinemaHallId = 2L,
                    MovieId = 1L,
                    Price = 12.50M,
                    Movie = new Movie
                    {
                        Id = 1L,
                        Duration = 1200,
                        Description = "Pretty good movie...",
                        GenreId = 3L,
                        Genre = new Genre
                        {
                            Id = 3L,
                            Name = "Action"
                        },
                        Title = "Salt",
                        Trailer = "youtube.com/salt",
                        Image = new byte[5]
                    },
                    CinemaHall = new CinemaHall {Id = 1L, Label = "Apollo 1"}
                },
                new Schedule
                {
                    Id = 5L,
                    StartTime = thirdStartTime, CinemaHallId = 1L, MovieId = 1L, Price = 12.50M,
                    Movie = new Movie
                    {
                        Id = 1L,
                        Duration = 1200,
                        Description = "Pretty good movie...",
                        GenreId = 3L,
                        Genre = new Genre
                        {
                            Id = 3L,
                            Name = "Action"
                        }, Title = "Salt",
                        Trailer = "youtube.com/salt",
                        Image = new byte[5]
                    },
                    CinemaHall = new CinemaHall
                    {
                        Id = 1L,
                        Label = "Apollo 1"
                    }
                },
                new Schedule
                {
                    Id = 6L,
                    StartTime = thirdStartTime,
                    CinemaHallId = 2L,
                    MovieId = 1L,
                    Price = 12.50M,
                    Movie = new Movie
                    {
                        Id = 1L,
                        Duration = 1200,
                        Description = "Pretty good movie...",
                        GenreId = 3L,
                        Genre = new Genre
                        {
                            Id = 3L,
                            Name = "Action"
                        },
                        Title = "Salt",
                        Trailer = "youtube.com/salt",
                        Image = new byte[5]
                    },
                    CinemaHall = new CinemaHall
                    {
                        Id = 1L,
                        Label = "Apollo 1"
                    }
                }
            };
        }

        private static IEnumerable<CinemaHall> CreateCinemaHalls()
        {
            return new List<CinemaHall>
            {
                new CinemaHall {Id = 1L, Label = "Apollo 1"},
                new CinemaHall {Id = 2L, Label = "Apollo 2"}
            };
        }

        private static IEnumerable<Movie> CreateMovies()
        {
            return new List<Movie>
            {
                new Movie
                {
                    Id = 1L, Duration = 1200, Description = "Pretty good movie...", GenreId = 3L,
                    Genre = new Genre {Id = 3L, Name = "Action"}, Title = "Salt", Trailer = "youtube.com/salt",
                    Image = new byte[5]
                },
                new Movie
                {
                    Id = 2L, Duration = 1337, Description = "A secret agent saves the world...", GenreId = 3L,
                    Genre = new Genre {Id = 3L, Name = "Action"}, Title = "007-Golden Eye",
                    Trailer = "youtube.com/james007", Image = new byte[5]
                },
                new Movie
                {
                    Id = 3L, Duration = 4242, Description = "Something with ships in the caribbean...", GenreId = 3L,
                    Genre = new Genre {Id = 3L, Name = "Action"}, Title = "Pirates of the Caribbean",
                    Trailer = "youtube.com/potc", Image = new byte[5]
                },
                new Movie
                {
                    Id = 4L, Duration = 9120, Description = "Dummy Movie", GenreId = 3L,
                    Genre = new Genre {Id = 1L, Name = "Horror"}, Title = "Dummy Title", Trailer = "dummy.com",
                    Image = new byte[5]
                }
            };
        }

        private static IEnumerable<Seat> CreateSeats()
        {
            return new List<Seat>
            {
                new Seat {Id = 1L,  Number = 1, LayoutColumn = 0, LayoutRow = 0, Locked = false, RowId = 1L, Row = new Row { Id = 1L, Number = 1, CategoryId = 1L, CinemaHallId = 1L, CinemaHall =   new CinemaHall {Id = 1L, Label = "Apollo 1"}, Category = new RowCategory {Id = 1L,  Name = "Silver", PriceFactor = 1.0}}},
                new Seat {Id = 2L , Number = 2, LayoutColumn = 1, LayoutRow = 0, Locked = false, RowId = 1L, Row = new Row { Id = 1L, Number = 1, CategoryId = 1L, CinemaHallId = 1L, CinemaHall =   new CinemaHall {Id = 1L, Label = "Apollo 1"}, Category = new RowCategory {Id = 1L,  Name = "Silver", PriceFactor = 1.0} }},
                new Seat {Id = 3L,  Number = 3, LayoutColumn = 2, LayoutRow = 0, Locked = false, RowId = 1L, Row = new Row { Id = 1L, Number = 1, CategoryId = 1L, CinemaHallId = 1L, CinemaHall =   new CinemaHall {Id = 1L, Label = "Apollo 1"} , Category = new RowCategory {Id = 1L,  Name = "Silver", PriceFactor = 1.0}}},
                new Seat {Id = 4L,  Number = 4, LayoutColumn = 3, LayoutRow = 0, Locked = false, RowId = 1L, Row = new Row { Id = 1L, Number = 1, CategoryId = 1L, CinemaHallId = 1L, CinemaHall =   new CinemaHall {Id = 1L, Label = "Apollo 1"} , Category = new RowCategory {Id = 1L,  Name = "Silver", PriceFactor = 1.0}}},
                new Seat {Id = 5L,  Number = 5, LayoutColumn = 4, LayoutRow = 0, Locked = false, RowId = 1L, Row = new Row { Id = 1L, Number = 1, CategoryId = 1L, CinemaHallId = 1L , CinemaHall =   new CinemaHall {Id = 1L, Label = "Apollo 1"}, Category = new RowCategory {Id = 1L,  Name = "Silver", PriceFactor = 1.0}}},
                new Seat {Id = 6L,  Number = 6, LayoutColumn = 5, LayoutRow = 0, Locked = false, RowId = 1L, Row = new Row { Id = 1L, Number = 1, CategoryId = 1L, CinemaHallId = 1L, CinemaHall =   new CinemaHall {Id = 1L, Label = "Apollo 1"} , Category = new RowCategory {Id = 1L,  Name = "Silver", PriceFactor = 1.0}}},
                new Seat {Id = 7L,  Number = 7, LayoutColumn = 6, LayoutRow = 0, Locked = false, RowId = 1L, Row = new Row { Id = 1L, Number = 1, CategoryId = 1L, CinemaHallId = 1L , CinemaHall =   new CinemaHall {Id = 1L, Label = "Apollo 1"}, Category = new RowCategory {Id = 1L,  Name = "Silver", PriceFactor = 1.0}}},
                new Seat {Id = 8L,  Number = 8, LayoutColumn = 7, LayoutRow = 0, Locked = false, RowId = 1L, Row = new Row { Id = 1L, Number = 1, CategoryId = 1L, CinemaHallId = 1L , CinemaHall =   new CinemaHall {Id = 1L, Label = "Apollo 1"}, Category = new RowCategory {Id = 1L,  Name = "Silver", PriceFactor = 1.0}}},

                new Seat {Id = 9L,  Number = 9, LayoutColumn = 0, LayoutRow = 1, Locked = false, RowId = 2L, Row = new Row { Id = 2L, Number = 2, CategoryId = 2L, CinemaHallId = 1L, CinemaHall =   new CinemaHall {Id = 1L, Label = "Apollo 1"} , Category = new RowCategory {Id = 2L,  Name = "Gold", PriceFactor = 1.25}}},
                new Seat {Id = 10L , Number = 10, LayoutColumn = 1, LayoutRow = 1, Locked = false, RowId = 2L, Row = new Row { Id = 2L, Number = 2, CategoryId = 2L, CinemaHallId = 1L , CinemaHall =   new CinemaHall {Id = 1L, Label = "Apollo 1"}, Category = new RowCategory {Id = 2L,  Name = "Gold", PriceFactor = 1.25}}},
                new Seat {Id = 11L,  Number = 11, LayoutColumn = 2, LayoutRow = 1, Locked = false, RowId = 2L, Row = new Row { Id = 2L, Number = 2, CategoryId = 2L, CinemaHallId = 1L , CinemaHall =   new CinemaHall {Id = 1L, Label = "Apollo 1"}, Category = new RowCategory {Id = 2L,  Name = "Gold", PriceFactor = 1.25}}},
                new Seat {Id = 12L,  Number = 12, LayoutColumn = 3, LayoutRow = 1, Locked = false, RowId = 2L, Row = new Row { Id = 2L, Number = 2, CategoryId = 2L, CinemaHallId = 1L, CinemaHall =   new CinemaHall {Id = 1L, Label = "Apollo 1"} , Category = new RowCategory {Id = 2L,  Name = "Gold", PriceFactor = 1.25}}},
                new Seat {Id = 13L,  Number = 13, LayoutColumn = 4, LayoutRow = 1, Locked = false, RowId = 2L, Row = new Row { Id = 2L, Number = 2, CategoryId = 2L, CinemaHallId = 1L , CinemaHall =   new CinemaHall {Id = 1L, Label = "Apollo 1"}, Category = new RowCategory {Id = 2L,  Name = "Gold", PriceFactor = 1.25}}},
                new Seat {Id = 14L,  Number = 14, LayoutColumn = 5, LayoutRow = 1, Locked = false, RowId = 2L, Row = new Row { Id = 2L, Number = 2, CategoryId = 2L, CinemaHallId = 1L , CinemaHall =   new CinemaHall {Id = 1L, Label = "Apollo 1"}, Category = new RowCategory {Id = 2L,  Name = "Gold", PriceFactor = 1.25}}},
                new Seat {Id = 15L,  Number = 15, LayoutColumn = 6, LayoutRow = 1, Locked = false, RowId = 2L, Row = new Row { Id = 2L, Number = 2, CategoryId = 2L, CinemaHallId = 1L , CinemaHall =   new CinemaHall {Id = 1L, Label = "Apollo 1"}, Category = new RowCategory {Id = 2L,  Name = "Gold", PriceFactor = 1.25}}},
                new Seat {Id = 16L,  Number = 16, LayoutColumn = 7, LayoutRow = 1, Locked = false, RowId = 2L, Row = new Row { Id = 2L, Number = 2, CategoryId = 2L, CinemaHallId = 1L , CinemaHall =   new CinemaHall {Id = 1L, Label = "Apollo 1"}, Category = new RowCategory {Id = 2L,  Name = "Gold", PriceFactor = 1.25}}},

                new Seat {Id = 17L,  Number = 20, LayoutColumn = 0, LayoutRow = 2, Locked = false, RowId = 3L, Row = new Row { Id = 3L, Number = 3, CategoryId = 4L, CinemaHallId = 1L , CinemaHall =   new CinemaHall {Id = 1L, Label = "Apollo 1"}, Category =  new RowCategory {Id = 4L,  Name = "Diamond", PriceFactor = 2.0}}},
                new Seat {Id = 18L , Number = 21, LayoutColumn = 1, LayoutRow = 2, Locked = false, RowId = 3L, Row = new Row { Id = 3L, Number = 3, CategoryId = 4L, CinemaHallId = 1L , CinemaHall =   new CinemaHall {Id = 1L, Label = "Apollo 1"}, Category =  new RowCategory {Id = 4L,  Name = "Diamond", PriceFactor = 2.0}}},
                new Seat {Id = 19L,  Number = 22, LayoutColumn = 2, LayoutRow = 2, Locked = false, RowId = 3L, Row = new Row { Id = 3L, Number = 3, CategoryId = 4L, CinemaHallId = 1L, CinemaHall =   new CinemaHall {Id = 1L, Label = "Apollo 1"} , Category =  new RowCategory {Id = 4L,  Name = "Diamond", PriceFactor = 2.0}}},
                new Seat {Id = 20L,  Number = 23, LayoutColumn = 3, LayoutRow = 2, Locked = true, RowId = 3L, Row = new Row { Id = 3L, Number = 3, CategoryId = 4L, CinemaHallId = 1L , CinemaHall =   new CinemaHall {Id = 1L, Label = "Apollo 1"}, Category =  new RowCategory {Id = 4L,  Name = "Diamond", PriceFactor = 2.0}}},
                new Seat {Id = 21L,  Number = 24, LayoutColumn = 4, LayoutRow = 2, Locked = false, RowId = 3L, Row = new Row { Id = 3L, Number = 3, CategoryId = 4L, CinemaHallId = 1L, CinemaHall =   new CinemaHall {Id = 1L, Label = "Apollo 1"} , Category =  new RowCategory {Id = 4L,  Name = "Diamond", PriceFactor = 2.0}}},
                new Seat {Id = 22L,  Number = 25, LayoutColumn = 5, LayoutRow = 2, Locked = true, RowId = 3L, Row = new Row { Id = 3L, Number = 3, CategoryId = 4L, CinemaHallId = 1L, CinemaHall =   new CinemaHall {Id = 1L, Label = "Apollo 1"} , Category =  new RowCategory {Id = 4L,  Name = "Diamond", PriceFactor = 2.0}}},
                new Seat {Id = 23L,  Number = 26, LayoutColumn = 6, LayoutRow = 2, Locked = false, RowId = 3L, Row = new Row { Id = 3L, Number = 3, CategoryId = 4L, CinemaHallId = 1L , CinemaHall =   new CinemaHall {Id = 1L, Label = "Apollo 1"}, Category =  new RowCategory {Id = 4L,  Name = "Diamond", PriceFactor = 2.0}}},
                new Seat {Id = 24L,  Number = 27, LayoutColumn = 7, LayoutRow = 2, Locked = true, RowId = 3L, Row = new Row { Id = 3L, Number = 3, CategoryId = 4L, CinemaHallId = 1L, CinemaHall =   new CinemaHall {Id = 1L, Label = "Apollo 1"} , Category =  new RowCategory {Id = 4L,  Name = "Diamond", PriceFactor = 2.0}}},
            };
        }
    }
}