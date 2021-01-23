using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Apollo.Core.Dto;
using Apollo.Core.Exception;
using Apollo.Core.Interfaces;
using Apollo.Core.Validation;
using Apollo.Domain.Entity;
using FluentAssertions;
using Moq;
using NUnit.Framework;
using static Apollo.Core.Dto.Mapper;

namespace Apollo.Core.Test
{
    public class ReservationServiceTests
    {
        private MockingHelper _mockingHelper;
        private IReservationService _reservationService;

        [SetUp]
        public void Setup()
        {
            _mockingHelper = new MockingHelper();
            _reservationService = _mockingHelper.ServiceFactory.Object.CreateReservationService();
        }

        [Test]
        public async Task Test_AddCustomRule_ShouldThrowException()
        {
            const int maxSeats = 5;
            var desiredSeats = CreateSeats().Where(s => s.Id <= (long)maxSeats).Select(Map).ToList();
            PrepareReservationMocks(maxSeats, ScheduleA, ScheduleB, desiredSeats);

            _reservationService.ClearRules(); // no rules active -> normally it would throw 
            _reservationService.AddRule(new SeatAvailableValidation()); // add seat available rule -> should throw now 
            Func<Task<ReservationDto>> addReservationAsync = async () => await _reservationService.AddReservationAsync(desiredSeats, ScheduleB.Id, 2L);
            await addReservationAsync.Should().ThrowExactlyAsync<SeatValidationException>();
        }

        [Test]
        public async Task Test_ClearRules_ShouldRemoveAllValidationRules()
        {
            const int maxSeats = 5;
            var desiredSeats = CreateSeats().Where(s => s.Id <= (long)maxSeats).Select(Map).ToList();
            PrepareReservationMocks(maxSeats, ScheduleA, ScheduleB, desiredSeats);

            _reservationService.ClearRules(); // no rules active -> normally it would throw 
            Func<Task<ReservationDto>> addReservationAsync = async () => await _reservationService.AddReservationAsync(desiredSeats, ScheduleB.Id, 2L);
            await addReservationAsync.Should().NotThrowAsync();
        }

        [Test]
        public async Task Test_ClearRules_AndResetDefault_ShouldFirstNotThrowAndThenThrow()
        {
            const int maxSeats = 5;
            var desiredSeats = CreateSeats().Where(s => s.Id <= (long)maxSeats).Select(Map).ToList();
            PrepareReservationMocks(maxSeats, ScheduleA, ScheduleB, desiredSeats);

            _reservationService.ClearRules(); // no rules active -> normally it would throw 
            Func<Task<ReservationDto>> addReservationAsync = async () => await _reservationService.AddReservationAsync(desiredSeats, ScheduleB.Id, 2L);
            await addReservationAsync.Should().NotThrowAsync();

            _reservationService.UseDefaultRules();
            await addReservationAsync.Should().ThrowExactlyAsync<SeatValidationException>();
        }

        [Test]
        public async Task Test_AddReservation_MultipleThreads_ShouldThrowForSecondThread()
        {
            const int maxSeats = 5;
            var desiredSeats = CreateSeats().Where(s => s.Id <= (long)maxSeats).Select(Map).ToList();
            PrepareReservationMocks(maxSeats, ScheduleA, ScheduleB, desiredSeats);

            Func<Task<ReservationDto>> userAReservesSeatAsync =  async () => await  _reservationService.AddReservationAsync(desiredSeats, ScheduleA.Id, 1L);
            Func<Task<ReservationDto>> userBReserveSeatAsync = async () => await _reservationService.AddReservationAsync(desiredSeats, ScheduleB.Id, 2L);
            var tasks = new[]
            {
                new Task<Task<ReservationDto>>(userAReservesSeatAsync),
                new Task<Task<ReservationDto>>(userBReserveSeatAsync
),
            };
            tasks[0].Start();
            tasks[1].Start();
            
            var reservationTaskA = await tasks[0];
            var reservationTaskB = await tasks[1];
            try
            {
                await reservationTaskB; //should throw SeatValidationException
                Assert.False(true, "This block should not be executed otherwise multi-thread-test is corrupted");
            }
            catch (SeatValidationException e)
            {
                Console.WriteLine(e);
                Assert.True(true, "This block should be executed otherwise multi-thread-test is corrupted");
            }

            var reservationDto = await reservationTaskA;
            reservationDto.Should().NotBeNull();
            _mockingHelper.RepositoryTicket.Verify(
        _ => _.AddReservationAsync(It.IsAny<IEnumerable<Seat>>(), It.IsAny<long>(), It.IsAny<long>()),
        Times.Exactly(1));
        }

        [Test]
        public async Task Test_AddReservation_InvalidParameter_SeatDto_ShouldThrow()
        {
            const int maxSeats = 5;
            //mocks for GetLayout in Infrastructure
            _mockingHelper.RepositoryInfrastructure
                .Setup(_ => _.GetActiveSeatsWithRowAndCategoryAsync(It.IsAny<long>()))
                .ReturnsAsync(CreateSeats);

            _mockingHelper.RepositoryInfrastructure
                .Setup(_ => _.GetCinemaHallByIdAsync(It.IsAny<long>()))
                .ReturnsAsync(CreateCinemaHalls().First());

            _mockingHelper.RepositoryTicket
                .Setup(_ => _.GetFreeSeatsAsync(It.IsAny<Schedule>()))
                .ReturnsAsync(CreateSeats().Where(s => !new List<long> { 19L, 21L }.Contains(s.Id)));

            //mocks for User (maxSeatReservations)
            _mockingHelper.RepositoryUser
                .Setup(_ => _.GetUserWithAllReferencesByIdAsync(It.IsAny<long>()))
                .ReturnsAsync(CreateUserWithMaxReservations(maxSeats));

            _mockingHelper.RepositorySchedule.Setup(_ => _.GetScheduleByIdAsync(ScheduleA.Id)).ReturnsAsync(ScheduleA);
            _mockingHelper.RepositoryUser.Setup(_ => _.UserExistAsync(1L)).ReturnsAsync(true);
            _mockingHelper.RepositoryInfrastructure.Setup(_ => _.GetSeatByIdAsync(CreateSeats().First().Id))
                .ReturnsAsync(CreateSeats().First());
            _mockingHelper.RepositoryInfrastructure.Setup(_ => _.GetSeatByIdAsync(CreateSeats().Last().Id))
                .ThrowsAsync(new PersistenceException(""));

            Func<Task> callback = () => _reservationService.AddReservationAsync(null, 1L, 1L);
            await callback.Should().ThrowAsync<ArgumentException>();
            _mockingHelper.RepositoryTicket.Verify(
                _ => _.AddReservationAsync(It.IsAny<IEnumerable<Seat>>(), It.IsAny<long>(), It.IsAny<long>()),
                Times.Never);

            callback = () => _reservationService.AddReservationAsync(Enumerable.Empty<SeatDto>(), 1L, 1L);
            await callback.Should().ThrowAsync<ArgumentException>();
            _mockingHelper.RepositoryTicket.Verify(
                _ => _.AddReservationAsync(It.IsAny<IEnumerable<Seat>>(), It.IsAny<long>(), It.IsAny<long>()),
                Times.Never);


            callback = () => _reservationService.AddReservationAsync(new[] { Map(CreateSeats().Last()) }, 1L, 1L);
            await callback.Should().ThrowAsync<SeatValidationException>();
            _mockingHelper.RepositoryTicket.Verify(
                _ => _.AddReservationAsync(It.IsAny<IEnumerable<Seat>>(), It.IsAny<long>(), It.IsAny<long>()),
                Times.Never);
        }

