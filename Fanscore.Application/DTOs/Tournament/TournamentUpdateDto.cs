using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Fanscore.Application.DTOs.Tournament
{
    public class TournamentUpdateDto
    {
        public string Name { get; set; } = null!;
        public string? Description { get; set; }

        public int CityId { get; set; }

        public decimal Price { get; set; }
        public int TeamSize { get; set; }
    }
}