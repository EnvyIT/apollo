using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Apollo.Domain.Entity;
using Apollo.Persistence.FluentEntity;
using Apollo.Persistence.FluentEntity.Interfaces.Shared;
using Apollo.Persistence.Util;
using Apollo.Repository.Interfaces;
using Apollo.Util;
using Microsoft.Extensions.Configuration;

namespace Apollo.Repository.Test
{
    public class RepositoryHelper
    {
        public IFluentEntityFrom FluentEntity { get; }
        public IRepositoryFactory RepositoryFactory { get; }
        public ISet<EntityType> FillTypes { get; set; }

        public IEnumerable<Movie> Movies { get; private set; }
        public IEnumerable<MovieActor> MovieActors { get; private set; }
        public IEnumerable<Actor> Actors { get; private set; }
        public IEnumerable<CinemaHall> CinemaHalls { get; private set; }
        public IEnumerable<Row> Rows { get; private set; }
        public IEnumerable<RowCategory> RowCategories { get; private set; }
        public IEnumerable<User> Users { get; private set; }
        public IEnumerable<Role> Roles { get; private set; }
        public IEnumerable<Address> Addresses { get; private set; }
        public IEnumerable<Seat> Seats { get; private set; }
        public IEnumerable<SeatReservation> SeatReservations { get; private set; }
        public IEnumerable<Schedule> Schedules { get; private set; }
        public IEnumerable<Genre> Genres { get; private set; }
        public IEnumerable<City> Cities { get; private set; }
        public IEnumerable<Reservation> Reservations { get; private set; }


        public RepositoryHelper()
        {
            ConfigurationHelper.ConfigurationRoot = new ConfigurationBuilder()
                .AddJsonFile("testsettings.json")
                .Build();
            var appSettings = ConfigurationHelper.GetValues("Apollo_Test");
            var connectionFactory = new ConnectionFactory(appSettings[0]);
            FluentEntity = EntityManagerFactory.CreateEntityManager(connectionFactory).FluentEntity();
            RepositoryFactory = new RepositoryFactory(connectionFactory);
            FillTypes = new HashSet<EntityType>();
        }


        public async Task TearDown()
        {
            await FluentEntity.Delete<SeatReservation>().ExecuteAsync();
            await FluentEntity.Delete<Reservation>().ExecuteAsync();
            await FluentEntity.Delete<Ticket>().ExecuteAsync();
            await FluentEntity.Delete<Schedule>().ExecuteAsync();

            await FluentEntity.Delete<Seat>().ExecuteAsync();
            await FluentEntity.Delete<Row>().ExecuteAsync();
            await FluentEntity.Delete<RowCategory>().ExecuteAsync();
            await FluentEntity.Delete<CinemaHall>().ExecuteAsync();

            await FluentEntity.Delete<MovieActor>().ExecuteAsync();
            await FluentEntity.Delete<Actor>().ExecuteAsync();
            await FluentEntity.Delete<Movie>().ExecuteAsync();
            await FluentEntity.Delete<Genre>().ExecuteAsync();

            await FluentEntity.Delete<User>().ExecuteAsync();
            await FluentEntity.Delete<Address>().ExecuteAsync();
            await FluentEntity.Delete<City>().ExecuteAsync();
            await FluentEntity.Delete<Role>().ExecuteAsync();
        }