        [Test]
        public async Task Test_AddReservation_InvalidParameter_ScheduleId_ShouldThrow()
        {
            _mockingHelper.RepositorySchedule.Setup(_ => _.GetScheduleByIdAsync(ScheduleA.Id)).ReturnsAsync(ScheduleA);
            _mockingHelper.RepositoryUser.Setup(_ => _.UserExistAsync(1L)).ReturnsAsync(true);
            _mockingHelper.RepositoryInfrastructure.Setup(_ => _.GetSeatByIdAsync(CreateSeats().First().Id))
                .ReturnsAsync(CreateSeats().First());

            Func<Task> callback = () => _reservationService.AddReservationAsync(new[] { Map(CreateSeats().Last()) }, 500L, 1L);
            await callback.Should().ThrowAsync<ArgumentNullException>();
            _mockingHelper.RepositoryTicket.Verify(
                _ => _.AddReservationAsync(It.IsAny<IEnumerable<Seat>>(), It.IsAny<long>(), It.IsAny<long>()),
                Times.Never);
        }

        [Test]
        public async Task Test_AddReservation_InvalidParameter_UserId_ShouldThrow()
        {
            _mockingHelper.RepositorySchedule.Setup(_ => _.GetScheduleByIdAsync(ScheduleA.Id)).ReturnsAsync(ScheduleA);
            _mockingHelper.RepositoryUser.Setup(_ => _.UserExistAsync(1L)).ReturnsAsync(false);
            _mockingHelper.RepositoryInfrastructure.Setup(_ => _.GetSeatByIdAsync(CreateSeats().First().Id))
                .ReturnsAsync(CreateSeats().First());

            Func<Task> callback = () => _reservationService.AddReservationAsync(new[] { Map(CreateSeats().Last()) }, 500L, 1L);
            await callback.Should().ThrowAsync<ArgumentNullException>();
            _mockingHelper.RepositoryTicket.Verify(
                _ => _.AddReservationAsync(It.IsAny<IEnumerable<Seat>>(), It.IsAny<long>(), It.IsAny<long>()),
                Times.Never);
        }

        [Test]
        public async Task Test_AddReservation_ValidParameters_ShouldReturn_Object()
        {
            var reservation = Reservations.First();
            const int maxSeats = 5;
            var desiredSeats = CreateSeats().Where(s => s.Id <= (long)maxSeats).Select(Map).ToList();
            PrepareReservationMocks(maxSeats, ScheduleA, ScheduleB, desiredSeats);

            var result = await _reservationService.AddReservationAsync(desiredSeats, ScheduleA.Id, 1L);

            result.Should().NotBeNull();
            result.UserId.Should().Be(reservation.UserId);
            result.ScheduleId.Should().Be(reservation.ScheduleId);
            result.Ticket.Should().BeNull();
            _mockingHelper.RepositoryTicket.Verify(
                _ => _.AddReservationAsync(It.IsAny<IEnumerable<Seat>>(), It.IsAny<long>(), It.IsAny<long>()),
                Times.Once);
        }


        [Test]
        public async Task Test_AddReservation_UserCannotReserveMoreThan4Seats_ShouldThrowArgumentOutOfRangeException()
        {
            const int maxSeats = 4;
            const long maxSeatId = 5L;

            //mocks for GetLayout in Infrastructure
            _mockingHelper.RepositoryInfrastructure
                .Setup(_ => _.GetActiveSeatsWithRowAndCategoryAsync(It.IsAny<long>()))
                .ReturnsAsync(CreateSeats);

            _mockingHelper.RepositoryInfrastructure
                .Setup(_ => _.GetCinemaHallByIdAsync(It.IsAny<long>()))
                .ReturnsAsync(CreateCinemaHalls().First());

            _mockingHelper.RepositoryTicket
                .Setup(_ => _.GetFreeSeatsAsync(It.IsAny<Schedule>()))
                .ReturnsAsync(CreateSeats().Where(s => !new List<long> { 19L, 21L }.Contains(s.Id)));

            //mocks for User (maxSeatReservations)
            _mockingHelper.RepositoryUser
                .Setup(_ => _.GetUserWithAllReferencesByIdAsync(It.IsAny<long>()))
                .ReturnsAsync(CreateUserWithMaxReservations(maxSeats));


            _mockingHelper.RepositorySchedule.Setup(_ => _.GetScheduleByIdAsync(ScheduleA.Id)).ReturnsAsync(ScheduleA);
            _mockingHelper.RepositoryUser.Setup(_ => _.UserExistAsync(1L)).ReturnsAsync(true);
            _mockingHelper.RepositoryTicket.Setup(_ =>
                    _.AddReservationAsync(It.IsAny<IEnumerable<Seat>>(), It.IsAny<long>(), It.IsAny<long>()))
                .ReturnsAsync(1L);
            _mockingHelper.RepositoryTicket.Setup(_ => _.GetReservationByIdAsync(1L))
                .ReturnsAsync(Reservations.First());
            foreach (var seat in CreateSeats())
            {
                _mockingHelper.RepositoryInfrastructure.Setup(_ => _.GetSeatByIdAsync(seat.Id)).ReturnsAsync(seat);
            }

            var desiredSeats = CreateSeats().Where(s => s.Id <= maxSeatId).Select(Map).ToList();
            Func<Task<ReservationDto>> result = async () => await _reservationService.AddReservationAsync(desiredSeats, ScheduleA.Id, 1L);
            await result.Should().ThrowExactlyAsync<ArgumentOutOfRangeException>();
        }

