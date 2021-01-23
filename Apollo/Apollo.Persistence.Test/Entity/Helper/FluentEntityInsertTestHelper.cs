using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Apollo.Persistence.FluentEntity.Interfaces;
using Apollo.Persistence.FluentEntity.Interfaces.Shared;
using Apollo.Persistence.Test.Entity.Mock;

namespace Apollo.Persistence.Test.Entity.Helper
{
    public class FluentEntityInsertTestHelper
    {
        private readonly IFluentEntityFrom _fluentEntity;
        private static DateTime _now;

        public FluentEntityInsertTestHelper(IEntityManager entityManager)
        {
            if (entityManager == null) throw new ArgumentNullException(nameof(entityManager));
            _fluentEntity = entityManager.FluentEntity();
            SeatMocks = CreateSeatMocks();
            RowCategoryMocks = CreateRowCategoryMocks();
            CinemaHallMocks = CreateCinemaHallMocks();
            CityMocks = CreateCityMocks();
            AddressMocks = CreateAddressMocks();
            GenreMocks = CreateGenreMocks();
            MovieMocks = CreateMovieMocks();
            MovieActorMocks = CreateMovieActorMocks();
            ActorMocks = CreateActorMocks();
            RoleMocks = CreateRoleMocks();
            UserMocks = CreateUserMocks();
            ScheduleMocks = CreateScheduleMocks();
            ReservationMocks = CreateReservationMocks();
            _now = DateTime.UtcNow;
        }

        public RowMock RowMock =>
            new RowMock { Id = 1L, RowVersion = _now, Number = 1, Category = 1L, CinemaHallId = 1L };

        public IList<RowCategoryMock> RowCategoryMocks { get; }

        public IList<SeatMock> SeatMocks { get; }

        public IList<CinemaHallMock> CinemaHallMocks { get; }

        public IList<ReservationMock> ReservationMocks { get; }
        public IList<ScheduleMock> ScheduleMocks { get; }
        public IList<MovieMock> MovieMocks { get; }
        public IList<GenreMock> GenreMocks { get; }
        public IList<ActorMock> ActorMocks { get; }
        public IList<MovieActorMock> MovieActorMocks { get; }


        public IList<CityMock> CityMocks { get; }

        public IList<AddressMock> AddressMocks { get; }

        public IList<RoleMock> RoleMocks { get; }
        public IList<UserMock> UserMocks { get; }


        public async Task TearDown()
        {
            await _fluentEntity.Delete<ReservationMock>().ExecuteAsync();
            await _fluentEntity.Delete<ScheduleMock>().ExecuteAsync();

            await _fluentEntity.Delete<SeatMock>().ExecuteAsync();
            await _fluentEntity.Delete<RowMock>().ExecuteAsync();
            await _fluentEntity.Delete<RowCategoryMock>().ExecuteAsync();
            await _fluentEntity.Delete<CinemaHallMock>().ExecuteAsync();

            await _fluentEntity.Delete<MovieActorMock>().ExecuteAsync();
            await _fluentEntity.Delete<ActorMock>().ExecuteAsync();
            await _fluentEntity.Delete<MovieMock>().ExecuteAsync();
            await _fluentEntity.Delete<GenreMock>().ExecuteAsync();

            await _fluentEntity.Delete<UserMock>().ExecuteAsync();
            await _fluentEntity.Delete<AddressMock>().ExecuteAsync();
            await _fluentEntity.Delete<CityMock>().ExecuteAsync();
            await _fluentEntity.Delete<RoleMock>().ExecuteAsync();
        }

