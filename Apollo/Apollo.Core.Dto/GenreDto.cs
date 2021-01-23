using System.ComponentModel.DataAnnotations;

namespace Apollo.Core.Dto
{
    public class GenreDto : BaseDto
    {
        [Required(AllowEmptyStrings = false)]
        [MinLength(3, ErrorMessage = "Name must have at least 3 characters")]
        public string Name { get; set; }
    }
}
