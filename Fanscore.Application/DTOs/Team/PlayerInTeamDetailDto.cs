using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Fanscore.Application.DTOs.Team
{
    public class PlayerInTeamDetailDto
    {
        public int PlayerId { get; set; }
        public string FullName { get; set; } = null!;
        public string? Position { get; set; }
        public string? PpUrl { get; set; }
        public decimal RatingValue { get; set; }
    }
}