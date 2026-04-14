using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Fanscore.Application.DTOs.Tournament
{
    public class TournamentDetailDto
    {
        public int HaliSahaId { get; set; }
        public string Name { get; set; } = null!;
        public string? Description { get; set; }
        public string City { get; set; } = null!;
        public DateTime CreatedAt { get; set; }

        public decimal Price { get; set; }
        public int TeamSize { get; set; }

        public int UserId { get; set; }
        public string? UserName { get; set; }
        public string? UserProfilePic { get; set; }
        public string? PhoneNumber { get; set; }
    }
}