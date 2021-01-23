using System.Collections.Generic;
using Apollo.Core.Dto;

namespace Apollo.Core.Validation
{
    public interface ISeatValidation
    {
        bool IsValid(IEnumerable<SeatDto> seatLayout, IEnumerable<SeatDto> desiredSeats);
    }
}