        [Test]
        public async Task Test_AddReservation_BlockRuleViolation_ShouldThrowSeatValidationException()
        {
            const int maxSeats = 5;

            //mocks for GetLayout in Infrastructure
            _mockingHelper.RepositoryInfrastructure
                .Setup(_ => _.GetActiveSeatsWithRowAndCategoryAsync(It.IsAny<long>()))
                .ReturnsAsync(CreateSeats);

            _mockingHelper.RepositoryInfrastructure
                .Setup(_ => _.GetCinemaHallByIdAsync(It.IsAny<long>()))
                .ReturnsAsync(CreateCinemaHalls().First());

            _mockingHelper.RepositoryTicket
                .Setup(_ => _.GetFreeSeatsAsync(It.IsAny<Schedule>()))
                .ReturnsAsync(CreateSeats().Where(s => !new List<long> { 19L, 21L }.Contains(s.Id)));

            //mocks for User (maxSeatReservations)
            _mockingHelper.RepositoryUser
                .Setup(_ => _.GetUserWithAllReferencesByIdAsync(It.IsAny<long>()))
                .ReturnsAsync(CreateUserWithMaxReservations(maxSeats));


            _mockingHelper.RepositorySchedule.Setup(_ => _.GetScheduleByIdAsync(ScheduleA.Id)).ReturnsAsync(ScheduleA);
            _mockingHelper.RepositoryUser.Setup(_ => _.UserExistAsync(1L)).ReturnsAsync(true);
            _mockingHelper.RepositoryTicket.Setup(_ =>
                    _.AddReservationAsync(It.IsAny<IEnumerable<Seat>>(), It.IsAny<long>(), It.IsAny<long>()))
                .ReturnsAsync(1L);
            _mockingHelper.RepositoryTicket.Setup(_ => _.GetReservationByIdAsync(1L))
                .ReturnsAsync(Reservations.First());
            foreach (var seat in CreateSeats())
            {
                _mockingHelper.RepositoryInfrastructure.Setup(_ => _.GetSeatByIdAsync(seat.Id)).ReturnsAsync(seat);
            }

            var desiredSeats = CreateSeats().Where(s => s.Id % 2L == 0).Take(4).Select(Map).ToList();
            Func<Task<ReservationDto>> result = async () => await _reservationService.AddReservationAsync(desiredSeats, ScheduleA.Id, 1L);
            await result.Should().ThrowExactlyAsync<SeatValidationException>();
        }

        private static User CreateUserWithMaxReservations(int i)
        {
            return new User()
            {
                Id = 1L,
                Address = new Address
                {
                    Id = 1L,
                    City = new City()
                    {
                        Id = 1L,
                        Name = "Hagenberg",
                        PostalCode = "4232"
                    },
                    Number = 12,
                    Street = "Softwarepark-Straße",
                    CityId = 1L
                },
                Role = new Role()
                {
                    Id = 1L,
                    MaxReservations = i,
                    Label = "Terminal-User"
                },
                AddressId = 1L,
                Email = "support@apollo.com",
                FirstName = "Terminal",
                LastName = "User",
                Phone = "00669/12346161234",
                RoleId = 1L
            };
        }

        [Test]
        public async Task Test_DeleteReservation_InvalidIdList_ShouldThrow()
        {
            Func<Task<bool>> callback = () => _reservationService.DeleteReservationsAsync((IEnumerable<long>)null);
            await callback.Should().ThrowAsync<ArgumentException>();
            _mockingHelper.RepositoryTicket.Verify(_ => _.DeleteReservationAsync(It.IsAny<long>()), Times.Never);

            callback = () => _reservationService.DeleteReservationsAsync(Enumerable.Empty<long>());
            await callback.Should().ThrowAsync<ArgumentException>();
            _mockingHelper.RepositoryTicket.Verify(_ => _.DeleteReservationAsync(It.IsAny<long>()), Times.Never);
        }


        [Test]
        public async Task Test_DeleteReservation_InvalidId_ShouldThrow()
        {
            _mockingHelper.RepositoryTicket.Setup(_ => _.DeleteReservationAsync(1L))
                .ThrowsAsync(new PersistenceException(""));
            _mockingHelper.RepositoryTicket.Setup(_ => _.GetSeatReservationsByReservationIdAsync(It.IsAny<long>()))
                .ReturnsAsync(SeatReservations);
            _mockingHelper.RepositoryTicket.Setup(_ => _.DeleteSeatReservationAsync(It.IsAny<long>()))
                .ReturnsAsync(true);

            Func<Task<bool>> callback = () => _reservationService.DeleteReservationAsync(1L);
            await callback.Should().ThrowAsync<PersistenceException>();
            _mockingHelper.RepositoryTicket.Verify(_ => _.DeleteReservationAsync(It.IsAny<long>()), Times.Once);
        }

        [Test]
        public async Task Test_DeleteReservation_NoSeatReservationsFound_ShouldReturnFalse()
        {
            _mockingHelper.RepositoryTicket.Setup(_ => _.GetSeatReservationsByReservationIdAsync(It.IsAny<long>()))
                .ReturnsAsync(new List<SeatReservation>());

            var result = await _reservationService.DeleteReservationAsync(1L);
            result.Should().BeFalse();
        }

        [Test]
        public async Task Test_DeleteReservations_NoSeatReservationsFound_ShouldReturnFalse()
        {
            _mockingHelper.RepositoryTicket.Setup(_ => _.GetSeatReservationsByReservationIdAsync(It.IsAny<long>()))
                .ReturnsAsync(new List<SeatReservation>());

            var reservations = Reservations.Select(Map);
            var result = await _reservationService.DeleteReservationsAsync(reservations);
            result.Should().BeFalse();
        }

