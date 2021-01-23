using System.ComponentModel.DataAnnotations;
using Apollo.Core.Dto.ValidationAttributes;

namespace Apollo.Core.Dto
{
    public class SeatDto : BaseDto
    {
        [Required]
        [Range(1L, long.MaxValue, ErrorMessage = "Id must between one and maximum value of long")]
        public long RowId { get; set; }

        [MatchId(nameof(RowId), ErrorMessage = "RowId and Row.Id must be equal")]
        public RowDto Row { get; set; }

        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "Number must between one and int.MaxValue")]
        public int Number { get; set; }

        public SeatState State { get; set; }

        [Required]
        [Range(0, int.MaxValue, ErrorMessage = "LayoutColumn must between zero and int.MaxValue")]
        public int LayoutColumn { get; set; }

        [Required]
        [Range(0, int.MaxValue, ErrorMessage = "LayoutRow must between zero and int.MaxValue")]
        public int LayoutRow { get; set; }
    }

}
