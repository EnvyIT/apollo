using System;
using System.Collections.Generic;
using System.Linq;
using Apollo.Core.Dto;
using Apollo.Util;
using Apollo.Util.Logger;

namespace Apollo.Core.Validation
{
    public class SeatBlockValidation : ISeatValidation
    {
        private static readonly IApolloLogger<SeatBlockValidation> Logger = LoggerFactory.CreateLogger<SeatBlockValidation>();

        public bool IsValid(IEnumerable<SeatDto> seatLayout, IEnumerable<SeatDto> desiredSeats)
        {
            ValidationHelper.ValidateEnumerableIsNotNullOrEmpty(Logger.Here(), seatLayout, nameof(seatLayout));
            ValidationHelper.ValidateEnumerableIsNotNullOrEmpty(Logger.Here(), desiredSeats, nameof(desiredSeats));

            var seatsForReservation = desiredSeats.ToList();
            var lowestSeatNo = seatsForReservation.Min(s => s.Number);
            var highestSeatNo = seatsForReservation.Max(s => s.Number);

            return Math.Abs(highestSeatNo - lowestSeatNo) + 1 == seatsForReservation.Count();
        }
    }
}