        private static IList<SeatMock> CreateSeatMocks()
        {
            return new List<SeatMock>
            {
                new SeatMock {Id = 1L, RowVersion = _now, Number = 1, LayoutColumn = 0, LayoutRow = 0, Locked = false, RowId = 1L},
                new SeatMock {Id = 2L ,RowVersion = _now, Number = 2, LayoutColumn = 1, LayoutRow = 0, Locked = true, RowId = 1L},
                new SeatMock {Id = 3L, RowVersion = _now, Number = 3, LayoutColumn = 2, LayoutRow = 0, Locked = false, RowId = 1L},
                new SeatMock {Id = 4L, RowVersion = _now, Number = 4, LayoutColumn = 3, LayoutRow = 0, Locked = true, RowId = 1L},
                new SeatMock {Id = 5L, RowVersion = _now, Number = 5, LayoutColumn = 4, LayoutRow = 0, Locked = false, RowId = 1L},
                new SeatMock {Id = 6L, RowVersion = _now, Number = 6, LayoutColumn = 5, LayoutRow = 0, Locked = true, RowId = 1L},
                new SeatMock {Id = 7L, RowVersion = _now, Number = 7, LayoutColumn = 6, LayoutRow = 0, Locked = false, RowId = 1L},
                new SeatMock {Id = 8L, RowVersion = _now, Number = 8, LayoutColumn = 7, LayoutRow = 0, Locked = true, RowId = 1L}
            };
        }

        private static IList<RowCategoryMock> CreateRowCategoryMocks()
        {
            return new List<RowCategoryMock>
            {
                new RowCategoryMock {Id = 1L, RowVersion = _now, Name = "Silver", PriceFactor = 1.0},
                new RowCategoryMock {Id = 2L, RowVersion = _now, Name = "Gold", PriceFactor = 1.25},
                new RowCategoryMock {Id = 3L, RowVersion = _now, Name = "Platinum", PriceFactor = 1.75},
                new RowCategoryMock {Id = 4L, RowVersion = _now, Name = "Diamond", PriceFactor = 2.0},
            };
        }

        private static IList<CinemaHallMock> CreateCinemaHallMocks()
        {
            return new List<CinemaHallMock>
            {
                new CinemaHallMock {Id = 1L, RowVersion = _now, Label = "Star-Movie", SizeRow = 20, SizeColumn = 30}
            };
        }

        private static IList<ScheduleMock> CreateScheduleMocks()
        {
            return new List<ScheduleMock>
            {
                new ScheduleMock {Id = 1L, StartTime = _now, CinemaHallId = 1L, MovieId = 1L, Price = 10.50M}
            };
        }

        private static IList<CityMock> CreateCityMocks()
        {
            return new List<CityMock>
            {
                new CityMock {Id = 1L, PostalCode = "4232", Name = "Hagenberg"}
            };
        }

        private static IList<AddressMock> CreateAddressMocks()
        {
            return new List<AddressMock>
            {
                new AddressMock {Id = 1L, Number = 42, CityId = 1L, Street = "Softwareparkstraße"}
            };
        }

        private static IList<RoleMock> CreateRoleMocks()
        {
            return new List<RoleMock>
            {
                new RoleMock {Id = 1L, Label = "Customer", MaxReservations = 5}
            };
        }

        private static IList<UserMock> CreateUserMocks()
        {
            return new List<UserMock>
            {
                new UserMock {Id = 1L, Uuid = "724f75e6-71ce-45b0-8bb6-d3908cee727e", AddressId = 1L, Email = "Test.mai1l@gmx.at", FirstName = "Bert", LastName = "Her", Phone = "0660/12341235", RoleId = 1L},
                new UserMock {Id = 2L, Uuid = "f7f57b3d-890f-4ba8-9c86-278c68ec1dad", AddressId = 1L, Email = "Test.mail2@gmx.at", FirstName = "Bert", LastName = "Her", Phone = "0699/12341235", RoleId = 1L},
                new UserMock {Id = 3L, Uuid = "b008704f-4bab-4f89-858b-dcd0c4a7ada1", AddressId = 1L, Email = "Test.mail3@gmx.at", FirstName = "Bert", LastName = "Her", Phone = "0676/12341235", RoleId = 1L},
                new UserMock {Id = 4L, Uuid = "26919ccb-a9ef-4efa-b92f-f70cac055e4c", AddressId = 1L, Email = "Test.mail4@gmx.at", FirstName = "Bert", LastName = "Her", Phone = "0680/12341235", RoleId = 1L},
            };
        }