        [Test]
        public async Task Test_DeleteReservationsByReservation_NoSeatReservationsFound_ShouldReturnFalse()
        {
            _mockingHelper.RepositoryTicket.Setup(_ => _.GetSeatReservationsByReservationIdAsync(It.IsAny<long>()))
                .ReturnsAsync(new List<SeatReservation>());

            var reservation = Reservations.Select(Map).First();
            var result = await _reservationService.DeleteReservationAsync(reservation);
            result.Should().BeFalse();
        }
        [Test]
        public async Task Test_DeleteReservationsByReservationIds_NoSeatReservationsFound_ShouldReturnFalse()
        {
            _mockingHelper.RepositoryTicket.Setup(_ => _.GetSeatReservationsByReservationIdAsync(It.IsAny<long>()))
                .ReturnsAsync(new List<SeatReservation>());

            var reservationIds = Reservations.Select(r => r.Id);
            var result = await _reservationService.DeleteReservationsAsync(reservationIds);
            result.Should().BeFalse();
        }

        [Test]
        public async Task Test_DeleteReservation_InvalidObjectList_ShouldThrow()
        {
            _mockingHelper.RepositoryTicket.Setup(_ => _.DeleteReservationAsync(1L))
                .ThrowsAsync(new PersistenceException(""));
            _mockingHelper.RepositoryTicket.Setup(_ => _.GetSeatReservationsByIds(It.IsAny<IEnumerable<long>>()))
                .ReturnsAsync(SeatReservations);
            _mockingHelper.RepositoryTicket.Setup(_ => _.DeleteSeatReservationAsync(It.IsAny<long>()))
                .ReturnsAsync(true);

            Func<Task<bool>> callback = () =>
                _reservationService.DeleteReservationsAsync((IEnumerable<ReservationDto>)null);
            await callback.Should().ThrowAsync<ArgumentException>();
            _mockingHelper.RepositoryTicket.Verify(_ => _.DeleteReservationAsync(It.IsAny<long>()), Times.Never);

            callback = () => _reservationService.DeleteReservationsAsync(Enumerable.Empty<ReservationDto>());
            await callback.Should().ThrowAsync<ArgumentException>();
            _mockingHelper.RepositoryTicket.Verify(_ => _.DeleteReservationAsync(It.IsAny<long>()), Times.Never);

            callback = () =>
                _reservationService.DeleteReservationsAsync(new[] { new ReservationDto { Id = 1L } });
            await callback.Should().ThrowAsync<PersistenceException>();
            _mockingHelper.RepositoryTicket.Verify(_ => _.DeleteReservationAsync(It.IsAny<long>()), Times.Once);
        }

        [Test]
        public async Task Test_DeleteReservation_InvalidObject_ShouldThrow()
        {
            _mockingHelper.RepositoryTicket.Setup(_ => _.DeleteReservationAsync(1L))
                .ThrowsAsync(new PersistenceException(""));
            _mockingHelper.RepositoryTicket.Setup(_ => _.GetSeatReservationsByReservationIdAsync(It.IsAny<long>()))
                .ReturnsAsync(SeatReservations);
            _mockingHelper.RepositoryTicket.Setup(_ => _.DeleteSeatReservationAsync(It.IsAny<long>()))
                .ReturnsAsync(true);

            Func<Task<bool>> callback = () => _reservationService.DeleteReservationAsync((ReservationDto)null);
            await callback.Should().ThrowAsync<ArgumentNullException>();
            _mockingHelper.RepositoryTicket.Verify(_ => _.DeleteReservationAsync(It.IsAny<long>()), Times.Never);

            callback = () => _reservationService.DeleteReservationAsync(new ReservationDto { Id = 1L });
            await callback.Should().ThrowAsync<PersistenceException>();
            _mockingHelper.RepositoryTicket.Verify(_ => _.DeleteReservationAsync(It.IsAny<long>()), Times.Once);
        }

        [Test]
        public async Task Test_DeleteReservation_IdList_Should_ReturnTrue()
        {
            _mockingHelper.RepositoryTicket.Setup(_ => _.DeleteReservationAsync(1L)).ReturnsAsync(1);
            _mockingHelper.RepositoryTicket.Setup(_ => _.GetSeatReservationsByIds(It.IsAny<long[]>()))
                .ReturnsAsync(SeatReservations);
            _mockingHelper.RepositoryTicket.Setup(_ => _.DeleteSeatReservationAsync(It.IsAny<long>()))
                .ReturnsAsync(true);

            var result = await _reservationService.DeleteReservationsAsync(new[] { 1L });

            result.Should().BeTrue();
            _mockingHelper.RepositoryTicket.Verify(_ => _.DeleteReservationAsync(1L), Times.Once);
            _mockingHelper.RepositoryTicket.Verify(_ => _.DeleteSeatReservationAsync(It.IsAny<long>()),
                Times.Exactly(SeatReservations.Count));
        }

        [Test]
        public async Task Test_DeleteReservation_Id_Should_ReturnTrue()
        {
            _mockingHelper.RepositoryTicket.Setup(_ => _.DeleteReservationAsync(1L)).ReturnsAsync(1);
            _mockingHelper.RepositoryTicket.Setup(_ => _.GetSeatReservationsByReservationIdAsync(It.IsAny<long>()))
                .ReturnsAsync(SeatReservations);
            _mockingHelper.RepositoryTicket.Setup(_ => _.DeleteSeatReservationAsync(It.IsAny<long>()))
                .ReturnsAsync(true);

            var result = await _reservationService.DeleteReservationAsync(1L);

            result.Should().BeTrue();
            _mockingHelper.RepositoryTicket.Verify(_ => _.DeleteReservationAsync(1L), Times.Once);
            _mockingHelper.RepositoryTicket.Verify(_ => _.DeleteSeatReservationAsync(It.IsAny<long>()),
                Times.Exactly(SeatReservations.Count));
        }

