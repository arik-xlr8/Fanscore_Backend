using FanScore.Application.DTOs.Admin;
using FanScore.Application.Interfaces.Services;
using FanScore.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace FanScore.Infrastructure.Services
{
    public class AdminService : IAdminService
    {
        private readonly AppDbContext _context;
        private readonly IPlayerService _playerService;

        public AdminService(AppDbContext context, IPlayerService playerService)
        {
            _context = context;
            _playerService = playerService;
        }

        public async Task<AdminDashboardStatsDto> GetDashboardStatsAsync()
        {
            return new AdminDashboardStatsDto
            {
                UserCount = await _context.Users.CountAsync(),
                BannedUserCount = await _context.Users.CountAsync(x => x.IsBanned == true),
                ProductCount = await _context.Products.CountAsync(),
                HalisahaCount = await _context.Halisahas.CountAsync(),
                PlayerCount = await _context.Players.CountAsync(),
                RatingCount = await _context.Ratings.CountAsync()
            };
        }

        public async Task<AdminPanelDto> GetPanelDataAsync()
        {
            return new AdminPanelDto
            {
                Stats = await GetDashboardStatsAsync(),
                Users = await GetUsersAsync(),
                Products = await GetProductsAsync(),
                Halisahas = await GetHalisahasAsync(),
                Players = await _playerService.GetAllPlayersAsync(null)
            };
        }

        public async Task<List<AdminUserDto>> GetUsersAsync()
        {
            return await _context.Users
                .OrderByDescending(x => x.CreatedAt)
                .Select(x => new AdminUserDto
                {
                    UserId = x.UserId,
                    UserName = x.UserName,
                    Name = x.Name,
                    Surname = x.Surname,
                    Email = x.Email,
                    Role = x.Role ?? "user",
                    CreatedAt = x.CreatedAt,
                    IsVerified = x.IsVerified ?? false,
                    ProfilePic = x.ProfilePic,
                    IsBanned = x.IsBanned ?? false,
                    BanReason = x.BanReason,
                    PhoneNumber = x.PhoneNumber
                })
                .ToListAsync();
        }

        public async Task<bool> BanUserAsync(int userId, string? banReason)
        {
            var user = await _context.Users.FirstOrDefaultAsync(x => x.UserId == userId);
            if (user == null)
                return false;

            user.IsBanned = true;
            user.BanReason = string.IsNullOrWhiteSpace(banReason) ? "Admin tarafindan banlandi." : banReason.Trim();
            user.RefreshToken = null;
            user.RefreshTokenExpires = null;

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> UnbanUserAsync(int userId)
        {
            var user = await _context.Users.FirstOrDefaultAsync(x => x.UserId == userId);
            if (user == null)
                return false;

            user.IsBanned = false;
            user.BanReason = null;

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> ChangeUserRoleAsync(int userId, string role)
        {
            var normalizedRole = role.Trim().ToLowerInvariant();
            var allowedRoles = new[] { "user", "admin" };
            if (!allowedRoles.Contains(normalizedRole))
                throw new Exception("Gecersiz rol. Sadece user veya admin kullanilabilir.");

            var user = await _context.Users.FirstOrDefaultAsync(x => x.UserId == userId);
            if (user == null)
                return false;

            user.Role = normalizedRole;
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> UpdateProductAsync(int productId, AdminUpdateProductDto dto)
        {
            var product = await _context.Products.FirstOrDefaultAsync(x => x.ProductId == productId);
            if (product == null)
                return false;

            await ValidateProductAsync(dto.CityId, dto.TeamId, dto.Condition);

            product.Name = dto.Name;
            product.ShortDescription = dto.ShortDescription;
            product.Description = dto.Description;
            product.Price = dto.Price;
            product.TeamId = dto.TeamId;
            product.CityId = dto.CityId;
            product.Condition = dto.Condition;

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteProductAsync(int productId)
        {
            var product = await _context.Products.FirstOrDefaultAsync(x => x.ProductId == productId);
            if (product == null)
                return false;

            var pics = await _context.Pics.Where(x => x.ProductId == productId).ToListAsync();
            if (pics.Any())
                _context.Pics.RemoveRange(pics);

            _context.Products.Remove(product);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> UpdateHalisahaAsync(int haliSahaId, AdminUpdateHalisahaDto dto)
        {
            var hali = await _context.Halisahas.FirstOrDefaultAsync(x => x.HaliSahaId == haliSahaId);
            if (hali == null)
                return false;

            var cityExists = await _context.Cities.AnyAsync(x => x.CityId == dto.CityId);
            if (!cityExists)
                throw new Exception("Gecersiz sehir secildi.");

            hali.Name = dto.Name;
            hali.Description = dto.Description;
            hali.CityId = dto.CityId;
            hali.Price = dto.Price;
            hali.TeamSize = dto.TeamSize;

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteHalisahaAsync(int haliSahaId)
        {
            var hali = await _context.Halisahas.FirstOrDefaultAsync(x => x.HaliSahaId == haliSahaId);
            if (hali == null)
                return false;

            _context.Halisahas.Remove(hali);
            await _context.SaveChangesAsync();
            return true;
        }

        private async Task<List<Fanscore.Application.DTOs.Product.ProductListDto>> GetProductsAsync()
        {
            const string defaultProductImageUrl = "http://localhost:5153/uploads/products/default_product.png";

            return await _context.Products
                .OrderByDescending(p => p.ListedAt)
                .Select(p => new Fanscore.Application.DTOs.Product.ProductListDto
                {
                    ProductId = p.ProductId,
                    Name = p.Name,
                    ShortDescription = p.ShortDescription,
                    Price = p.Price,
                    ListedAt = p.ListedAt,
                    Condition = p.Condition,
                    UserId = p.UserId,
                    UserName = p.User.UserName,
                    UserProfilePic = p.User.ProfilePic,
                    TeamId = p.TeamId,
                    TeamName = p.Team != null ? p.Team.TeamName : null,
                    CityId = p.CityId,
                    CityName = p.City.CityName,
                    MainPicUrl = _context.Pics
                        .Where(x => x.ProductId == p.ProductId)
                        .Select(x => x.PicUrl)
                        .FirstOrDefault() ?? defaultProductImageUrl
                })
                .ToListAsync();
        }

        private async Task<List<Fanscore.Application.DTOs.Tournement.TournamentListDto>> GetHalisahasAsync()
        {
            return await _context.Halisahas
                .OrderByDescending(x => x.CreatedAt)
                .Select(x => new Fanscore.Application.DTOs.Tournement.TournamentListDto
                {
                    HaliSahaId = x.HaliSahaId,
                    Name = x.Name,
                    Description = x.Description,
                    CityId = x.CityId,
                    CityName = x.City.CityName,
                    CreatedAt = x.CreatedAt,
                    Price = x.Price,
                    TeamSize = x.TeamSize,
                    UserId = x.UserId,
                    UserName = x.User.UserName
                })
                .ToListAsync();
        }

        private async Task ValidateProductAsync(int cityId, int? teamId, string condition)
        {
            var cityExists = await _context.Cities.AnyAsync(x => x.CityId == cityId);
            if (!cityExists)
                throw new Exception("Gecersiz sehir secildi.");

            if (teamId.HasValue)
            {
                var teamExists = await _context.Teams.AnyAsync(x => x.TeamId == teamId.Value);
                if (!teamExists)
                    throw new Exception("Gecersiz takim secildi.");
            }

            var validConditions = new[] { "Sifir", "AzKullanilmis", "Iyi", "Orta", "Yipranmis" };
            if (!validConditions.Contains(condition))
                throw new Exception("Gecersiz urun durumu.");
        }
    }
}
