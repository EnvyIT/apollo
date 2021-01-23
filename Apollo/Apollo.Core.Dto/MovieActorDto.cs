using System.ComponentModel.DataAnnotations;
using Apollo.Core.Dto.ValidationAttributes;

namespace Apollo.Core.Dto
{
    public class MovieActorDto : BaseDto
    {
        public MovieDto Movie { get; set; }

        public ActorDto Actor { get; set; }

        [Required]
        [Range(1L, long.MaxValue, ErrorMessage = "ActorId must between one and maximum value of long")]
        public long ActorId { get; set; }

        [Required]
        [Range(1L, long.MaxValue, ErrorMessage = "MovieId must between one and maximum value of long")]
        public long MovieId { get; set; }
    }
}
