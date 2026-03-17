using FanScore.Application.DTOs.Auth;

namespace FanScore.Application.Interfaces.Services
{
    public interface IAuthService
    {
        Task<AuthResponseDto> RegisterAsync(RegisterDto dto);
        Task<AuthResponseDto> LoginAsync(LoginDto dto);
        Task VerifyEmailAsync(string token);
        Task<MeDto> GetMeAsync(int userId);
        Task LogoutAsync(int userId);
    }
}