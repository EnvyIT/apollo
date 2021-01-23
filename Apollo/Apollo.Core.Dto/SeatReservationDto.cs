using System.ComponentModel.DataAnnotations;
using Apollo.Core.Dto.ValidationAttributes;

namespace Apollo.Core.Dto
{
    public class SeatReservationDto : BaseDto
    {
        [Required]
        [Range(1L, long.MaxValue, ErrorMessage = "Id must between one and maximum value of long")]
        public long ReservationId { get; set; }

        [MatchId(nameof(ReservationId), ErrorMessage = "ReservationId and Reservation.Id must be equal")]
        public ReservationDto Reservation { get; set; }

        [Required]
        [Range(1L, long.MaxValue, ErrorMessage = "Id must between one and maximum value of long")]
        public long SeatId { get; set; }

        [MatchId(nameof(SeatId), ErrorMessage = "SeatId and SeatId.Id must be equal")]
        public SeatDto Seat { get; set; }
    }
}
