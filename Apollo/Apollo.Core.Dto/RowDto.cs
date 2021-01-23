using System.ComponentModel.DataAnnotations;
using Apollo.Core.Dto.ValidationAttributes;

namespace Apollo.Core.Dto
{
    public class RowDto : BaseDto
    {
        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "Number must be between 1 and int.MaxValue")]
        public int Number { get; set; }

        [Required]
        [Range(1L, long.MaxValue, ErrorMessage = "Id must between one and maximum value of long")]
        public long CategoryId { get; set; }

        [MatchId(nameof(CategoryId), ErrorMessage = "CategoryId and Category.Id must be equal")]
        public RowCategoryDto Category { get; set; }

        [Required]
        [Range(1L, long.MaxValue, ErrorMessage = "Id must between one and maximum value of long")]
        public long CinemaHallId { get; set; }

        [MatchId(nameof(CinemaHallId), ErrorMessage = "CinemaHallId and CinemaHall.Id must be equal")]
        public CinemaHallDto CinemaHall { get; set; }
    }
}
