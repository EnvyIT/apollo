using System;
using System.ComponentModel.DataAnnotations;
using Apollo.Core.Dto.ValidationAttributes;

namespace Apollo.Core.Dto
{
    public class ScheduleDto : BaseDto
    {
        private const double PercentageFactor = 100.0;
        private double _loadFactor;

        [Required]
        [Range(1L, long.MaxValue, ErrorMessage = "Id must between one and maximum value of long")]
        public long MovieId { get; set; }

        [MatchId(nameof(MovieId), ErrorMessage = "MovieId and Movie.Id must be equal")]
        public MovieDto Movie { get; set; }

        [Required]
        [Range(1L, long.MaxValue, ErrorMessage = "Id must between one and maximum value of long")]
        public long CinemaHallId { get; set; }

        [MatchId(nameof(CinemaHallId), ErrorMessage = "CinemaHallId and CinemaHall.Id must be equal")]
        public CinemaHallDto CinemaHall { get; set; }

        [Range(5.0, 50.0, ErrorMessage = "Price must between 5.0 and 50.0 Euro")]
        public decimal Price { get; set; }

        [Future(4.0, ErrorMessage = "StartTime (UTC) must be at least 4 hours in the future ")]
        public DateTime StartTime { get; set; }

        public double LoadFactor
        {
            get => _loadFactor * PercentageFactor;
            set => _loadFactor = value;
        }

        public string LoadFactorText => $"{LoadFactor:F1}%";

    }
}
