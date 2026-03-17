using FanScore.Application.DTOs.Auth;
using FanScore.Application.Interfaces.Repositories;
using FanScore.Application.Interfaces.Services;
using FanScore.Domain.Entities;

namespace FanScore.Application.Services
{
    public class AuthService : IAuthService
    {
        private readonly IUserRepository _userRepository;
        private readonly IJwtService _jwtService;

        public AuthService(IUserRepository userRepository, IJwtService jwtService)
        {
            _userRepository = userRepository;
            _jwtService = jwtService;
        }

        public async Task<AuthResponseDto> RegisterAsync(RegisterDto dto)
        {
            var existingUser = await _userRepository.GetByEmailAsync(dto.Email);

            if (existingUser != null)
            {
                throw new Exception("Bu email zaten kullanımda.");
            }

            var user = new User
            {
                Email = dto.Email,
                Password = BCrypt.Net.BCrypt.HashPassword(dto.Password),
                Role = "User"
            };

            await _userRepository.AddAsync(user);
            await _userRepository.SaveChangesAsync();

            var token = _jwtService.CreateToken(user.UserId, user.Email, user.Role);

            return new AuthResponseDto
            {
                UserId = user.UserId,
                Email = user.Email,
                Token = token
            };
        }

        public async Task<AuthResponseDto> LoginAsync(LoginDto dto)
        {
            var user = await _userRepository.GetByEmailAsync(dto.Email);

            if (user == null)
            {
                throw new Exception("Email veya şifre hatalı.");
            }

            var isPasswordValid = BCrypt.Net.BCrypt.Verify(dto.Password, user.Password);

            if (!isPasswordValid)
            {
                throw new Exception("Email veya şifre hatalı.");
            }

            var token = _jwtService.CreateToken(user.UserId, user.Email, user.Role);

            return new AuthResponseDto
            {
                UserId = user.UserId,
                Email = user.Email,
                Token = token
            };
        }
    }
}