        [Test]
        public async Task Test_DeleteReservation_Object_Should_ReturnTrue()
        {
            _mockingHelper.RepositoryTicket.Setup(_ => _.DeleteReservationAsync(1L)).ReturnsAsync(1);
            _mockingHelper.RepositoryTicket.Setup(_ => _.GetSeatReservationsByReservationIdAsync(It.IsAny<long>()))
                .ReturnsAsync(SeatReservations);
            _mockingHelper.RepositoryTicket.Setup(_ => _.DeleteSeatReservationAsync(It.IsAny<long>()))
                .ReturnsAsync(true);


            var result = await _reservationService.DeleteReservationAsync(new ReservationDto { Id = 1L });

            result.Should().BeTrue();
            _mockingHelper.RepositoryTicket.Verify(_ => _.DeleteReservationAsync(1L), Times.Once);
            _mockingHelper.RepositoryTicket.Verify(_ => _.DeleteSeatReservationAsync(It.IsAny<long>()),
                Times.Exactly(SeatReservations.Count));
        }

        [Test]
        public async Task Test_DeleteReservation_ObjectList_Should_ReturnTrue()
        {
            _mockingHelper.RepositoryTicket.Setup(_ => _.DeleteReservationAsync(1L)).ReturnsAsync(1);
            _mockingHelper.RepositoryTicket.Setup(_ => _.GetSeatReservationsByIds(It.IsAny<IEnumerable<long>>()))
                .ReturnsAsync(SeatReservations);
            _mockingHelper.RepositoryTicket.Setup(_ => _.DeleteSeatReservationAsync(It.IsAny<long>()))
                .ReturnsAsync(true);

            var result = await _reservationService.DeleteReservationsAsync(new[] { new ReservationDto { Id = 1L } });

            result.Should().BeTrue();
            _mockingHelper.RepositoryTicket.Verify(_ => _.DeleteReservationAsync(1L), Times.Once);
            _mockingHelper.RepositoryTicket.Verify(_ => _.DeleteSeatReservationAsync(It.IsAny<long>()),
                Times.Exactly(SeatReservations.Count));
        }

        [Test]
        public async Task Test_GetTickets_NoTickets_ShouldReturn_EmptyList()
        {
            _mockingHelper.RepositoryTicket.Setup(_ => _.GetTicketsAsync()).ReturnsAsync(Enumerable.Empty<Ticket>());
            var result = (await _reservationService.GetTicketsAsync()).ToList();
            result.Should().BeEmpty();
        }

        [Test]
        public async Task Test_GetTickets_ShouldReturn_Objects()
        {
            _mockingHelper.RepositoryTicket.Setup(_ => _.GetTicketsAsync()).ReturnsAsync(Tickets);

            var result = (await _reservationService.GetTicketsAsync()).ToList();

            result.Should().HaveCount(Tickets.Count);
            for (var i = 0; i < Tickets.Count; i++)
            {
                var entry = result[i];
                entry.Id.Should().Be(Tickets[i].Id);
                entry.Printed.Should().Be(Tickets[i].Printed);
            }

            _mockingHelper.RepositoryTicket.Verify(_ => _.GetTicketsAsync(), Times.Once);
        }

        [Test]
        public async Task Test_GetTicketsByUserId_NoTickets_ShouldReturn_EmptyList()
        {
            _mockingHelper.RepositoryTicket.Setup(_ => _.GetTicketsByUserIdAsync(It.IsAny<long>()))
                .ReturnsAsync(Enumerable.Empty<Ticket>());
            var result = (await _reservationService.GetTicketsAsync()).ToList();
            result.Should().BeEmpty();
        }

        [Test]
        public async Task Test_GetTicketsByUserId_ShouldReturn_Objects()
        {
            _mockingHelper.RepositoryTicket.Setup(_ => _.GetTicketsByUserIdAsync(It.IsAny<long>()))
                .ReturnsAsync(Tickets);

            var result = (await _reservationService.GetTicketsByUserIdAsync(1L)).ToList();

            result.Should().HaveCount(Tickets.Count);
            for (var i = 0; i < Tickets.Count; i++)
            {
                var entry = result[i];
                entry.Id.Should().Be(Tickets[i].Id);
                entry.Printed.Should().Be(Tickets[i].Printed);
            }

            _mockingHelper.RepositoryTicket.Verify(_ => _.GetTicketsByUserIdAsync(It.IsAny<long>()), Times.Once);
        }

        [Test]
        public async Task Test_GetSeatReservations_NoReservations_ShouldReturn_EmptyList()
        {
            _mockingHelper.RepositoryTicket.Setup(_ => _.GetReservationsAsync())
                .ReturnsAsync(Enumerable.Empty<Reservation>());
            var result = (await _reservationService.GetReservationsAsync()).ToList();
            result.Should().BeEmpty();
        }

        [Test]
        public async Task Test_GetReservations_ShouldReturn_Objects()
        {
            _mockingHelper.RepositoryTicket.Setup(_ => _.GetReservationsAsync()).ReturnsAsync(Reservations);

            var result = (await _reservationService.GetReservationsAsync()).ToList();

            result.Should().HaveCount(Reservations.Count);
            for (var i = 0; i < Reservations.Count; i++)
            {
                var entry = result[i];
                entry.Id.Should().Be(Reservations[i].Id);
                entry.ScheduleId.Should().Be(Reservations[i].ScheduleId);
                entry.UserId.Should().Be(Reservations[i].UserId);
                entry.TicketId.Should().Be(Reservations[i].TicketId);
            }

            _mockingHelper.RepositoryTicket.Verify(_ => _.GetReservationsAsync(), Times.Once);
        }

        [Test]
        public async Task Test_GetSeatReservationsByUserId_NoReservations_ShouldReturn_EmptyList()
        {
            _mockingHelper.RepositoryTicket.Setup(_ => _.GetReservationsByUserIdAsync(1L))
                .ReturnsAsync(Enumerable.Empty<Reservation>());
            var result = (await _reservationService.GetSeatReservationsByUserIdAsync(1L)).ToList();
            result.Should().BeEmpty();
        }