        public async Task SeedDatabase()
        {

            if (EntityDependsOnRole())
            {
                Roles = CreateRoles();
                await FluentEntity.InsertInto(Roles).ExecuteAsync();
            }
            if (EntityDependsOnCity())
            {
                Cities = CreateCities();
                await FluentEntity.InsertInto(Cities).ExecuteAsync();
            }
            if (EntityDependsOnAddress())
            {
                Addresses = CreateAddresses();
                await FluentEntity.InsertInto(Addresses).ExecuteAsync();
            }
            if (EntityDependsOnUser())
            {
                Users = CreateUsers();
                await FluentEntity.InsertInto(Users).ExecuteAsync();
            }


            if (EntityDependsOnGenre())
            {
                Genres = CreateGenres();
                await FluentEntity.InsertInto(Genres).ExecuteAsync();
            }
            if (EntityDependsOnMovie())
            {
                Movies = CreateMovies();
                await FluentEntity.InsertInto(Movies).ExecuteAsync();
            }
            if (EntityDependsOnActor())
            {
                Actors = CreateActors();
                await FluentEntity.InsertInto(Actors).ExecuteAsync();
            }
            if (EntityDependsOnMovieActor())
            {
                MovieActors = CreateMovieActors();
                await FluentEntity.InsertInto(MovieActors).ExecuteAsync();
            }

            if (EntityDependsOnCinemaHall())
            {
                CinemaHalls = CreateCinemaHalls();
                await FluentEntity.InsertInto(CinemaHalls).ExecuteAsync();
            }
            if (EntityDependsOnRowCategory())
            {
                RowCategories = CreateRowCategories();
                await FluentEntity.InsertInto(RowCategories).ExecuteAsync();
            }
            if (EntityDependsOnRow())
            {
                Rows = CreateRows();
                await FluentEntity.InsertInto(Rows).ExecuteAsync();
            }
            if (EntityDependsOnSeat())
            {
                Seats = CreateSeats();
                await FluentEntity.InsertInto(Seats).ExecuteAsync();
            }

            if (EntityDependsOnSchedule())
            {
                Schedules = CreateSchedules();
                await FluentEntity.InsertInto(Schedules).ExecuteAsync();
            }

            if (EntityDependsOnReservation())
            {
                Reservations = CreateReservations();
                await FluentEntity.InsertInto(Reservations).ExecuteAsync();
            }

            if (EntityDependsOnSeatReservation())
            {
                SeatReservations = CreateSeatReservations();
                await FluentEntity.InsertInto(SeatReservations).ExecuteAsync();
            }

        }

        private bool EntityDependsOnSeatReservation()
        {
            return FillTypes.Contains(EntityType.SeatReservation);
        }

        private bool EntityDependsOnReservation()
        {
            return FillTypes.Contains(EntityType.Reservation) || FillTypes.Contains(EntityType.SeatReservation);
        }

        private bool EntityDependsOnSchedule()
        {
            return FillTypes.Contains(EntityType.Schedule) || FillTypes.Contains(EntityType.Reservation) || FillTypes.Contains(EntityType.SeatReservation);
        }

        private bool EntityDependsOnSeat()
        {
            return FillTypes.Contains(EntityType.Seat) || FillTypes.Contains(EntityType.SeatReservation);
        }

        private bool EntityDependsOnRow()
        {
            return FillTypes.Contains(EntityType.Row) || FillTypes.Contains(EntityType.Seat) || FillTypes.Contains(EntityType.SeatReservation);
        }

        private bool EntityDependsOnRowCategory()
        {
            return FillTypes.Contains(EntityType.RowCategory) || FillTypes.Contains(EntityType.Row) || FillTypes.Contains(EntityType.Seat) || FillTypes.Contains(EntityType.SeatReservation);
        }

        private bool EntityDependsOnCinemaHall()
        {
            return FillTypes.Contains(EntityType.CinemaHall) || FillTypes.Contains(EntityType.Row) || FillTypes.Contains(EntityType.Seat) || FillTypes.Contains(EntityType.Schedule) || FillTypes.Contains(EntityType.Reservation) || FillTypes.Contains(EntityType.SeatReservation);
        }

        private bool EntityDependsOnMovieActor()
        {
            return FillTypes.Contains(EntityType.MovieActor);
        }

        private bool EntityDependsOnActor()
        {
            return FillTypes.Contains(EntityType.Actor) || FillTypes.Contains(EntityType.MovieActor);
        }

        private bool EntityDependsOnMovie()
        {
            return FillTypes.Contains(EntityType.Movie) || FillTypes.Contains(EntityType.MovieActor) || FillTypes.Contains(EntityType.Schedule) || FillTypes.Contains(EntityType.Reservation) || FillTypes.Contains(EntityType.SeatReservation);
        }

