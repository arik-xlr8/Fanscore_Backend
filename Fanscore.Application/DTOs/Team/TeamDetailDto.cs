using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Fanscore.Application.DTOs.Team
{
    public class TeamDetailDto
    {
        public int TeamId { get; set; }
        public string TeamName { get; set; } = null!;
        public string? PpUrl { get; set; }
        public string SelectedPeriod { get; set; } = null!;
        public decimal TeamTotalValue { get; set; }
        public List<PlayerInTeamDetailDto> Players { get; set; } = new();
    }
}