        [Test]
        public async Task Test_GetSeatReservationsByUserId_ShouldReturn_List()
        {
            _mockingHelper.RepositoryTicket.Setup(_ => _.GetReservationsByUserIdAsync(1L))
                .ReturnsAsync(Reservations);
            _mockingHelper.RepositoryTicket.Setup(_ => _.GetSeatReservationsByIds(It.IsAny<IEnumerable<long>>()))
                .ReturnsAsync(SeatReservations);

            var result = (await _reservationService.GetSeatReservationsByUserIdAsync(1L)).ToList();

            result.Should().HaveCount(SeatReservations.Count);
            for (var i = 0; i < Reservations.Count; i++)
            {
                var entry = result[i];
                entry.Id.Should().Be(SeatReservations[i].Id);
                entry.ReservationId.Should().Be(SeatReservations[i].ReservationId);
                entry.SeatId.Should().Be(SeatReservations[i].SeatId);
            }

            _mockingHelper.RepositoryTicket.Verify(_ => _.GetSeatReservationsByIds(It.IsAny<IEnumerable<long>>()), Times.Once);
        }

        [Test]
        public async Task Test_GetReservationsByUserId_ShouldReturn_Objects()
        {
            _mockingHelper.RepositoryTicket.Setup(_ => _.GetReservationsByUserIdAsync(1L)).ReturnsAsync(Reservations);

            var result = (await _reservationService.GetReservationsByUserIdAsync(1L)).ToList();

            result.Should().HaveCount(Reservations.Count);
            for (var i = 0; i < Reservations.Count; i++)
            {
                var entry = result[i];
                entry.Id.Should().Be(Reservations[i].Id);
                entry.ScheduleId.Should().Be(Reservations[i].ScheduleId);
                entry.UserId.Should().Be(Reservations[i].UserId);
                entry.TicketId.Should().Be(Reservations[i].TicketId);
            }

            _mockingHelper.RepositoryTicket.Verify(_ => _.GetReservationsByUserIdAsync(1L), Times.Once);
        }

        [Test]
        public async Task Test_GetSeatReservationsAsync_NoEntries_ShouldReturn_EmptyList()
        {
            _mockingHelper.RepositoryTicket.Setup(_ => _.GetSeatReservationsAsync())
                .ReturnsAsync(Enumerable.Empty<SeatReservation>());
            var result = (await _reservationService.GetSeatReservationsAsync()).ToList();
            result.Should().BeEmpty();
        }

        [Test]
        public async Task Test_GetSeatReservationsAsync_ShouldReturn_List()
        {
            _mockingHelper.RepositoryTicket.Setup(_ => _.GetSeatReservationsAsync()).ReturnsAsync(SeatReservations);

            var result = (await _reservationService.GetSeatReservationsAsync()).ToList();

            result.Should().HaveCount(SeatReservations.Count);
            for (int i = 0; i < SeatReservations.Count; i++)
            {
                var entry = result[i];
                entry.ReservationId.Should().Be(SeatReservations[i].ReservationId);
                entry.SeatId.Should().Be(SeatReservations[i].SeatId);
            }

            _mockingHelper.RepositoryTicket.Verify(_ => _.GetSeatReservationsAsync(), Times.Once);
        }

        [Test]
        public async Task Test_GetReservationById_InvalidId_ShouldThrow_Exception()
        {
            _mockingHelper.RepositoryTicket.Setup(_ => _.GetReservationByIdAsync(1L))
                .ThrowsAsync(new PersistenceException("Invalid id"));

            Func<Task> callback = async () => await _reservationService.GetReservationByIdAsync(1L);
            await callback.Should().ThrowAsync<PersistenceException>();
        }

        [Test]
        public async Task Test_GetReservationById_ValidId_ShouldReturn_Object()
        {
            var reservation = Reservations.First();
            _mockingHelper.RepositoryTicket.Setup(_ => _.GetReservationByIdAsync(reservation.Id))
                .ReturnsAsync(reservation);

            var result = await _reservationService.GetReservationByIdAsync(1L);

            result.Should().NotBeNull();
            result.Id.Should().Be(reservation.Id);
            result.ScheduleId.Should().Be(reservation.ScheduleId);
            result.TicketId.Should().Be(reservation.TicketId);
            _mockingHelper.RepositoryTicket.Verify(_ => _.GetReservationByIdAsync(reservation.Id), Times.Once);
        }

        private static readonly Schedule ScheduleA = new Schedule
        {
            Id = 1L,
            CinemaHallId = 2L,
            MovieId = 3L,
            StartTime = DateTime.UtcNow.AddDays(1),
            Price = new decimal(10.75)
        };

        private static readonly Schedule ScheduleB = new Schedule
        {
            Id = 2L,
            CinemaHallId = 2L,
            MovieId = 3L,
            StartTime = DateTime.UtcNow.AddDays(1),
            Price = new decimal(10.75)
        };


        private static readonly List<Reservation> Reservations = new List<Reservation>
        {
            new Reservation
            {
                Id = 1L,
                ScheduleId = 2L,
                TicketId = 3L,
                UserId = 4L
            },
            new Reservation
            {
                Id = 1L,
                ScheduleId = 2L,
                TicketId = 3L,
                UserId = 4L
            },
            new Reservation
            {
                Id = 1L,
                ScheduleId = 2L,
                UserId = 4L
            }
        };

        private static readonly List<SeatReservation> SeatReservations = new List<SeatReservation>
        {
            new SeatReservation
            {
                Id = 1L,
                ReservationId = 1L,
                SeatId = 1L
            },
            new SeatReservation
            {
                Id = 2L,
                ReservationId = 1L,
                SeatId = 2L
            },
            new SeatReservation
            {
                Id = 3L,
                ReservationId = 1L,
                SeatId = 3L
            }
        };

        private static readonly List<Ticket> Tickets = new List<Ticket>
        {
            new Ticket
            {
                Id = 1L,
                Printed = DateTime.UtcNow.AddDays(1)
            },
            new Ticket
            {
                Id = 2L,
                Printed = DateTime.UtcNow.AddDays(2)
            },
            new Ticket
            {
                Id = 3L,
                Printed = DateTime.UtcNow.AddDays(3)
            }
        };

