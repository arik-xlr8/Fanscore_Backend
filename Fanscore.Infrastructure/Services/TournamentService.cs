using Fanscore.Application.DTOs.Tournament;
using Fanscore.Application.DTOs.Tournement;
using Fanscore.Application.Interfaces.Services;
using FanScore.Domain.Entities;
using FanScore.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace FanScore.Api.Services.Concrete
{
    public class TournamentService : ITournamentService
    {
        private readonly AppDbContext _context;

        public TournamentService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<TournamentListDto>> GetAllAsync()
        {
            return await _context.Halisahas
                .OrderByDescending(x => x.CreatedAt)
                .Select(x => new TournamentListDto
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

        public async Task<List<TournamentListDto>> GetByUserIdAsync(int userId)
        {
            return await _context.Halisahas
                .Where(x => x.UserId == userId)
                .OrderByDescending(x => x.CreatedAt)
                .Select(x => new TournamentListDto
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

        public async Task<TournamentDetailDto?> GetByIdAsync(int id)
        {
            return await _context.Halisahas
                .Where(x => x.HaliSahaId == id)
                .Select(x => new TournamentDetailDto
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
                    UserName = x.User.UserName,
                    UserProfilePic = x.User.ProfilePic,
                    PhoneNumber = x.User.PhoneNumber
                })
                .FirstOrDefaultAsync();
        }

        public async Task<TournamentDetailDto> CreateAsync(int userId, TournamentCreateDto dto)
        {
            var cityExists = await _context.Cities.AnyAsync(x => x.CityId == dto.CityId);

            if (!cityExists)
                throw new Exception("Geçersiz şehir seçildi.");

            var hali = new Halisaha
            {
                Name = dto.Name,
                Description = dto.Description,

                CityId = dto.CityId,

                CreatedAt = DateTime.UtcNow,
                UserId = userId,
                Price = dto.Price,
                TeamSize = dto.TeamSize
            };

            _context.Halisahas.Add(hali);
            await _context.SaveChangesAsync();

            return await GetByIdAsync(hali.HaliSahaId)
                ?? throw new Exception("Oluşturulamadı");
        }

        public async Task<bool> UpdateAsync(int id, int userId, TournamentUpdateDto dto)
        {
            var hali = await _context.Halisahas
                .FirstOrDefaultAsync(x => x.HaliSahaId == id);

            if (hali == null)
                return false;

            if (hali.UserId != userId)
                throw new Exception("Yetkin yok");

            var cityExists = await _context.Cities.AnyAsync(x => x.CityId == dto.CityId);

            if (!cityExists)
                throw new Exception("Geçersiz şehir seçildi.");

            hali.Name = dto.Name;
            hali.Description = dto.Description;

            hali.CityId = dto.CityId;

            hali.Price = dto.Price;
            hali.TeamSize = dto.TeamSize;

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteAsync(int id, int userId)
        {
            var hali = await _context.Halisahas
                .FirstOrDefaultAsync(x => x.HaliSahaId == id);

            if (hali == null)
                return false;

            if (hali.UserId != userId)
                throw new Exception("Yetkin yok");

            _context.Halisahas.Remove(hali);
            await _context.SaveChangesAsync();

            return true;
        }
        
    }
}