        private bool EntityDependsOnGenre()
        {
            return FillTypes.Contains(EntityType.Genre) || FillTypes.Contains(EntityType.Movie) || FillTypes.Contains(EntityType.MovieActor) || FillTypes.Contains(EntityType.Schedule) || FillTypes.Contains(EntityType.Reservation) || FillTypes.Contains(EntityType.SeatReservation);
        }

        private bool EntityDependsOnUser()
        {
            return FillTypes.Contains(EntityType.User) || FillTypes.Contains(EntityType.Reservation) || FillTypes.Contains(EntityType.SeatReservation);
        }

        private bool EntityDependsOnAddress()
        {
            return FillTypes.Contains(EntityType.Address) || FillTypes.Contains(EntityType.User) || FillTypes.Contains(EntityType.Reservation) || FillTypes.Contains(EntityType.SeatReservation);
        }

        private bool EntityDependsOnCity()
        {
            return FillTypes.Contains(EntityType.City) || FillTypes.Contains(EntityType.Address) || FillTypes.Contains(EntityType.User) || FillTypes.Contains(EntityType.Reservation) || FillTypes.Contains(EntityType.SeatReservation);
        }

        private bool EntityDependsOnRole()
        {
            return FillTypes.Contains(EntityType.Role) || FillTypes.Contains(EntityType.City) || FillTypes.Contains(EntityType.Address) || FillTypes.Contains(EntityType.User) || FillTypes.Contains(EntityType.Reservation) || FillTypes.Contains(EntityType.SeatReservation);
        }
        private static IEnumerable<Reservation> CreateReservations()
        {
            return new List<Reservation>
            {
                new Reservation {Id = 1L, ScheduleId = 1L, UserId = 1000L}
            };
        }

        private static IEnumerable<SeatReservation> CreateSeatReservations()
        {
            return new List<SeatReservation>
            {
                new SeatReservation {Id = 1L, ReservationId = 1L, SeatId = 1L},
                new SeatReservation {Id = 2L, ReservationId = 1L, SeatId = 2L},
                new SeatReservation {Id = 3L, ReservationId = 1L, SeatId = 3L},
            };
        }

        private static IEnumerable<Genre> CreateGenres()
        {
            return new List<Genre>
            {
                new Genre {Id = 1L, Name = "Horror"},
                new Genre {Id = 2L, Name = "Romance"},
                new Genre {Id = 3L, Name = "Action"},
                new Genre {Id = 4L, Name = "Comedy"},
                new Genre {Id = 5L, Name = "Sci-Fi"},
                new Genre {Id = 6L, Name = "Action-Adventure"},
                new Genre {Id = 7L, Name = "Fantasy"},
                new Genre {Id = 8L, Name = "Splatter"}
            };
        }

        private static IEnumerable<Actor> CreateActors()
        {
            return new List<Actor>
            {
                new Actor {Id = 1L,  LastName = "Jolie", FirstName = "Angelina"},
                new Actor {Id = 2L,  LastName = "Pitt", FirstName = "Brad"},
                new Actor {Id = 3L, LastName = "Cloney", FirstName = "George"},
                new Actor {Id = 4L,   LastName = "Diesel", FirstName = "Vin"},
                new Actor {Id = 5L,   LastName = "Depp", FirstName = "Jhonny"},
                new Actor {Id = 6L,   LastName = "Brosnan", FirstName = "Pierce"}
            };
        }

        private static IEnumerable<MovieActor> CreateMovieActors()
        {
            return new List<MovieActor>
            {
                new MovieActor{Id = 1L, ActorId = 1L, MovieId = 1L},
                new MovieActor{Id = 2L, ActorId = 2L, MovieId = 1L},
                new MovieActor{Id = 3L, ActorId = 3L, MovieId = 1L},
                new MovieActor{Id = 4L, ActorId = 4L, MovieId = 1L},
                new MovieActor{Id = 5L, ActorId = 5L, MovieId = 1L},
                new MovieActor{Id = 6L, ActorId = 6L, MovieId = 2L},
                new MovieActor{Id = 7L, ActorId = 5L, MovieId = 3L},
            };
        }

