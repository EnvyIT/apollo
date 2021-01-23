using System.ComponentModel.DataAnnotations;

namespace Apollo.Core.Dto
{
    public class RowCategoryDto : BaseDto
    {
        [Required(AllowEmptyStrings = false)]
        [MinLength(3, ErrorMessage = "Title must have at least 3 characters")]
        public string Name { get; set; }

        [Required]
        [Range(1.0, 2.0)] // price range from 100% up to a maximum of 200% possible
        public double PriceFactor { get; set; }
    }
}
