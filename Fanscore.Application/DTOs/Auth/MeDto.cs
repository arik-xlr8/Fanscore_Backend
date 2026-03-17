using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FanScore.Application.DTOs.Auth
{
    public class MeDto
    {
        public int UserId { get; set; }
        public string Email { get; set; } = string.Empty;
        public string Role { get; set; } = string.Empty;
        public bool IsVerified { get; set; }
        public string? Name { get; set; }
        public string? Surname { get; set; }
        public string? ProfilePic { get; set; }
    }
}