        private static IEnumerable<Seat> CreateSeats()
        {
            return new List<Seat>
            {
                // seat at (0,0) and (1, 0) are missing -> therefore they should be null in layout
                new Seat {Id = 1L,  Number = 3, LayoutColumn = 2, LayoutRow = 0, Locked = false, RowId = 1L, Row = new Row { Id = 1L, Number = 1, CategoryId = 1L, CinemaHallId = 1L, CinemaHall =   new CinemaHall {Id = 1L, Label = "Apollo 1"} , Category = new RowCategory {Id = 1L,  Name = "Silver", PriceFactor = 1.0}}},
                new Seat {Id = 2L,  Number = 4, LayoutColumn = 3, LayoutRow = 0, Locked = false, RowId = 1L, Row = new Row { Id = 1L, Number = 1, CategoryId = 1L, CinemaHallId = 1L, CinemaHall =   new CinemaHall {Id = 1L, Label = "Apollo 1"} , Category = new RowCategory {Id = 1L,  Name = "Silver", PriceFactor = 1.0}}},
                new Seat {Id = 3L,  Number = 5, LayoutColumn = 4, LayoutRow = 0, Locked = false, RowId = 1L, Row = new Row { Id = 1L, Number = 1, CategoryId = 1L, CinemaHallId = 1L , CinemaHall =   new CinemaHall {Id = 1L, Label = "Apollo 1"}, Category = new RowCategory {Id = 1L,  Name = "Silver", PriceFactor = 1.0}}},
                new Seat {Id = 4L,  Number = 6, LayoutColumn = 5, LayoutRow = 0, Locked = false, RowId = 1L, Row = new Row { Id = 1L, Number = 1, CategoryId = 1L, CinemaHallId = 1L, CinemaHall =   new CinemaHall {Id = 1L, Label = "Apollo 1"} , Category = new RowCategory {Id = 1L,  Name = "Silver", PriceFactor = 1.0}}},
                new Seat {Id = 5L,  Number = 7, LayoutColumn = 6, LayoutRow = 0, Locked = false, RowId = 1L, Row = new Row { Id = 1L, Number = 1, CategoryId = 1L, CinemaHallId = 1L , CinemaHall =   new CinemaHall {Id = 1L, Label = "Apollo 1"}, Category = new RowCategory {Id = 1L,  Name = "Silver", PriceFactor = 1.0}}},
                new Seat {Id = 6L,  Number = 8, LayoutColumn = 7, LayoutRow = 0, Locked = false, RowId = 1L, Row = new Row { Id = 1L, Number = 1, CategoryId = 1L, CinemaHallId = 1L , CinemaHall =   new CinemaHall {Id = 1L, Label = "Apollo 1"}, Category = new RowCategory {Id = 1L,  Name = "Silver", PriceFactor = 1.0}}},

                new Seat {Id = 7L,  Number = 9, LayoutColumn = 0, LayoutRow = 1, Locked = false, RowId = 2L, Row = new Row { Id = 2L, Number = 2, CategoryId = 2L, CinemaHallId = 1L, CinemaHall =   new CinemaHall {Id = 1L, Label = "Apollo 1"} , Category = new RowCategory {Id = 2L,  Name = "Gold", PriceFactor = 1.25}}},
                new Seat {Id = 8L , Number = 10, LayoutColumn = 1, LayoutRow = 1, Locked = false, RowId = 2L, Row = new Row { Id = 2L, Number = 2, CategoryId = 2L, CinemaHallId = 1L , CinemaHall =   new CinemaHall {Id = 1L, Label = "Apollo 1"}, Category = new RowCategory {Id = 2L,  Name = "Gold", PriceFactor = 1.25}}},
                new Seat {Id = 9L,  Number = 11, LayoutColumn = 2, LayoutRow = 1, Locked = false, RowId = 2L, Row = new Row { Id = 2L, Number = 2, CategoryId = 2L, CinemaHallId = 1L , CinemaHall =   new CinemaHall {Id = 1L, Label = "Apollo 1"}, Category = new RowCategory {Id = 2L,  Name = "Gold", PriceFactor = 1.25}}},
                new Seat {Id = 10L,  Number = 12, LayoutColumn = 3, LayoutRow = 1, Locked = false, RowId = 2L, Row = new Row { Id = 2L, Number = 2, CategoryId = 2L, CinemaHallId = 1L, CinemaHall =   new CinemaHall {Id = 1L, Label = "Apollo 1"} , Category = new RowCategory {Id = 2L,  Name = "Gold", PriceFactor = 1.25}}},
                new Seat {Id = 11L,  Number = 13, LayoutColumn = 4, LayoutRow = 1, Locked = false, RowId = 2L, Row = new Row { Id = 2L, Number = 2, CategoryId = 2L, CinemaHallId = 1L , CinemaHall =   new CinemaHall {Id = 1L, Label = "Apollo 1"}, Category = new RowCategory {Id = 2L,  Name = "Gold", PriceFactor = 1.25}}},
                new Seat {Id = 12L,  Number = 14, LayoutColumn = 5, LayoutRow = 1, Locked = false, RowId = 2L, Row = new Row { Id = 2L, Number = 2, CategoryId = 2L, CinemaHallId = 1L , CinemaHall =   new CinemaHall {Id = 1L, Label = "Apollo 1"}, Category = new RowCategory {Id = 2L,  Name = "Gold", PriceFactor = 1.25}}},
                new Seat {Id = 13L,  Number = 15, LayoutColumn = 6, LayoutRow = 1, Locked = false, RowId = 2L, Row = new Row { Id = 2L, Number = 2, CategoryId = 2L, CinemaHallId = 1L , CinemaHall =   new CinemaHall {Id = 1L, Label = "Apollo 1"}, Category = new RowCategory {Id = 2L,  Name = "Gold", PriceFactor = 1.25}}},
                new Seat {Id = 14L,  Number = 16, LayoutColumn = 7, LayoutRow = 1, Locked = false, RowId = 2L, Row = new Row { Id = 2L, Number = 2, CategoryId = 2L, CinemaHallId = 1L , CinemaHall =   new CinemaHall {Id = 1L, Label = "Apollo 1"}, Category = new RowCategory {Id = 2L,  Name = "Gold", PriceFactor = 1.25}}},

                new Seat {Id = 15L,  Number = 20, LayoutColumn = 0, LayoutRow = 2, Locked = false, RowId = 3L, Row = new Row { Id = 3L, Number = 3, CategoryId = 4L, CinemaHallId = 1L , CinemaHall =   new CinemaHall {Id = 1L, Label = "Apollo 1"}, Category =  new RowCategory {Id = 4L,  Name = "Diamond", PriceFactor = 2.0}}},
                new Seat {Id = 16L , Number = 21, LayoutColumn = 1, LayoutRow = 2, Locked = false, RowId = 3L, Row = new Row { Id = 3L, Number = 3, CategoryId = 4L, CinemaHallId = 1L , CinemaHall =   new CinemaHall {Id = 1L, Label = "Apollo 1"}, Category =  new RowCategory {Id = 4L,  Name = "Diamond", PriceFactor = 2.0}}},
                new Seat {Id = 17L,  Number = 22, LayoutColumn = 2, LayoutRow = 2, Locked = false, RowId = 3L, Row = new Row { Id = 3L, Number = 3, CategoryId = 4L, CinemaHallId = 1L, CinemaHall =   new CinemaHall {Id = 1L, Label = "Apollo 1"} , Category =  new RowCategory {Id = 4L,  Name = "Diamond", PriceFactor = 2.0}}},
                new Seat {Id = 18L,  Number = 23, LayoutColumn = 3, LayoutRow = 2, Locked = true, RowId = 3L, Row = new Row { Id = 3L, Number = 3, CategoryId = 4L, CinemaHallId = 1L , CinemaHall =   new CinemaHall {Id = 1L, Label = "Apollo 1"}, Category =  new RowCategory {Id = 4L,  Name = "Diamond", PriceFactor = 2.0}}},
                new Seat {Id = 19L,  Number = 24, LayoutColumn = 4, LayoutRow = 2, Locked = false, RowId = 3L, Row = new Row { Id = 3L, Number = 3, CategoryId = 4L, CinemaHallId = 1L, CinemaHall =   new CinemaHall {Id = 1L, Label = "Apollo 1"} , Category =  new RowCategory {Id = 4L,  Name = "Diamond", PriceFactor = 2.0}}},
                new Seat {Id = 20L,  Number = 25, LayoutColumn = 5, LayoutRow = 2, Locked = true, RowId = 3L, Row = new Row { Id = 3L, Number = 3, CategoryId = 4L, CinemaHallId = 1L, CinemaHall =   new CinemaHall {Id = 1L, Label = "Apollo 1"} , Category =  new RowCategory {Id = 4L,  Name = "Diamond", PriceFactor = 2.0}}},
                new Seat {Id = 21L,  Number = 26, LayoutColumn = 6, LayoutRow = 2, Locked = false, RowId = 3L, Row = new Row { Id = 3L, Number = 3, CategoryId = 4L, CinemaHallId = 1L , CinemaHall =   new CinemaHall {Id = 1L, Label = "Apollo 1"}, Category =  new RowCategory {Id = 4L,  Name = "Diamond", PriceFactor = 2.0}}},
                new Seat {Id = 22L,  Number = 27, LayoutColumn = 7, LayoutRow = 2, Locked = true, RowId = 3L, Row = new Row { Id = 3L, Number = 3, CategoryId = 4L, CinemaHallId = 1L, CinemaHall =   new CinemaHall {Id = 1L, Label = "Apollo 1"} , Category =  new RowCategory {Id = 4L,  Name = "Diamond", PriceFactor = 2.0}}},
            };
        }

