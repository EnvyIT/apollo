using System.ComponentModel.DataAnnotations;
using Apollo.Core.Dto.ValidationAttributes;

namespace Apollo.Core.Dto
{
    public class MovieDto : BaseDto
    {
        [Required(AllowEmptyStrings = false)]
        [MinLength(3, ErrorMessage = "Title must have at least 3 characters")]
        public string Title { get; set; }

        [Required(AllowEmptyStrings = false)]
        [MinLength(4, ErrorMessage = "Description must have at least 4 characters")]
        public string Description { get; set; }

        [MatchId(nameof(GenreId), ErrorMessage = "GenreId and Genre.Id must be equal")]
        public GenreDto Genre { get; set; }

        [Required]
        [Range(1L, 600L, ErrorMessage = "Duration must be between 1 and 600 minutes")]
        public long Duration { get; set; }

        public byte[] Image { get; set; }

        [Url(ErrorMessage = "Trailer must be a valid URL")]
        public string Trailer { get; set; }

        public MovieActorDto MovieActor { get; set; }

        [Range(1L, long.MaxValue, ErrorMessage = "Id must between one and maximum value of long")]
        public long GenreId { get; set; }

        [Required]
        [Range(1, 5)]
        public int Rating { get; set; }
    }
}