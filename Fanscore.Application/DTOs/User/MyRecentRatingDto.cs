using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Fanscore.Application.DTOs.User
{
    public class MyRecentRatingDto
    {
        public int RatingId { get; set; }
        public int PlayerId { get; set; }
        public string PlayerName { get; set; } = null!;
        public string? PlayerPhoto { get; set; }
        public decimal RatingValue { get; set; }
        public string? Comment { get; set; }
        public string PeriodType { get; set; } = null!;
        public DateTime CreatedAt { get; set; }
    }
}