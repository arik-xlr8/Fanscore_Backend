using Fanscore.Application.DTOs.User;
using Fanscore.Application.Interfaces.Services;
using FanScore.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace FanScore.Infrastructure.Services
{
    public class ProfileService : IProfileService
    {
        private readonly AppDbContext _context;

        public ProfileService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<ProfileDto> GetMyProfileAsync(int userId)
        {
            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.UserId == userId);

            if (user == null)
                throw new KeyNotFoundException("Kullanıcı bulunamadı.");

            var recentRatings = await (
                from r in _context.Ratings
                join p in _context.Players on r.PlayerId equals p.PlayerId
                where r.UserId == userId
                orderby r.CreatedAt descending
                select new MyRecentRatingDto
                {
                    RatingId = r.RatingId,
                    PlayerId = p.PlayerId,
                    PlayerName = ((p.Name ?? "") + " " + (p.Surname ?? "")).Trim(),
                    PlayerPhoto = p.PpUrl,
                    RatingValue = r.RatingValue,
                    Comment = r.Comment,
                    PeriodType = r.PeriodType,
                    CreatedAt = r.CreatedAt
                }
            ).Take(10).ToListAsync();

            return new ProfileDto
            {
                UserId = user.UserId,
                Name = user.Name,
                Surname = user.Surname,
                UserName = user.UserName,
                ProfilePic = user.ProfilePic,
                RecentRatings = recentRatings
            };
        }

        public async Task<ProfileDto> UpdateMyProfileAsync(int userId, UpdateProfileDto dto)
        {
            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.UserId == userId);

            if (user == null)
                throw new KeyNotFoundException("Kullanıcı bulunamadı.");

            if (!string.IsNullOrWhiteSpace(dto.UserName))
            {
                var normalizedUserName = dto.UserName.Trim();

                var exists = await _context.Users.AnyAsync(u =>
                    u.UserId != userId && u.UserName == normalizedUserName);

                if (exists)
                    throw new InvalidOperationException("Bu kullanıcı adı zaten kullanılıyor.");

                user.UserName = normalizedUserName;
            }

            user.Name = string.IsNullOrWhiteSpace(dto.Name) ? user.Name : dto.Name.Trim();
            user.Surname = string.IsNullOrWhiteSpace(dto.Surname) ? user.Surname : dto.Surname.Trim();

            if (!string.IsNullOrWhiteSpace(dto.ProfilePic))
                user.ProfilePic = dto.ProfilePic.Trim();

            await _context.SaveChangesAsync();

            return await GetMyProfileAsync(userId);
        }

        public async Task<UploadProfilePhotoResultDto> UploadProfilePhotoAsync(
            int userId,
            Stream fileStream,
            string originalFileName,
            string webRootPath,
            string baseUrl)
        {
            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.UserId == userId);

            if (user == null)
                throw new KeyNotFoundException("Kullanıcı bulunamadı.");

            if (fileStream == null || string.IsNullOrWhiteSpace(originalFileName))
                throw new ArgumentException("Dosya boş olamaz.");

            var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".webp" };
            var extension = Path.GetExtension(originalFileName).ToLowerInvariant();

            if (!allowedExtensions.Contains(extension, StringComparer.OrdinalIgnoreCase))
                throw new ArgumentException("Sadece jpg, jpeg, png veya webp yükleyebilirsiniz.");

            var uploadsFolder = Path.Combine(webRootPath, "uploads", "profile-pics");
            Directory.CreateDirectory(uploadsFolder);

            var fileName = $"{userId}_{Guid.NewGuid():N}{extension}";
            var filePath = Path.Combine(uploadsFolder, fileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await fileStream.CopyToAsync(stream);
            }

            var relativePath = $"/uploads/profile-pics/{fileName}";
            var fullUrl = $"{baseUrl}{relativePath}";

            user.ProfilePic = fullUrl;
            await _context.SaveChangesAsync();

            return new UploadProfilePhotoResultDto
            {
                Url = fullUrl
            };
        }
            
    }
}