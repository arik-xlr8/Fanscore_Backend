using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Fanscore.Application.DTOs.Tournement
{
    public class TournamentListDto
    {
        public int HaliSahaId { get; set; }
        public string Name { get; set; } = null!;
        public string? Description { get; set; }

        public int CityId { get; set; }
        public string? CityName { get; set; }

        public DateTime CreatedAt { get; set; }

        public decimal Price { get; set; }
        public int TeamSize { get; set; }

        public int UserId { get; set; }
        public string? UserName { get; set; }
    }
}