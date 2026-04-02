using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FanScore.Application.DTOs.Player
{
    public class PlayerDto
    {
        public int PlayerId { get; set; }
        public string Name { get; set; } = null!;
        public string Surname { get; set; } = null!;
        public int? TeamId { get; set; }
        public string TeamName { get; set; } = null!;
        public int? Age { get; set; }
        public string Position { get; set; } = null!;
        public string? PpUrl { get; set; }

        public decimal? AverageRating { get; set; }
        public int RatingCount { get; set; }
        public decimal? Change { get; set; }

        public List<PlayerRatingDto> Ratings { get; set; } = new();
    }

    public class PlayerRatingDto
    {
        public int RatingId { get; set; }
        public int UserId { get; set; }
        public decimal RatingValue { get; set; }
        public string? Comment { get; set; }
        public DateTime CreatedAt { get; set; }
        public string PeriodType { get; set; } = null!;
        public DateOnly BucketStart { get; set; }
    }
}