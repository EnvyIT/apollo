using System.ComponentModel.DataAnnotations;

namespace Apollo.Core.Dto
{
    public class RoleDto : BaseDto
    {
        [Required(AllowEmptyStrings = false)]
        [MinLength(3, ErrorMessage = "Label must have at least 3 characters")]
        public string Label { get; set; }

        [Required]
        [Range(1, 50)]
        public int MaxReservations { get; set; }
    }
}
