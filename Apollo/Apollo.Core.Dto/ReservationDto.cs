using System.ComponentModel.DataAnnotations;
using Apollo.Core.Dto.ValidationAttributes;

namespace Apollo.Core.Dto
{
    public class ReservationDto : BaseDto
    {
        [Range(1L, long.MaxValue, ErrorMessage = "Id must between one and maximum value of long")]
        public long ScheduleId { get; set; }

        [MatchId(nameof(ScheduleId), ErrorMessage = "ScheduleId and Schedule.Id must be equal")]
        public ScheduleDto Schedule { get; set; }

        [Range(1L, long.MaxValue, ErrorMessage = "Id must between one and maximum value of long")]
        public long UserId { get; set; }

        [MatchId(nameof(UserId), ErrorMessage = "UserId and User.Id must be equal")]
        public UserDto User { get; set; }

        [MatchId(nameof(TicketId), ErrorMessage = "TicketId and Ticket.Id must be equal")]
        public TicketDto Ticket { get; set; }

        [Range(1L, long.MaxValue, ErrorMessage = "Id must between one and maximum value of long")]
        public long TicketId { get; set; }
    }
}
