using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FanScore.Application.DTOs.Auth
{
    public class AuthResponseDto
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
        public string? AccessToken { get; set; }
        public string? RefreshToken { get; set; }
    }
}