        private static IEnumerable<Movie> CreateMovies()
        {
            return new List<Movie>
            {
                new Movie{Id = 1L, Duration = 1200, Description = "Pretty good movie...", GenreId = 3L, Title = "Salt", Trailer = "youtube.com/salt", Image= new byte[5], Rating = 1},
                new Movie{Id = 2L,  Duration = 1337, Description = "A secret agent saves the world...", GenreId = 3L, Title = "007-Golden Eye", Trailer = "youtube.com/james007" , Image= new byte[5], Rating = 3},
                new Movie{Id = 3L,  Duration = 4242, Description = "Something with ships in the caribbean...", GenreId = 3L, Title = "Pirates of the Caribbean", Trailer = "youtube.com/potc" , Image= new byte[5], Rating = 4},
                new Movie{Id = 4L,  Duration = 9120, Description = "Dummy Movie", GenreId = 3L, Title = "Dummy Title", Trailer = "dummy.com" , Image= new byte[5], Rating = 5},
            };
        }

        private static IEnumerable<Seat> CreateSeats()
        {
            return new List<Seat>
            {
                new Seat {Id = 1L,  Number = 1, LayoutColumn = 0, LayoutRow = 0, Locked = false, RowId = 1L},
                new Seat {Id = 2L , Number = 2, LayoutColumn = 1, LayoutRow = 0, Locked = false, RowId = 1L},
                new Seat {Id = 3L,  Number = 3, LayoutColumn = 2, LayoutRow = 0, Locked = false, RowId = 1L},
                new Seat {Id = 4L,  Number = 4, LayoutColumn = 3, LayoutRow = 0, Locked = false, RowId = 1L},
                new Seat {Id = 5L,  Number = 5, LayoutColumn = 4, LayoutRow = 0, Locked = false, RowId = 1L},
                new Seat {Id = 6L,  Number = 6, LayoutColumn = 5, LayoutRow = 0, Locked = false, RowId = 1L},
                new Seat {Id = 7L,  Number = 7, LayoutColumn = 6, LayoutRow = 0, Locked = false, RowId = 1L},
                new Seat {Id = 8L,  Number = 8, LayoutColumn = 7, LayoutRow = 0, Locked = false, RowId = 1L},

                new Seat {Id = 9L,  Number = 9, LayoutColumn = 0, LayoutRow = 1, Locked = false, RowId = 2L},
                new Seat {Id = 10L , Number = 10, LayoutColumn = 1, LayoutRow = 1, Locked = false, RowId = 2L},
                new Seat {Id = 11L,  Number = 11, LayoutColumn = 2, LayoutRow = 1, Locked = false, RowId = 2L},
                new Seat {Id = 12L,  Number = 12, LayoutColumn = 3, LayoutRow = 1, Locked = false, RowId = 2L},
                new Seat {Id = 13L,  Number = 13, LayoutColumn = 4, LayoutRow = 1, Locked = false, RowId = 2L},
                new Seat {Id = 14L,  Number = 14, LayoutColumn = 5, LayoutRow = 1, Locked = false, RowId = 2L},
                new Seat {Id = 15L,  Number = 15, LayoutColumn = 6, LayoutRow = 1, Locked = false, RowId = 2L},
                new Seat {Id = 16L,  Number = 16, LayoutColumn = 7, LayoutRow = 1, Locked = false, RowId = 2L},

                new Seat {Id = 17L,  Number = 20, LayoutColumn = 0, LayoutRow = 2, Locked = false, RowId = 3L},
                new Seat {Id = 18L , Number = 21, LayoutColumn = 1, LayoutRow = 2, Locked = false, RowId = 3L},
                new Seat {Id = 19L,  Number = 22, LayoutColumn = 2, LayoutRow = 2, Locked = false, RowId = 3L},
                new Seat {Id = 20L,  Number = 23, LayoutColumn = 3, LayoutRow = 2, Locked = true, RowId = 3L},
                new Seat {Id = 21L,  Number = 24, LayoutColumn = 4, LayoutRow = 2, Locked = false, RowId = 3L},
                new Seat {Id = 22L,  Number = 25, LayoutColumn = 5, LayoutRow = 2, Locked = true, RowId = 3L},
                new Seat {Id = 23L,  Number = 26, LayoutColumn = 6, LayoutRow = 2, Locked = false, RowId = 3L},
                new Seat {Id = 24L,  Number = 27, LayoutColumn = 7, LayoutRow = 2, Locked = true, RowId = 3L},
            };
        }

