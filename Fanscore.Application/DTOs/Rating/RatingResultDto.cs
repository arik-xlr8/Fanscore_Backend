using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Fanscore.Application.DTOs.Rating
{
    public class RatingResultDto
    {
        public int RatingId { get; set; }
        public int UserId { get; set; }
        public int PlayerId { get; set; }
        public decimal RatingValue { get; set; }
        public string PeriodType { get; set; } = null!;
        public string? Comment { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime ExpiresAt { get; set; }
    }
}