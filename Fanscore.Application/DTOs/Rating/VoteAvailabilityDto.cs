using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Fanscore.Application.DTOs.Rating
{
    public class VoteAvailabilityDto
    {
        public bool CanVote { get; set; }
        public string PeriodType { get; set; } = null!;
        public int PlayerId { get; set; }
        public DateTime? NextAvailableAt { get; set; }
        public string? Message { get; set; }
    }
}