using System;
using System.ComponentModel.DataAnnotations;

namespace Apollo.Core.Dto
{
    public class AddressDto : BaseDto
    {
        [Range(1L, long.MaxValue, ErrorMessage = "Id must between one and maximum value of long")]
        public long CityId { get; set; }

        [Required(AllowEmptyStrings = false)]
        public string Street { get; set; }

        [Range(1, 999)]
        public int Number { get; set; }

        [Required(AllowEmptyStrings = false)]
        [RegularExpression("^\\d{4,5}$")] //For DE, AT and S
        public string PostalCode { get; set; }

        [Required]
        [MinLength(4, ErrorMessage = "City must ")]
        public string City { get; set; }

        public DateTime CityRowVersion { get; set; }
    }
}
