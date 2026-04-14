using Fanscore.Application.DTOs.Tournament;
using Fanscore.Application.DTOs.Tournement;
using Fanscore.Application.Interfaces.Services;
using FanScore.Api.Services.Abstract;
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
                    City = x.City,
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
                    City = x.City,
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
            var hali = new Halisaha
            {
                Name = dto.Name,
                Description = dto.Description,
                City = dto.City,
                CreatedAt = DateTime.UtcNow,
                UserId = userId,
                Price = dto.Price,
                TeamSize = dto.TeamSize
            };

            _context.Halisahas.Add(hali);
            await _context.SaveChangesAsync();

            return await GetByIdAsync(hali.HaliSahaId) ?? throw new Exception("Oluşturulamadı");
        }

        public async Task<bool> UpdateAsync(int id, int userId, TournamentUpdateDto dto)
        {
            var hali = await _context.Halisahas.FirstOrDefaultAsync(x => x.HaliSahaId == id);

            if (hali == null)
                return false;

            if (hali.UserId != userId)
                throw new Exception("Yetkin yok");

            hali.Name = dto.Name;
            hali.Description = dto.Description;
            hali.City = dto.City;
            hali.Price = dto.Price;
            hali.TeamSize = dto.TeamSize;

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteAsync(int id, int userId)
        {
            var hali = await _context.Halisahas.FirstOrDefaultAsync(x => x.HaliSahaId == id);

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