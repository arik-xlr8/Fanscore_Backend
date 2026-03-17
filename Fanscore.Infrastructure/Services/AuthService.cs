using System.Security.Cryptography;
using Fanscore.Application.Interfaces.Services;
using FanScore.Application.DTOs.Auth;
using FanScore.Application.Interfaces.Services;
using FanScore.Domain.Entities;
using FanScore.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace FanScore.Infrastructure.Services
{
    public class AuthService : IAuthService
    {
        private readonly AppDbContext _context;
        private readonly IJwtService _jwtService;
        private readonly IEmailService _emailService;
        private readonly IConfiguration _configuration;

        public AuthService(
            AppDbContext context,
            IJwtService jwtService,
            IEmailService emailService,
            IConfiguration configuration)
        {
            _context = context;
            _jwtService = jwtService;
            _emailService = emailService;
            _configuration = configuration;
        }

        public async Task<AuthResponseDto> RegisterAsync(RegisterDto dto)
        {
            var existingUser = await _context.Users
                .FirstOrDefaultAsync(x => x.Email == dto.Email);

            if (existingUser != null)
                throw new Exception("Bu email zaten kayıtlı.");

            var verificationToken = GenerateVerificationToken();

            var user = new User
            {
                Email = dto.Email,
                Password = BCrypt.Net.BCrypt.HashPassword(dto.Password),
                EmailVerificationToken = verificationToken,
                EmailVerificationTokenExpires = DateTime.UtcNow.AddHours(24),
                LastVerificationEmailSentAt = DateTime.UtcNow,
                IsVerified = false,
                IsBanned = false,
                CreatedAt = DateTime.UtcNow,
                Role = "user",
                Name = null,
                Surname = null
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            var backendBaseUrl = _configuration["App:BackendBaseUrl"];
            if (string.IsNullOrWhiteSpace(backendBaseUrl))
                throw new Exception("App:BackendBaseUrl bulunamadı.");

            var verificationLink =
                $"{backendBaseUrl}/api/auth/verify-email?token={Uri.EscapeDataString(verificationToken)}";

            var displayName = !string.IsNullOrWhiteSpace(user.Name)
                ? user.Name
                : user.Email.Split('@')[0];

            await _emailService.SendVerificationEmailAsync(
                user.Email,
                displayName,
                verificationLink
            );

            return new AuthResponseDto
            {
                Success = true,
                Message = "Kayıt başarılı. Doğrulama maili gönderildi."
            };
        }

        public async Task<AuthResponseDto> LoginAsync(LoginDto dto)
        {
            var user = await _context.Users
                .FirstOrDefaultAsync(x => x.Email == dto.Email);

            if (user == null)
                throw new Exception("Email veya şifre hatalı.");

            var isPasswordCorrect = BCrypt.Net.BCrypt.Verify(dto.Password, user.Password);
            if (!isPasswordCorrect)
                throw new Exception("Email veya şifre hatalı.");

            if (user.IsVerified != true)
            {
                var backendBaseUrl = _configuration["App:BackendBaseUrl"];
                if (string.IsNullOrWhiteSpace(backendBaseUrl))
                    throw new Exception("App:BackendBaseUrl bulunamadı.");

                var shouldResendVerification =
                    !user.EmailVerificationTokenExpires.HasValue ||
                    user.EmailVerificationTokenExpires.Value < DateTime.UtcNow;

                if (shouldResendVerification)
                {
                    var cooldownMinutes = 2;

                    if (user.LastVerificationEmailSentAt.HasValue &&
                        user.LastVerificationEmailSentAt.Value.AddMinutes(cooldownMinutes) > DateTime.UtcNow)
                    {
                        throw new Exception("Doğrulama maili kısa süre önce tekrar gönderildi. Lütfen biraz bekleyip tekrar deneyin.");
                    }

                    var newVerificationToken = GenerateVerificationToken();

                    user.EmailVerificationToken = newVerificationToken;
                    user.EmailVerificationTokenExpires = DateTime.UtcNow.AddHours(24);
                    user.LastVerificationEmailSentAt = DateTime.UtcNow;

                    await _context.SaveChangesAsync();

                    var verificationLink =
                        $"{backendBaseUrl}/api/auth/verify-email?token={Uri.EscapeDataString(newVerificationToken)}";

                    var displayName = !string.IsNullOrWhiteSpace(user.Name)
                        ? user.Name
                        : user.Email.Split('@')[0];

                    await _emailService.SendVerificationEmailAsync(
                        user.Email,
                        displayName,
                        verificationLink
                    );

                    throw new Exception("Doğrulama linkinin süresi dolmuş. Yeni doğrulama maili gönderildi.");
                }

                throw new Exception("Email adresinizi doğrulamadan giriş yapamazsınız. Lütfen mail kutunuzu kontrol edin.");
            }

            if (user.IsBanned == true)
                throw new Exception($"Hesabınız banlanmış. Sebep: {user.BanReason}");

            var role = user.Role ?? "user";
            var accessToken = _jwtService.CreateToken(user.UserId, user.Email, role);

            return new AuthResponseDto
            {
                Success = true,
                Message = "Giriş başarılı.",
                AccessToken = accessToken,
                RefreshToken = user.RefreshToken
            };
        }

        public async Task VerifyEmailAsync(string token)
        {
            if (string.IsNullOrWhiteSpace(token))
                throw new Exception("Geçersiz token.");

            var user = await _context.Users
                .FirstOrDefaultAsync(x => x.EmailVerificationToken == token);

            if (user == null)
                throw new Exception("Geçersiz doğrulama linki.");

            if (user.IsVerified == true)
                return;

            if (!user.EmailVerificationTokenExpires.HasValue)
                throw new Exception("Doğrulama linkinin süresi bulunamadı.");

            if (user.EmailVerificationTokenExpires.Value < DateTime.UtcNow)
                throw new Exception("Doğrulama linkinin süresi dolmuş.");

            user.IsVerified = true;
            user.EmailVerificationToken = null;
            user.EmailVerificationTokenExpires = null;
            user.LastVerificationEmailSentAt = null;

            await _context.SaveChangesAsync();
        }

        public async Task LogoutAsync(int userId)
        {
            var user = await _context.Users
                .FirstOrDefaultAsync(x => x.UserId == userId);

            if (user == null)
                throw new Exception("Kullanıcı bulunamadı.");

            user.RefreshToken = null;
            user.RefreshTokenExpires = null;

            await _context.SaveChangesAsync();
        }

        public async Task<MeDto> GetMeAsync(int userId)
        {
            var user = await _context.Users
                .FirstOrDefaultAsync(x => x.UserId == userId);

            if (user == null)
                throw new Exception("Kullanıcı bulunamadı.");

            return new MeDto
            {
                UserId = user.UserId,
                Email = user.Email,
                Role = user.Role ?? "user",
                IsVerified = user.IsVerified ?? false,
                Name = user.Name,
                Surname = user.Surname,
                ProfilePic = user.ProfilePic
            };
        }

        private static string GenerateVerificationToken()
        {
            var bytes = RandomNumberGenerator.GetBytes(64);
            return Convert.ToBase64String(bytes)
                .Replace("+", "-")
                .Replace("/", "_")
                .Replace("=", "");
        }
    }
}