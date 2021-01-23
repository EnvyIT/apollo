using System;
using Apollo.Persistence.Attributes.Base;
using FluentAssertions;
using NUnit.Framework;

namespace Apollo.Persistence.Test.Entity.Mock
{
    public class MockTest
    {

        private ActorMock _actorMock;
        private GenreMock _genreMock;
        private MovieMock _movieMock;
        private MovieActorMock _movieActorMock;

        private CinemaHallMock _cinemaHallMock;
        private RowCategoryMock _rowCategoryMock;
        private RowMock _rowMock;
        private SeatMock _seatMock;
        private ReservationMock _reservationMock;
        private TicketMock _ticketMock;

        private UserMock _userMock;
        private CityMock _cityMock;
        private AddressMock _addressMock;
        private RoleMock _roleMock;


        [SetUp]
        public void Setup()
        {
            var now = DateTime.UtcNow;
            _actorMock = new ActorMock { FirstName = "Max", Id = 1L, LastName = "Power", RowVersion = now };
            _cinemaHallMock = new CinemaHallMock { Id = 1L, Label = "MoviePlex", RowVersion = now };
            _genreMock = new GenreMock { Id = 1L, Name = "Horror", RowVersion = now };
            _movieMock = new MovieMock
            {
                Description = "Boring Movie",
                Duration = 1234,
                GenreId = 1L,
                GenreMock = _genreMock,
                Id = 1L,
                Image = new byte[5],
                Title = "Horror-Part2",
                Trailer = "www.horror-part2.com/trailer",
                RowVersion = now
            };
            _movieActorMock = new MovieActorMock
            { ActorId = 1L, ActorMock = _actorMock, Id = 1L, MovieId = 1L, RowVersion = now, MovieMock = _movieMock };
            _movieMock.MovieActorMock = _movieActorMock;
            _rowCategoryMock = new RowCategoryMock { Id = 1L, Name = "Premium", PriceFactor = 1.5, RowVersion = now };
            _rowMock = new RowMock
            { Category = 1L, CinemaHallId = 1L, CinemaHallMock = _cinemaHallMock, Id = 1L, Number = 4, RowVersion = now };
            _seatMock = new SeatMock
            {
                Id = 1L,
                LayoutColumn = 1,
                LayoutRow = 1,
                Locked = false,
                Number = 1,
                RowVersion = now,
                RowMock = _rowMock,
                RowId = 1L
            };
            _ticketMock = new TicketMock { Id = 10L, Printed = DateTime.UtcNow };
            _cityMock = new CityMock()
            {
                Id = 1L,
                Name = "Hagenberg",
                RowVersion = DateTime.UtcNow,
                PostalCode = "4232"
            };
            _addressMock = new AddressMock
            {
                Id = 1L,
                City = _cityMock,
                CityId = 1L,
                RowVersion = DateTime.UtcNow,
                Number = 42,
                Street = "MegaImbaStraße"
            };
            _roleMock = new RoleMock {Id = 1L, Label = "Customer", MaxReservations = 5, RowVersion = DateTime.UtcNow};
            _userMock = new UserMock
            {
                Id = 1L, Address = _addressMock, AddressId = 1L, FirstName = "Hugo", LastName = "Hinterstoder",
                Email = "Hugo.H@gmail.com", Phone = "0660/1337424213", Role = _roleMock, RoleId = 1L,
                RowVersion = DateTime.UtcNow
            };
            _reservationMock = new ReservationMock { TicketId = 10L, Ticket = _ticketMock, User = _userMock, UserId = 1L };
        }

        [Test]
        public void TestInterfaces()
        {
            TestBaseInterfaceMethods(_actorMock, _genreMock);
            TestBaseInterfaceMethods(_cinemaHallMock, _actorMock);
            TestBaseInterfaceMethods(_genreMock, _actorMock);
            TestBaseInterfaceMethods(_movieActorMock, _actorMock);
            TestBaseInterfaceMethods(_movieMock, _actorMock);
            TestBaseInterfaceMethods(_rowCategoryMock, _actorMock);
            TestBaseInterfaceMethods(_rowMock, _actorMock);
            TestBaseInterfaceMethods(_seatMock, _actorMock);
            TestBaseInterfaceMethods(_ticketMock, _actorMock);
            TestBaseInterfaceMethods(_reservationMock, _actorMock);
            TestBaseInterfaceMethods(_cityMock, _actorMock);
            TestBaseInterfaceMethods(_addressMock, _actorMock);
            TestBaseInterfaceMethods(_userMock, _actorMock);
            TestBaseInterfaceMethods(_roleMock, _actorMock);
        }


        private static void TestBaseInterfaceMethods<T1, T2>(BaseEntity<T1> thisEntity, BaseEntity<T2> otherEntity) where T1 : BaseEntity<T1>, new() where T2 : BaseEntity<T2>, new()
        {
            thisEntity.Equals(otherEntity).Should().BeFalse();

            thisEntity.Equals(null).Should().BeFalse();

            thisEntity.Equals((T1)null).Should().BeFalse();

            thisEntity.Equals((object)null).Should().BeFalse();

            thisEntity.Equals(thisEntity).Should().BeTrue();

            thisEntity.Equals((T1)thisEntity).Should().BeTrue();

            var clone = (T1)thisEntity.Clone();
            clone.Equals(thisEntity).Should().BeTrue();


            var hashEqual = thisEntity.GetHashCode() == otherEntity.GetHashCode();
            hashEqual.Should().BeFalse();
            hashEqual = thisEntity.GetHashCode() == clone.GetHashCode();
            hashEqual.Should().BeTrue();

        }
    }
}
