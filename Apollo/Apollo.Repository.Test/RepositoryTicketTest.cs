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
    public class RepositoryTicketTest
    {
        private RepositoryHelper _repositoryHelper;

        [SetUp]
        public async Task Setup()
        {
            _repositoryHelper = new RepositoryHelper();
            _repositoryHelper.FillTypes.AddAll(new List<EntityType>
            {
                EntityType.Seat, 
                EntityType.Schedule,
                EntityType.User,
                EntityType.Reservation,
                EntityType.SeatReservation
            });
            await _repositoryHelper.SeedDatabase();
        }

        [TearDown]
        public async Task TearDown()
        {
            await _repositoryHelper.TearDown();
        }

        [Test]
        public async Task CreateReservation_ShouldReturnReservationId()
        {
            var repositoryTicket = _repositoryHelper.RepositoryFactory.CreateRepositoryTicket();
            var repositoryInfrastructure = _repositoryHelper.RepositoryFactory.CreateRepositoryInfrastructure();
            var repositorySchedule = _repositoryHelper.RepositoryFactory.CreateRepositorySchedule();
            var repositoryUser = _repositoryHelper.RepositoryFactory.CreateRepositoryUser();

            var categories = await repositoryInfrastructure.GetActiveRowCategoriesAsync();
            var category = categories.Single(c => c.Name == "Gold");

            var seats = await repositoryInfrastructure.GetActiveSeatsFromCinemaHallByCategoryAsync(1L, category.Id);
            var seatGroup = seats.OrderBy(s => s.Number).Take(2);

            var schedule = (await repositorySchedule.GetStartTimeAsync(new DateTime(2020, 11, 4, 20, 15, 00))).First();

            var user = (await repositoryUser.GetUsersAsync()).First();


            var reservationId = await repositoryTicket.AddReservationAsync(seatGroup, schedule.Id, user.Id);
            reservationId.Should().NotBe(0L);
        }

        [Test]
        public async Task CreateReservationWithoutSeats_ShouldThrowArgumentNullException()
        {
            var repositoryTicket = _repositoryHelper.RepositoryFactory.CreateRepositoryTicket();
            var repositorySchedule = _repositoryHelper.RepositoryFactory.CreateRepositorySchedule();
            var repositoryUser = _repositoryHelper.RepositoryFactory.CreateRepositoryUser();

            var schedule = (await repositorySchedule.GetStartTimeAsync(new DateTime(2020, 11, 4, 20, 15, 00))).First();
            var user = (await repositoryUser.GetUsersAsync()).First();

            Func<Task<long>> addReservation = async () => await repositoryTicket.AddReservationAsync(new List<Seat>(), schedule.Id, user.Id);

            await addReservation.Should().ThrowExactlyAsync<ArgumentException>();
        }

        [Test]
        public async Task GetAllReservations_ShouldReturn0Reservations()
        {
            var repositoryTicket = _repositoryHelper.RepositoryFactory.CreateRepositoryTicket();
            var reservations = await repositoryTicket.GetReservationsAsync();
            reservations.Should().HaveCount(1);
        }

        [Test]
        public async Task AddReservation_ShouldReturn5SeatReservations1TicketAnd2Reservation()
        {

            var repositoryTicket = _repositoryHelper.RepositoryFactory.CreateRepositoryTicket();
            var repositoryInfrastructure = _repositoryHelper.RepositoryFactory.CreateRepositoryInfrastructure();
            var repositorySchedule = _repositoryHelper.RepositoryFactory.CreateRepositorySchedule();
            var repositoryUser = _repositoryHelper.RepositoryFactory.CreateRepositoryUser();

            var categories = await repositoryInfrastructure.GetActiveRowCategoriesAsync();
            var category = categories.Single(c => c.Name == "Gold");

            var seats = await repositoryInfrastructure.GetActiveSeatsFromCinemaHallByCategoryAsync(1L, category.Id);
            var seatGroup = seats.OrderBy(s => s.Number).Take(2);

            var schedule = (await repositorySchedule.GetStartTimeAsync(new DateTime(2020, 11, 4, 20, 15, 00))).First();

            var user = (await repositoryUser.GetUsersAsync()).First();


            var reservationId = await repositoryTicket.AddReservationAsync(seatGroup, schedule.Id, user.Id);
            reservationId.Should().NotBe(0L);

            var ticketId = await repositoryTicket.AddTicketAsync(reservationId);
            ticketId.Should().NotBe(0L);

            var reservations = await repositoryTicket.GetReservationsAsync();
            var tickets = await repositoryTicket.GetTicketsAsync();
            var seatReservations = await repositoryTicket.GetSeatReservationsAsync();
            reservations.Should().HaveCount(1 + _repositoryHelper.Reservations.Count());
            seatReservations.Should().HaveCount(2 + _repositoryHelper.SeatReservations.Count());
            tickets.Should().HaveCount(1);
        }

        [Test]
        public async Task DeleteAllReservations_ShouldReturnReservationsAfterDeletion()
        {
            var repositoryTicket = _repositoryHelper.RepositoryFactory.CreateRepositoryTicket();
            var repositoryInfrastructure = _repositoryHelper.RepositoryFactory.CreateRepositoryInfrastructure();
            var repositorySchedule = _repositoryHelper.RepositoryFactory.CreateRepositorySchedule();
            var repositoryUser = _repositoryHelper.RepositoryFactory.CreateRepositoryUser();

            var categories = await repositoryInfrastructure.GetActiveRowCategoriesAsync();
            var category = categories.Single(c => c.Name == "Gold");

            var seats = await repositoryInfrastructure.GetActiveSeatsFromCinemaHallByCategoryAsync(1L, category.Id);
            var seatGroup = seats.OrderBy(s => s.Number).Take(2);

            var schedule = (await repositorySchedule.GetStartTimeAsync(new DateTime(2020, 11, 4, 20, 15, 00))).First();

            var user = (await repositoryUser.GetUsersAsync()).First();


            var reservationId = await repositoryTicket.AddReservationAsync(seatGroup, schedule.Id, user.Id);
            reservationId.Should().NotBe(0L);

            var ticketId = await repositoryTicket.AddTicketAsync(reservationId);
            ticketId.Should().NotBe(0L);

            var reservation = await repositoryTicket.GetReservationByIdAsync(reservationId);

            await repositoryTicket.DeleteReservationAsync(reservation);

            var reservations = await repositoryTicket.GetReservationsAsync();
            var tickets = await repositoryTicket.GetTicketsAsync();
            var seatReservations = await repositoryTicket.GetSeatReservationsAsync();
            reservations.Should().HaveCount(1);
            seatReservations.Should().HaveCount(3);
            tickets.Should().HaveCount(1);

        }
        [Test]
        public async Task DeleteAllReservationsById_ShouldReturn3ReservationsAfterDeletion()
        {
            var repositoryTicket = _repositoryHelper.RepositoryFactory.CreateRepositoryTicket();
            var repositoryInfrastructure = _repositoryHelper.RepositoryFactory.CreateRepositoryInfrastructure();
            var repositorySchedule = _repositoryHelper.RepositoryFactory.CreateRepositorySchedule();
            var repositoryUser = _repositoryHelper.RepositoryFactory.CreateRepositoryUser();

            var categories = await repositoryInfrastructure.GetActiveRowCategoriesAsync();
            var category = categories.Single(c => c.Name == "Gold");

            var seats = await repositoryInfrastructure.GetActiveSeatsFromCinemaHallByCategoryAsync(1L, category.Id);
            var seatGroup = seats.OrderBy(s => s.Number).Take(2);

            var schedule = (await repositorySchedule.GetStartTimeAsync(new DateTime(2020, 11, 4, 20, 15, 00))).First();

            var user = (await repositoryUser.GetUsersAsync()).First();


            var reservationId = await repositoryTicket.AddReservationAsync(seatGroup, schedule.Id, user.Id);
            reservationId.Should().NotBe(0L);

            await repositoryTicket.DeleteReservationAsync(reservationId);

            var reservations = await repositoryTicket.GetReservationsAsync();
            var seatReservations = await repositoryTicket.GetSeatReservationsAsync();
            reservations.Should().HaveCount(1);
            seatReservations.Should().HaveCount(3);
        }

        [Test]
        public async Task DeleteAllReservationsNonExistingIds_ShouldThrowInvalidEntityIdException()
        {
            var repositoryTicket = _repositoryHelper.RepositoryFactory.CreateRepositoryTicket();
            const long reservationId = 1134L;
            
            Func<Task<int>> deleteReservation = async () => await repositoryTicket.DeleteReservationAsync(reservationId);
            await deleteReservation.Should().ThrowExactlyAsync<InvalidEntityIdException>();
        }

        [Test]
        public async Task DeleteAllEmptyCollection_ShouldThrowArgumentException()
        {
            var repositoryTicket = _repositoryHelper.RepositoryFactory.CreateRepositoryTicket();
            var reservations = new List<Reservation>();

            Func<Task<int>> deleteReservation = async () => await repositoryTicket.DeleteReservationsAsync(reservations);
            await deleteReservation.Should().ThrowExactlyAsync<ArgumentException>();
        }

        [Test]
        public async Task DeleteReservation_ShouldReturn1ReservationAfterDeletion()
        {
            var repositoryTicket = _repositoryHelper.RepositoryFactory.CreateRepositoryTicket();
            var repositoryInfrastructure = _repositoryHelper.RepositoryFactory.CreateRepositoryInfrastructure();
            var repositorySchedule = _repositoryHelper.RepositoryFactory.CreateRepositorySchedule();
            var repositoryUser = _repositoryHelper.RepositoryFactory.CreateRepositoryUser();

            var categories = await repositoryInfrastructure.GetActiveRowCategoriesAsync();
            var category = categories.Single(c => c.Name == "Gold");

            var seats = await repositoryInfrastructure.GetActiveSeatsFromCinemaHallByCategoryAsync(1L, category.Id);
          

            var schedule = (await repositorySchedule.GetStartTimeAsync(new DateTime(2020, 11, 4, 20, 15, 00))).First();

            var user = (await repositoryUser.GetUsersAsync()).First();
            var orderedSeats = seats.OrderBy(s => s.Number).ToArray();
            
            for (var i = 0; i < (orderedSeats.Length/2); ++i)
            {
                var index = i * 2;
                var seatGroup = new List<Seat> {orderedSeats[index], orderedSeats[index + 1]};
                await repositoryTicket.AddReservationAsync(seatGroup, schedule.Id, user.Id);
            }

            var reservations = await repositoryTicket.GetReservationsAsync();
            reservations.Should().HaveCount(5);

            var reservationsToDelete = reservations.Take(3).ToList();
            
            await repositoryTicket.DeleteReservationsAsync(reservationsToDelete); 
            reservations = await repositoryTicket.GetReservationsAsync();
            reservations.Should().HaveCount(2);
        }

        
       
        [Test]
        public async Task CreateTicketWithNonExistingReservationId_ShouldThrowInvalidEntityIdException()
        {
            var repositoryTicket = _repositoryHelper.RepositoryFactory.CreateRepositoryTicket();
            Func<Task<long>> addTicket = async () => await repositoryTicket.AddTicketAsync(999L);
            await addTicket.Should().ThrowExactlyAsync<InvalidEntityIdException>();
        }

        [Test]
        public async Task CreateTicketWithReservationId_ShouldReturnTicketId()
        {
            var repositoryInfrastructure = _repositoryHelper.RepositoryFactory.CreateRepositoryInfrastructure();
            var repositorySchedule = _repositoryHelper.RepositoryFactory.CreateRepositorySchedule();
            var repositoryUser = _repositoryHelper.RepositoryFactory.CreateRepositoryUser();
            var repositoryTicket = _repositoryHelper.RepositoryFactory.CreateRepositoryTicket();

            var categories = await repositoryInfrastructure.GetActiveRowCategoriesAsync();
            var category = categories.Single(c => c.Name == "Gold");

            var seats = await repositoryInfrastructure.GetActiveSeatsFromCinemaHallByCategoryAsync(1L, category.Id);
            var seatGroup = seats.OrderBy(s => s.Number).Take(2);

            var schedule = (await repositorySchedule.GetStartTimeAsync(new DateTime(2020, 11, 4, 20, 15, 00))).First();

            var user = (await repositoryUser.GetUsersAsync()).First();


            var reservationId = await repositoryTicket.AddReservationAsync(seatGroup, schedule.Id, user.Id);
            var ticketId= await repositoryTicket.AddTicketAsync(reservationId);
            ticketId.Should().NotBe(0L);
            var tickets = await repositoryTicket.GetTicketsAsync();
            tickets.Should().HaveCount(1);
            tickets.ElementAt(0).Printed.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(10));
        }

        [Test]
        public async Task CreateTicketWithReservation_ShouldReturnTicketId()
        {
            var repositoryInfrastructure = _repositoryHelper.RepositoryFactory.CreateRepositoryInfrastructure();
            var repositorySchedule = _repositoryHelper.RepositoryFactory.CreateRepositorySchedule();
            var repositoryUser = _repositoryHelper.RepositoryFactory.CreateRepositoryUser();
            var repositoryTicket = _repositoryHelper.RepositoryFactory.CreateRepositoryTicket();

            var categories = await repositoryInfrastructure.GetActiveRowCategoriesAsync();
            var category = categories.Single(c => c.Name == "Gold");

            var seats = await repositoryInfrastructure.GetActiveSeatsFromCinemaHallByCategoryAsync(1L, category.Id);
            var seatGroup = seats.OrderBy(s => s.Number).Take(2);

            var schedule = (await repositorySchedule.GetStartTimeAsync(new DateTime(2020, 11, 4, 20, 15, 00))).First();

            var user = (await repositoryUser.GetUsersAsync()).First();


            var reservationId = await repositoryTicket.AddReservationAsync(seatGroup, schedule.Id, user.Id);
            var reservation = await repositoryTicket.GetReservationByIdAsync(reservationId);

            var ticketId = await repositoryTicket.AddTicketAsync(reservation);
            ticketId.Should().NotBe(0L);
            var tickets = await repositoryTicket.GetTicketsAsync();
            tickets.Should().HaveCount(1);
            tickets.ElementAt(0).Printed.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(10));
        }

        [Test]
        public async Task CreateTicketWithReservationNull_ShouldThrowArgumentNullException()
        {
            var repositoryTicket = _repositoryHelper.RepositoryFactory.CreateRepositoryTicket();
            Func<Task<long>> addTicket = async () => await repositoryTicket.AddTicketAsync(null);
            await addTicket.Should().ThrowExactlyAsync<ArgumentNullException>();
        }

        [Test]
        public async Task GetTicketsAsync_ShouldReturn0Tickets()
        {
            var repositoryTicket = _repositoryHelper.RepositoryFactory.CreateRepositoryTicket();
            var tickets = await repositoryTicket.GetTicketsAsync();
            tickets.Should().HaveCount(0);
        }

    
        [Test]
        public async Task DeleteTicket_ShouldReturn3Tickets()
        {
            var repositoryTicket = _repositoryHelper.RepositoryFactory.CreateRepositoryTicket();
            var repositoryInfrastructure = _repositoryHelper.RepositoryFactory.CreateRepositoryInfrastructure();
            var repositorySchedule = _repositoryHelper.RepositoryFactory.CreateRepositorySchedule();
            var repositoryUser = _repositoryHelper.RepositoryFactory.CreateRepositoryUser();

            var categories = await repositoryInfrastructure.GetActiveRowCategoriesAsync();
            var category = categories.Single(c => c.Name == "Gold");

            var seats = await repositoryInfrastructure.GetActiveSeatsFromCinemaHallByCategoryAsync(1L, category.Id);


            var schedule = (await repositorySchedule.GetStartTimeAsync(new DateTime(2020, 11, 4, 20, 15, 00))).First();

            var user = (await repositoryUser.GetUsersAsync()).First();
            var orderedSeats = seats.OrderBy(s => s.Number).ToArray();

            for (var i = 0; i < (orderedSeats.Length / 2); ++i)
            {
                var index = i * 2;
                var seatGroup = new List<Seat> { orderedSeats[index], orderedSeats[index + 1] };
                await repositoryTicket.AddReservationAsync(seatGroup, schedule.Id, user.Id);
            }

            var reservations = await repositoryTicket.GetReservationsAsync();
            reservations.Should().HaveCount(5);

            foreach (var reservation in reservations)
            {
                await repositoryTicket.AddTicketAsync(reservation.Id);
            }
            var tickets = await repositoryTicket.GetTicketsAsync();
            tickets.Should().HaveCount(5);

            var ticketA = tickets.ElementAt(0);
            var ticketB = tickets.ElementAt(1);

            var deletedA = await repositoryTicket.DeleteTicketAsync(ticketA);
            deletedA.Should().Be(1);
            var deletedB = await repositoryTicket.DeleteTicketAsync(ticketB.Id);
            deletedB.Should().Be(1);
            tickets = await repositoryTicket.GetTicketsAsync();
            tickets.Should().HaveCount(3);
        }

        [Test]
        public async Task DeleteTicketWithNull_ShouldThrowArgumentNullException()
        {
            var repositoryTicket = _repositoryHelper.RepositoryFactory.CreateRepositoryTicket();
            Func<Task<long>> deleteTicket = async () => await repositoryTicket.DeleteTicketAsync(null);
            await deleteTicket.Should().ThrowExactlyAsync<ArgumentNullException>();
        }

        [Test]
        public async Task DeleteTicketWithNonExistingId_ShouldThrowInvalidEntityIdException()
        {
            var repositoryTicket = _repositoryHelper.RepositoryFactory.CreateRepositoryTicket();
            Func<Task<long>> deleteTicket = async () => await repositoryTicket.DeleteTicketAsync(1337L);
            await deleteTicket.Should().ThrowExactlyAsync<InvalidEntityIdException>();
        }

        [Test]
        public async Task GetFreeSeats_ShouldReturn18FreeSeats()
        {
            var repositoryTicket = _repositoryHelper.RepositoryFactory.CreateRepositoryTicket();
            var repositorySchedule = _repositoryHelper.RepositoryFactory.CreateRepositorySchedule();
            var schedule = (await repositorySchedule.GetSchedulesAsync()).First();
            var freeSeats = await repositoryTicket.GetFreeSeatsAsync(schedule);
            freeSeats.Should().HaveCount(18);
        }

        [Test]
        public async Task GetFreeSeats_ShouldReturn16SeatsFree()
        {
            var repositoryTicket = _repositoryHelper.RepositoryFactory.CreateRepositoryTicket();
            var repositorySchedule = _repositoryHelper.RepositoryFactory.CreateRepositorySchedule();
            var repositoryInfrastructure = _repositoryHelper.RepositoryFactory.CreateRepositoryInfrastructure();
            var repositoryUser = _repositoryHelper.RepositoryFactory.CreateRepositoryUser();

            var categories = await repositoryInfrastructure.GetActiveRowCategoriesAsync();
            var category = categories.Single(c => c.Name == "Gold");

            var seats = await repositoryInfrastructure.GetActiveSeatsFromCinemaHallByCategoryAsync(1L, category.Id);

            var schedule = (await repositorySchedule.GetSchedulesAsync()).First();

            var user = (await repositoryUser.GetUsersAsync()).First();
            var orderedSeats = seats.OrderBy(s => s.Number).ToArray();

            var seatGroup = new List<Seat> { orderedSeats[0], orderedSeats[1] };
            await repositoryTicket.AddReservationAsync(seatGroup, schedule.Id, user.Id);

            var reservations = await repositoryTicket.GetReservationsAsync();
            reservations.Should().HaveCount(2);

            var freeSeats = await repositoryTicket.GetFreeSeatsAsync(schedule);
            freeSeats.Should().HaveCount(16); // 22 free seats - 3 are locked - 3 are already reserved - so there are only 19 free seats in total
        }

        [Test]
        public async Task GetSeatsWithRowAndCategoryById_InvalidId_Should_ThrowException()
        {
            var repositoryTicket = _repositoryHelper.RepositoryFactory.CreateRepositoryTicket();
            Func<Task> createSchedules = async () => await repositoryTicket.GetSeatsWithRowAndCategoryByIdAsync(500L);
            await createSchedules.Should().ThrowAsync<InvalidEntityIdException>();
        }

        [Test]
        public async Task GetSeatsWithRowAndCategoryById_ShouldReturn_Entitities_WithReferences()
        {
            var repositoryTicket = _repositoryHelper.RepositoryFactory.CreateRepositoryTicket();
            var repositorySchedule = _repositoryHelper.RepositoryFactory.CreateRepositorySchedule();
            var repositoryInfrastructure = _repositoryHelper.RepositoryFactory.CreateRepositoryInfrastructure();
            var repositoryUser = _repositoryHelper.RepositoryFactory.CreateRepositoryUser();

            var categories = await repositoryInfrastructure.GetActiveRowCategoriesAsync();
            var category = categories.Single(c => c.Name == "Gold");

            var seats = await repositoryInfrastructure.GetActiveSeatsFromCinemaHallByCategoryAsync(1L, category.Id);

            var schedule = (await repositorySchedule.GetSchedulesAsync()).First();

            var user = (await repositoryUser.GetUsersAsync()).First();
            var orderedSeats = seats.OrderBy(s => s.Number).ToArray();

            var seatGroup = new List<Seat> { orderedSeats[0], orderedSeats[1], orderedSeats[2] };
            var reservationId = await repositoryTicket.AddReservationAsync(seatGroup, schedule.Id, user.Id);

            var resultSeats = (await repositoryTicket.GetSeatsWithRowAndCategoryByIdAsync(reservationId)).ToList();

            resultSeats.Should().HaveCount(3);
            for (var i = 0; i < resultSeats.Count; i++)
            {
                resultSeats[i].Id.Should().Be(orderedSeats[i].Id);
                resultSeats[i].Row.Id.Should().Be(orderedSeats[i].Row.Id);
                resultSeats[i].Row.Category.Id.Should().Be(orderedSeats[i].Row.Category.Id);
            }
        }

        [Test]
        public async Task GetSeatReservationsByIds_ShouldReturnsSeatReservations()
        {
            var repositoryTicket = _repositoryHelper.RepositoryFactory.CreateRepositoryTicket();
            var seatReservations =  (await repositoryTicket.GetSeatReservationsByIds(new List<long>{1L}))
                .OrderBy(s => s.Id)
                .ToList();
            seatReservations.Should().HaveCount(3);
            seatReservations[0].Id.Should().Be(1L);
            seatReservations[1].Id.Should().Be(2L);
            seatReservations[2].Id.Should().Be(3L);
        }

        [Test]
        public async Task GetSeatReservationsWithReservationId_ShouldReturnSeatReservations()
        {
            var repositoryTicket = _repositoryHelper.RepositoryFactory.CreateRepositoryTicket();
            var seatReservations =  (await repositoryTicket.GetSeatReservationsByReservationIdAsync(1L)).ToList();
            seatReservations.Should().HaveCount(3);
            seatReservations[0].Id.Should().Be(1L);
            seatReservations[1].Id.Should().Be(2L);
            seatReservations[2].Id.Should().Be(3L);
        }

        [Test]
        public async Task GetTicketsByUserId_ShouldReturnTickets()
        {
            var repositoryTicket = _repositoryHelper.RepositoryFactory.CreateRepositoryTicket();
            await repositoryTicket.AddTicketAsync(1L);
            var tickets = (await repositoryTicket.GetTicketsByUserIdAsync(
                _repositoryHelper.Users.First().Id)).ToList();
            tickets.Should().HaveCount(1);
            tickets[0].Should().NotBeNull();
        }

        [Test]
        public async Task GetReservationsByUserId_ShouldReturnReservations()
        {
            var repositoryTicket = _repositoryHelper.RepositoryFactory.CreateRepositoryTicket();
            var reservations =  await repositoryTicket.GetReservationsByUserIdAsync(
                _repositoryHelper.Users.First().Id);
            reservations.Should().HaveCount(1);
        }

        [Test]
        public async Task DeleteSeatReservation_ShouldDeleteSeatReservations()
        {
            var repositoryTicket = _repositoryHelper.RepositoryFactory.CreateRepositoryTicket();
            var count = (await repositoryTicket.GetSeatReservationsAsync()).Count();
            await repositoryTicket.DeleteSeatReservationAsync(3L);
            var reservations = await repositoryTicket.GetSeatReservationsAsync();
            reservations.Should().HaveCount(count - 1);
        }

        [Test]
        public async Task GetFreeSeatsAsync_ShouldDeleteSeatReservations()
        {
            var repositoryTicket = _repositoryHelper.RepositoryFactory.CreateRepositoryTicket();
            var repositorySchedule = _repositoryHelper.RepositoryFactory.CreateRepositorySchedule();
            var schedule = await repositorySchedule.GetScheduleByIdAsync(6L);
            var freeSeats = await repositoryTicket.GetFreeSeatsAsync(schedule);
            freeSeats.Should().HaveCount(0);
        }
    }
}
