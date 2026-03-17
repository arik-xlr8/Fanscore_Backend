using FanScore.Application.DTOs.Player;
using FanScore.Application.Interfaces.Services;
using FanScore.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace FanScore.Infrastructure.Services
{
    public class PlayerService : IPlayerService
    {
        private readonly AppDbContext _context;

        public PlayerService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<PlayerDto>> GetAllPlayersAsync()
        {
            return await _context.Players
                .Select(p => new PlayerDto
                {
                    PlayerId = p.PlayerId,
                    Name = p.Name,
                    Surname = p.Surname,
                    TeamId = p.TeamId,
                    Age = p.Age,
                    Position = p.Position,
                    PpUrl = p.PpUrl
                })
                .ToListAsync();
        }

        public async Task<PlayerDto?> GetPlayerByIdAsync(int playerId)
        {
            return await _context.Players
                .Where(p => p.PlayerId == playerId)
                .Select(p => new PlayerDto
                {
                    PlayerId = p.PlayerId,
                    Name = p.Name,
                    Surname = p.Surname,
                    TeamId = p.TeamId,
                    Age = p.Age,
                    Position = p.Position,
                    PpUrl = p.PpUrl
                })
                .FirstOrDefaultAsync();
        }
    }
}