        private static IEnumerable<Row> CreateRows()
        {
            return new List<Row>
            {
                new Row { Id = 1L, Number = 1, CategoryId = 1L, CinemaHallId = 1L },
                new Row { Id = 2L, Number = 2, CategoryId = 2L, CinemaHallId = 1L },
                new Row { Id = 3L, Number = 3, CategoryId = 4L, CinemaHallId = 1L },
            };

        }

        private static IEnumerable<RowCategory> CreateRowCategories()
        {
            return new List<RowCategory>
            {
                new RowCategory {Id = 1L,  Name = "Silver", PriceFactor = 1.0},
                new RowCategory {Id = 2L,  Name = "Gold", PriceFactor = 1.25},
                new RowCategory {Id = 3L,  Name = "Platinum", PriceFactor = 1.75},
                new RowCategory {Id = 4L,  Name = "Diamond", PriceFactor = 2.0},
            };
        }

        private static IEnumerable<CinemaHall> CreateCinemaHalls()
        {
            return new List<CinemaHall>
            {
                new CinemaHall {Id = 1L, Label = "Apollo 1", SizeRow = 3, SizeColumn = 8},
                new CinemaHall {Id = 2L, Label = "Apollo 2", SizeRow = 5, SizeColumn = 6}
            };
        }

        private static IEnumerable<Role> CreateRoles()
        {
            return new List<Role>
            {
                new Role {Id = 1L, Label = "Role 1", MaxReservations = 10},
                new Role {Id = 2L, Label = "Role 2", MaxReservations = 20},
                new Role {Id = 3L, Label = "Role 3", MaxReservations = 30},
                new Role {Id = 4L, Label = "Role 4", MaxReservations = 40},
                new Role {Id = 5L, Label = "Role 5", MaxReservations = 50}
            };
        }

        private static IEnumerable<City> CreateCities()
        {
            return new List<City>
            {
                new City {Id = 10L, PostalCode = "1111", Name = "City 1"},
                new City {Id = 20L, PostalCode = "2222", Name = "City 2"},
                new City {Id = 30L, PostalCode = "3333", Name = "City 3"},
                new City {Id = 40L, PostalCode = "4444", Name = "City 4"},
                new City {Id = 50L, PostalCode = "5555", Name = "City 5"}
            };
        }

        private static IEnumerable<Address> CreateAddresses()
        {
            return new List<Address>
            {
                new Address {Id = 100L, CityId = 10L, Street = "Street 1", Number = 10},
                new Address {Id = 200L, CityId = 10L, Street = "Street 2", Number = 20},
                new Address {Id = 300L, CityId = 20L, Street = "Street 3", Number = 30},
                new Address {Id = 400L, CityId = 20L, Street = "Street 4", Number = 40},
                new Address {Id = 500L, CityId = 30L, Street = "Street 5", Number = 50},
                new Address {Id = 600L, CityId = 30L, Street = "Street 6", Number = 60},
                new Address {Id = 700L, CityId = 40L, Street = "Street 7", Number = 70},
                new Address {Id = 800L, CityId = 40L, Street = "Street 8", Number = 80},
                new Address {Id = 900L, CityId = 50L, Street = "Street 9", Number = 90},
                new Address {Id = 1000L, CityId = 50L, Street = "Street 10", Number = 100}
            };
        }

