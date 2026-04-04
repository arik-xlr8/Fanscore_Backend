using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Fanscore.Application.DTOs.Team
{
    public class TeamListDto
    {
        public int TeamId { get; set; }
        public string TeamName { get; set; } = null!;
        public string? PpUrl { get; set; }
    }
}