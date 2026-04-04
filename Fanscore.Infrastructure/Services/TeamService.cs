using Fanscore.Application.DTOs.Team;
using FanScore.Application.Interfaces;
using FanScore.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace FanScore.Application.Services
{
    public class TeamService : ITeamService
    {
        private readonly AppDbContext _context;

        public TeamService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<TeamListDto>> GetAllTeamsAsync()
        {
            var teams = await _context.Teams
                .Select(t => new TeamListDto
                {
                    TeamId = t.TeamId,
                    TeamName = t.TeamName,
                    PpUrl = t.PpUrl
                })
                .ToListAsync();

            return teams;
        }

        public async Task<TeamDetailDto?> GetTeamDetailAsync(int teamId, string periodType)
        {
            var normalizedPeriod = periodType.Trim().ToLower();

            var allowedPeriods = new[] { "daily", "weekly", "monthly", "3months", "yearly" };
            if (!allowedPeriods.Contains(normalizedPeriod))
            {
                throw new ArgumentException("Geçersiz period type.");
            }

            var team = await _context.Teams
                .Where(t => t.TeamId == teamId)
                .Select(t => new
                {
                    t.TeamId,
                    t.TeamName,
                    t.PpUrl
                })
                .FirstOrDefaultAsync();

            if (team == null)
                return null;

            var players = await _context.Players
                .Where(p => p.TeamId == teamId)
                .Select(p => new PlayerInTeamDetailDto
                {
                    PlayerId = p.PlayerId,
                    FullName = p.Name + " " + p.Surname,
                    Position = p.Position,
                    PpUrl = p.PpUrl,
                    RatingValue = p.Ratings
                        .Where(r => r.PeriodType.ToLower() == normalizedPeriod)
                        .OrderByDescending(r => r.CreatedAt)
                        .Select(r => r.RatingValue)
                        .FirstOrDefault()
                })
                .ToListAsync();

            var dto = new TeamDetailDto
            {
                TeamId = team.TeamId,
                TeamName = team.TeamName,
                PpUrl = team.PpUrl,
                SelectedPeriod = normalizedPeriod,
                Players = players,
                TeamTotalValue = players.Sum(p => p.RatingValue)
            };

            return dto;
        }
    }
}