        private static IEnumerable<User> CreateUsers()
        {
            return new List<User>
            {
                new User {Id = 1000L, Uuid = "724f75e6-71ce-45b0-8bb6-d3908cee727e", AddressId = 100L, Email = "mail_1@fh-ooe.at", FirstName = "First 1", LastName = "Last 1", Phone = "0664000000", RoleId = 1L},
                new User {Id = 2000L, Uuid = "f7f57b3d-890f-4ba8-9c86-278c68ec1dad", AddressId = 200L, Email = "mail_2@fh-ooe.at", FirstName = "First 2", LastName = "Last 2", Phone = "0664111111", RoleId = 1L},
                new User {Id = 3000L, Uuid = "b008704f-4bab-4f89-858b-dcd0c4a7ada1", AddressId = 300L, Email = "mail_3@fh-ooe.at", FirstName = "First 3", LastName = "Last 3", Phone = "0664222222", RoleId = 2L},
                new User {Id = 4000L, Uuid = "26919ccb-a9ef-4efa-b92f-f70cac055e4c", AddressId = 400L, Email = "mail_4@fh-ooe.at", FirstName = "First 4", LastName = "Last 4", Phone = "0664333333", RoleId = 2L},
                new User {Id = 5000L, Uuid = "92679fae-a543-49e9-bac6-3c34e176b205", AddressId = 500L, Email = "mail_5@fh-ooe.at", FirstName = "First 5", LastName = "Last 5", Phone = "0664444444", RoleId = 3L},
                new User {Id = 6000L, Uuid = "87c629e3-053e-4a8f-a039-3953806d1bb8", AddressId = 600L, Email = "mail_6@fh-ooe.at", FirstName = "First 6", LastName = "Last 6", Phone = "0664555555", RoleId = 3L},
                new User {Id = 7000L, Uuid = "f5476050-6bf5-488c-afa9-1197fd94e8e6", AddressId = 700L, Email = "mail_7@fh-ooe.at", FirstName = "First 7", LastName = "Last 7", Phone = "0664666666", RoleId = 4L},
                new User {Id = 8000L, Uuid = "273cbeec-348e-4914-9160-5daddd194fdb", AddressId = 800L, Email = "mail_8@fh-ooe.at", FirstName = "First 8", LastName = "Last 8", Phone = "0664777777", RoleId = 4L},
                new User {Id = 9000L, Uuid = "9e12e422-87cb-4168-b0d7-29cc7bd80cd7", AddressId = 900L, Email = "mail_9@fh-ooe.at", FirstName = "First 9", LastName = "Last 9", Phone = "0664888888", RoleId = 5L},
                new User {Id = 10000L, Uuid = "32942e7f-da87-456e-ad3b-26afa7607880", AddressId = 1000L, Email = "mail_10@fh-ooe.at", FirstName = "First 10", LastName = "Last 10", Phone = "0664999999", RoleId = 5L}
            };
        }

        private static IEnumerable<Schedule> CreateSchedules()
        {
            var startTime = new DateTime(2020, 11, 4, 20, 15, 00);
            var secondStartTime = new DateTime(2020, 11, 5, 19, 35, 00);
            var thirdStartTime = new DateTime(2020, 11, 4, 23, 15, 00);
            return new List<Schedule>
            {
                new Schedule {Id = 1L,StartTime = startTime, CinemaHallId = 1L, MovieId = 1L, Price = 12.50M},
                new Schedule {Id = 2L,StartTime = startTime, CinemaHallId = 2L, MovieId = 1L, Price = 12.50M},
                new Schedule {Id = 3L,StartTime = secondStartTime, CinemaHallId = 1L, MovieId = 1L, Price = 12.50M},
                new Schedule {Id = 4L,StartTime = secondStartTime, CinemaHallId = 2L, MovieId = 1L, Price = 12.50M},
                new Schedule {Id = 5L,StartTime = thirdStartTime, CinemaHallId = 1L, MovieId = 1L, Price = 12.50M},
                new Schedule {Id = 6L, StartTime = thirdStartTime, CinemaHallId = 2L, MovieId = 1L, Price = 12.50M}
           
            };
        }

    }

}
