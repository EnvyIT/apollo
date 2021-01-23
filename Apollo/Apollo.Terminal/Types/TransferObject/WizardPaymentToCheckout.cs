using System.Collections.Generic;
using Apollo.Core.Dto;

namespace Apollo.Terminal.Types.TransferObject
{
    public class WizardPaymentToCheckout
    {
        public ReservationDto Reservation { get; }

        public TicketDto Ticket { get; }

        public IEnumerable<SeatDto> Seats { get; }

        public decimal TotalPrice { get; }

        public WizardPaymentToCheckout(ReservationDto reservation, TicketDto ticket,
            IEnumerable<SeatDto> seats, decimal totalPrice)
        {
            Reservation = reservation;
            Ticket = ticket;
            Seats = seats;
            TotalPrice = totalPrice;
        }
    }
}