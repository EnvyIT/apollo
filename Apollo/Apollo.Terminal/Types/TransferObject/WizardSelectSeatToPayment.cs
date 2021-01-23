using System.Collections.Generic;
using Apollo.Core.Dto;

namespace Apollo.Terminal.Types.TransferObject
{
    public class WizardSelectSeatToPayment
    {
        public ReservationDto Reservation { get; }
        public IEnumerable<SeatDto> Seats { get; }

        public WizardSelectSeatToPayment(ReservationDto reservation, IEnumerable<SeatDto> seats)
        {
            Reservation = reservation;
            Seats = seats;
        }
    }
}
