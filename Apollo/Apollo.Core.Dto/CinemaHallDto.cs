using System.ComponentModel.DataAnnotations;

namespace Apollo.Core.Dto
{
    public class CinemaHallDto : BaseDto
    {
        [Required(AllowEmptyStrings = false)]
        [MinLength(3, ErrorMessage = "Label must have at least 3 characters")]
        public string Label { get; set; }

        [Required(AllowEmptyStrings = false)]
        [Range(1, int.MaxValue, ErrorMessage = "SizeColumn must be between 1 and int.MaxValue")]
        public int SizeColumn { get; set; }

        [Required(AllowEmptyStrings = false)]
        [Range(1, int.MaxValue, ErrorMessage = "SizeRow must be between 1 and int.MaxValue")]
        public int SizeRow { get; set; }
    }
}
