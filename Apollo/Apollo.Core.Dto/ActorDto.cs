using System.ComponentModel.DataAnnotations;

namespace Apollo.Core.Dto
{
    public class ActorDto : BaseDto
    {
        [Required(AllowEmptyStrings = false)]
        [MinLength(3, ErrorMessage = "FirstName must have at least 3 characters")]
        public string FirstName { get; set; }

        [Required(AllowEmptyStrings = false)]
        [MinLength(3, ErrorMessage = "LastName must have at least 3 characters")]
        public string LastName { get; set; }

        public string Name => string.Empty == LastName ? FirstName : $"{FirstName} {LastName}";
    }
}
