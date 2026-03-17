using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FanScore.Application.DTOs.Player
{
    public class PlayerDto
    {
        public int PlayerId { get; set; }
        public string Name { get; set; } = null!;
        public string Surname { get; set; } = null!;
        public int? TeamId { get; set; }
        public int? Age { get; set; }
        public string? Position { get; set; }
        public string? PpUrl { get; set; }
    }
}