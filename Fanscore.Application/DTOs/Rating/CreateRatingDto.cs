using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Fanscore.Application.DTOs.Rating
{
    public class CreateRatingDto
    {
        public int PlayerId { get; set; }
        public string PeriodType { get; set; } = null!;
        public decimal RatingValue { get; set; }
        public string? Comment { get; set; }
    }
}