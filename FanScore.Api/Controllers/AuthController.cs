using System.Security.Claims;
using FanScore.Application.DTOs.Auth;
using FanScore.Application.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FanScore.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;
        private readonly IConfiguration _configuration;

        public AuthController(IAuthService authService, IConfiguration configuration)
        {
            _authService = authService;
            _configuration = configuration;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterDto dto)
        {
            try
            {
                var result = await _authService.RegisterAsync(dto);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto dto)
        {
            try
            {
                var result = await _authService.LoginAsync(dto);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return Unauthorized(new { message = ex.Message });
            }
        }

        [HttpGet("verify-email")]
        public async Task<IActionResult> VerifyEmail([FromQuery] string token)
        {
            var frontendBaseUrl = _configuration["App:FrontendBaseUrl"] ?? "http://localhost:4200";

            try
            {
                await _authService.VerifyEmailAsync(token);
                return Redirect($"{frontendBaseUrl}/login?verified=true");
            }
            catch (Exception ex)
            {
                return Redirect($"{frontendBaseUrl}/login?error={Uri.EscapeDataString(ex.Message)}");
            }
        }

        [Authorize]
        [HttpGet("me")]
        public async Task<IActionResult> Me()
        {
            try
            {
                var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

                if (string.IsNullOrWhiteSpace(userIdClaim))
                    return Unauthorized(new { message = "Geçersiz token." });

                var userId = int.Parse(userIdClaim);

                var result = await _authService.GetMeAsync(userId);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPost("logout")]
        [Authorize]
        public async Task<IActionResult> Logout()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrWhiteSpace(userIdClaim))
                return Unauthorized(new { message = "Geçersiz kullanıcı bilgisi." });

            if (!int.TryParse(userIdClaim, out var userId))
                return Unauthorized(new { message = "Geçersiz kullanıcı ID." });

            await _authService.LogoutAsync(userId);

            return Ok(new { message = "Çıkış başarılı." });
        }
    }
}