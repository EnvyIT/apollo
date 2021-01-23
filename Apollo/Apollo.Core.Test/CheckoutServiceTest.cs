using System;
using System.Threading.Tasks;
using Apollo.Core.Dto;
using Apollo.Core.Interfaces;
using Apollo.Core.Types;
using Apollo.Domain.Entity;
using Apollo.Payment.Domain;
using FluentAssertions;
using Moq;
using NUnit.Framework;

namespace Apollo.Core.Test
{
    public class CheckoutServiceTest
    {
        private MockingHelper _mockingHelper;
        private ICheckoutService _checkoutService;

        const long ReservationId = 1L;
        const long ScheduleId = 2L;
        const long CinemaHallId = 4L;
        const long TicketId = 9L;
        private static readonly decimal Price = new decimal(12.5);
        const double PriceFactor = 1.5;

        private static readonly ApolloCreditCard CreditCard = new ApolloCreditCard
        {
            CardNumber = "12345678",
            Cvc = "123",
            Expiration = DateTime.UtcNow,
            OwnerName = "Test",
            OwnerBirthday = DateTime.UtcNow
        };

        private static readonly Reservation Reservation = new Reservation
            {Id = ReservationId, ScheduleId = ScheduleId, UserId = 3L};

        private static readonly Schedule Schedule = new Schedule
        {
            Id = ScheduleId,
            CinemaHallId = CinemaHallId,
            MovieId = 5L,
            Price = Price,
            StartTime = DateTime.UtcNow
        };

        private static readonly RowCategory Category = new RowCategory
            {Id = 6L, Name = "Premium", PriceFactor = PriceFactor};

        private static readonly Row Row = new Row
            {Id = 7L, Number = 1, Category = Category, CinemaHallId = CinemaHallId};

        private static readonly Seat[] Seats =
        {
            new Seat {Id = 10L, Locked = false, LayoutRow = 0, LayoutColumn = 0, Number = 1, Row = Row},
            new Seat {Id = 11L, Locked = false, LayoutRow = 0, LayoutColumn = 2, Number = 1, Row = Row},
            new Seat {Id = 12L, Locked = false, LayoutRow = 0, LayoutColumn = 4, Number = 1, Row = Row}
        };

        private static readonly Ticket Ticket = new Ticket{Id = 5L, Deleted = false, Printed = DateTime.UtcNow};

        [SetUp]
        public void Setup()
        {
            _mockingHelper = new MockingHelper();
            _checkoutService = _mockingHelper.ServiceFactory.Object.CreateCheckoutService();
        }

        private void SetupDataLoading()
        {
            _mockingHelper.RepositoryTicket.Setup(_ => _.GetReservationByIdAsync(ReservationId))
                .ReturnsAsync(Reservation);
            _mockingHelper.RepositorySchedule.Setup(_ => _.GetScheduleByIdAsync(ScheduleId))
                .ReturnsAsync(Schedule);
            _mockingHelper.RepositoryTicket.Setup(_ => _.GetSeatsWithRowAndCategoryByIdAsync(ReservationId))
                .ReturnsAsync(Seats);
        }

        [Test]
        public async Task Test_Checkout_Success()
        {
            SetupDataLoading();
            _mockingHelper.RepositoryTicket.Setup(_ => _.AddTicketAsync(Reservation)).ReturnsAsync(TicketId);
            _mockingHelper.RepositoryTicket.Setup(_ => _.GetTicketByIdAsync(It.IsAny<long>())).ReturnsAsync(Ticket);

            var ticket = await _checkoutService.PayTicketAsync(ReservationId, PaymentType.FhPay, CreditCard);

            ticket.Should().NotBeNull();
            ticket.Id.Should().Be(Ticket.Id);
            ticket.Printed.Should().Be(Ticket.Printed);
            var expectedPrice = Seats.Length * Price * (decimal) PriceFactor;
            _mockingHelper.RepositoryTicket.Verify(_ => _.AddTicketAsync(Reservation), Times.Once);
            _mockingHelper.PaymentMock.Amount.Should().Be(expectedPrice);
            _mockingHelper.FluentTransactionCommit.Verify(_ => _.Commit(), Times.Once);
        }

        [Test]
        public async Task Test_Checkout_CreateTicketFailed_NotPaymentCall()
        {
            SetupDataLoading();
            _mockingHelper.RepositoryTicket.Setup(_ => _.AddTicketAsync(Reservation))
                .Throws<InvalidOperationException>();

            Func<Task> call = async () =>
                await _checkoutService.PayTicketAsync(ReservationId, PaymentType.FhPay, CreditCard);

            await call.Should().ThrowAsync<InvalidOperationException>();
            _mockingHelper.RepositoryTicket.Verify(_ => _.AddTicketAsync(Reservation), Times.Once);
            _mockingHelper.FluentTransactionCommit.Verify(_ => _.Commit(), Times.Never);
        }
    }
}