        private static IList<ReservationMock> CreateReservationMocks()
        {
            return new List<ReservationMock>
            {
                new ReservationMock {Id = 1L, Ticket = null, TicketId =  0L, ScheduleId = 1L, UserId = 1L},
                new ReservationMock {Id = 2L, Ticket = null, TicketId =  0L, ScheduleId = 1L, UserId = 2L},
                new ReservationMock {Id = 3L, Ticket = null, TicketId =  0L, ScheduleId = 1L, UserId = 3L},
                new ReservationMock {Id = 4L, Ticket = null, TicketId =  0L, ScheduleId = 1L, UserId = 4L}
            };
        }

        private static IList<GenreMock> CreateGenreMocks()
        {
            return new List<GenreMock>
            {
                new GenreMock {Id = 1L, Name="Horror"},
                new GenreMock {Id = 2L, Name="Romance"},
                new GenreMock {Id = 3L, Name="Action"},
                new GenreMock {Id = 4L, Name="Comedy"},
                new GenreMock {Id = 5L, Name="Sci-Fi"},
                new GenreMock {Id = 6L, Name="Action-Adventure"},
                new GenreMock {Id = 7L, Name="Fantasy"},
                new GenreMock {Id = 8L, Name="Splatter"}
            };
        }

        private static IList<ActorMock> CreateActorMocks()
        {
            return new List<ActorMock>
            {
                new ActorMock {Id = 1L,  RowVersion = _now, LastName = "Jolie", FirstName = "Angelina"},
                new ActorMock {Id = 2L,  RowVersion = _now, LastName = "Pitt", FirstName = "Brad"},
                new ActorMock {Id = 3L,  RowVersion = _now, LastName = "Cloney", FirstName = "George"},
                new ActorMock {Id = 4L,  RowVersion = _now, LastName = "Diesel", FirstName = "Vin"},
                new ActorMock {Id = 5L,  RowVersion = _now, LastName = "Depp", FirstName = "Jhonny"},
                new ActorMock {Id = 6L,  RowVersion = _now, LastName = "Brosnan", FirstName = "Pierce"}
            };
        }

        private static IList<MovieActorMock> CreateMovieActorMocks()
        {
            return new List<MovieActorMock>
            {
                new MovieActorMock{Id = 1L, RowVersion = _now, ActorId = 1L, MovieId = 1L},
                new MovieActorMock{Id = 2L, RowVersion = _now, ActorId = 6L, MovieId = 2L},
                new MovieActorMock{Id = 3L, RowVersion = _now, ActorId = 5L, MovieId = 3L},
            };
        }

        private static IList<MovieMock> CreateMovieMocks()
        {
            return new List<MovieMock>
            {
                new MovieMock{Id = 1L, RowVersion = _now, Duration = 1200, Description = "Pretty good movie...", GenreId = 3L, Title = "Salt", Trailer = "youtube.com/salt", Image= new byte[5], Rating = 3},
                new MovieMock{Id = 2L, RowVersion = _now, Duration = 1337, Description = "A secret agent saves the world...", GenreId = 3L, Title = "007-Golden Eye", Trailer = "youtube.com/james007" , Image= new byte[5], Rating = 4},
                new MovieMock{Id = 3L, RowVersion = _now, Duration = 4242, Description = "Something with ships in the caribbean...", GenreId = 3L, Title = "Pirates of the Caribbean", Trailer = "youtube.com/potc" , Image= new byte[5], Rating = 2},
                new MovieMock{Id = 4L, RowVersion = _now, Duration = 0, Description = "Dummy Movie", GenreId = 3L, Title = "Dummy Title", Trailer = "dummy.com" , Image= new byte[5], Rating = 0},
            };
        }

    }
}