        private static IEnumerable<CinemaHall> CreateCinemaHalls()
        {
            return new List<CinemaHall>
            {
                new CinemaHall {Id = 1L, Label = "Apollo 1", SizeColumn = 8, SizeRow = 3},
                new CinemaHall {Id = 2L, Label = "Apollo 2", SizeColumn = 20, SizeRow = 20}
            };
        }

        private void PrepareReservationMocks(int maxSeats, Schedule scheduleA, Schedule scheduleB,
            List<SeatDto> desiredSeats)
        {
            //mocks for GetLayout in Infrastructure
            _mockingHelper.RepositoryInfrastructure
                .Setup(_ => _.GetActiveSeatsWithRowAndCategoryAsync(It.IsAny<long>()))
                .ReturnsAsync(CreateSeats);

            _mockingHelper.RepositoryInfrastructure
                .Setup(_ => _.GetCinemaHallByIdAsync(It.IsAny<long>()))
                .ReturnsAsync(CreateCinemaHalls().First());

            var seats = CreateSeats().Where(s => !new List<long> { 19L, 21L }.Contains(s.Id)).ToList();
            var reservedSeats = seats.Where(s => !desiredSeats.Select(ds => ds.Id).Contains(s.Id)).ToList();


            _mockingHelper.RepositoryTicket
                .Setup(_ => _.GetFreeSeatsAsync(scheduleA))
                .ReturnsAsync(seats);


            _mockingHelper.RepositoryTicket
                .Setup(_ => _.GetFreeSeatsAsync(scheduleB))
                .ReturnsAsync(reservedSeats);

            //mocks for User (maxSeatReservations)
            _mockingHelper.RepositoryUser
                .Setup(_ => _.GetUserWithAllReferencesByIdAsync(It.IsAny<long>()))
                .ReturnsAsync(CreateUserWithMaxReservations(maxSeats));


            _mockingHelper.RepositorySchedule.Setup(_ => _.GetScheduleByIdAsync(ScheduleA.Id)).ReturnsAsync(ScheduleA);
            _mockingHelper.RepositorySchedule.Setup(_ => _.GetScheduleByIdAsync(ScheduleB.Id)).ReturnsAsync(ScheduleB);

            _mockingHelper.RepositoryUser.Setup(_ => _.UserExistAsync(It.IsAny<long>())).ReturnsAsync(true);
            _mockingHelper.RepositoryTicket.Setup(_ =>
                    _.AddReservationAsync(It.IsAny<IEnumerable<Seat>>(), It.IsAny<long>(), It.IsAny<long>()))
                .ReturnsAsync(1L);
            _mockingHelper.RepositoryTicket.Setup(_ => _.GetReservationByIdAsync(1L))
                .ReturnsAsync(Reservations.First());
            foreach (var seat in CreateSeats())
            {
                _mockingHelper.RepositoryInfrastructure.Setup(_ => _.GetSeatByIdAsync(seat.Id)).ReturnsAsync(seat);
            }
        }

    }
}