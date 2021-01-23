
using System.ComponentModel.DataAnnotations;
using Apollo.Core.Dto.ValidationAttributes;

namespace Apollo.Core.Dto
{
    public class UserDto : BaseDto
    {
        public string Uuid { get; set; }

        [Required(AllowEmptyStrings = false)]
        [MinLength(3, ErrorMessage = "Title must have at least 3 characters")]
        public string FirstName { get; set; }

        [Required(AllowEmptyStrings = false)]
        [MinLength(3, ErrorMessage = "Title must have at least 3 characters")]
        public string LastName { get; set; }

        [EmailAddress(ErrorMessage = "Email not valid")]
        public string Email { get; set; }

        [Phone(ErrorMessage = "Phone number not valid")]
        public string Phone { get; set; }

        [Required]
        [Range(1L, long.MaxValue, ErrorMessage = "Id must between one and maximum value of long")]
        public long RoleId { get; set; }

        [MatchId(nameof(RoleId), ErrorMessage = "RoleId and Role.Id must be equal")]
        public RoleDto Role { get; set; }

        public AddressDto Address { get; set; }
    }
}
