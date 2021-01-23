using System.Collections.Generic;
using System.Linq;
using Apollo.Core.Dto;
using Apollo.Util;
using Apollo.Util.Logger;

namespace Apollo.Core.Validation
{
    public class SeatAvailableValidation : ISeatValidation
    {
        private static readonly IApolloLogger<SeatAvailableValidation> Logger = LoggerFactory.CreateLogger<SeatAvailableValidation>();
        public bool IsValid(IEnumerable<SeatDto> seatLayout, IEnumerable<SeatDto> desiredSeats)
        {
            ValidationHelper.ValidateEnumerableIsNotNullOrEmpty(Logger.Here(), seatLayout, nameof(seatLayout));
            ValidationHelper.ValidateEnumerableIsNotNullOrEmpty(Logger.Here(), desiredSeats, nameof(desiredSeats));
            var desiredSeatIds = desiredSeats.Select(ds => ds.Id).ToList();
            var seats = seatLayout.Where(s => desiredSeatIds.Contains(s.Id))
                .ToList();
            return seats.Count() == desiredSeatIds.Count() && seats.All(s => s.State == SeatState.Free);
        }
    }
}
