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

        public async Task<List<PlayerDto>> GetAllPlayersAsync(string? periodType)
        {
            var currentStart = GetCurrentPeriodStartDate(periodType);
            var previousStart = GetPreviousPeriodStartDate(periodType);
            var previousEnd = GetPreviousPeriodEndDate(periodType);

            var players = await _context.Players
                .Include(p => p.Team)
                .Include(p => p.Ratings)
                .ToListAsync();

            var result = players.Select(p =>
            {
                var periodRatings = p.Ratings.AsEnumerable();

                if (!string.IsNullOrEmpty(periodType))
                {
                    periodRatings = periodRatings.Where(r => r.PeriodType == periodType);
                }

                var currentRatings = periodRatings;

                if (currentStart != null)
                {
                    currentRatings = currentRatings.Where(r => r.CreatedAt >= currentStart.Value);
                }

                var currentRatingsList = currentRatings.ToList();

                decimal? currentAverage = currentRatingsList.Any()
                    ? currentRatingsList.Average(r => r.RatingValue)
                    : null;

                int currentCount = currentRatingsList.Count;

                IEnumerable<Domain.Entities.Rating> previousRatings = Enumerable.Empty<Domain.Entities.Rating>();

                if (previousStart != null && previousEnd != null)
                {
                    previousRatings = periodRatings.Where(r =>
                        r.CreatedAt >= previousStart.Value &&
                        r.CreatedAt < previousEnd.Value);
                }

                var previousRatingsList = previousRatings.ToList();

                decimal? previousAverage = previousRatingsList.Any()
                    ? previousRatingsList.Average(r => r.RatingValue)
                    : null;

                decimal currentAvg = currentAverage ?? 0;
                decimal previousAvg = previousAverage ?? 0;

                decimal change;

                if (previousAvg == 0)
                {
                    if (currentAvg == 0)
                        change = 0;
                    else
                        change = 100;
                }
                else
                {
                    change = ((currentAvg - previousAvg) / previousAvg) * 100;
                }

                return new PlayerDto
                {
                    PlayerId = p.PlayerId,
                    Name = p.Name,
                    Surname = p.Surname,
                    TeamId = p.TeamId,
                    TeamName = p.Team != null ? p.Team.TeamName : null,
                    Age = p.Age,
                    Position = p.Position,
                    PpUrl = p.PpUrl,
                    AverageRating = currentAverage,
                    RatingCount = currentCount,
                    Change = change,
                    Ratings = new List<PlayerRatingDto>()
                };
            }).ToList();

            return result;
        }

        public async Task<PlayerDto?> GetPlayerByIdAsync(int playerId, string? periodType)
        {
            var currentStart = GetCurrentPeriodStartDate(periodType);
            var previousStart = GetPreviousPeriodStartDate(periodType);
            var previousEnd = GetPreviousPeriodEndDate(periodType);

            var player = await _context.Players
                .Include(p => p.Team)
                .Include(p => p.Ratings)
                .FirstOrDefaultAsync(p => p.PlayerId == playerId);

            if (player == null)
                return null;

            var periodRatings = player.Ratings.AsEnumerable();

            if (!string.IsNullOrEmpty(periodType))
            {
                periodRatings = periodRatings.Where(r => r.PeriodType == periodType);
            }

            var currentRatings = periodRatings;

            if (currentStart != null)
            {
                currentRatings = currentRatings.Where(r => r.CreatedAt >= currentStart.Value);
            }

            var currentRatingsList = currentRatings.ToList();

            decimal? currentAverage = currentRatingsList.Any()
                ? currentRatingsList.Average(r => r.RatingValue)
                : null;

            int currentCount = currentRatingsList.Count;

            IEnumerable<Domain.Entities.Rating> previousRatings = Enumerable.Empty<Domain.Entities.Rating>();

            if (previousStart != null && previousEnd != null)
            {
                previousRatings = periodRatings.Where(r =>
                    r.CreatedAt >= previousStart.Value &&
                    r.CreatedAt < previousEnd.Value);
            }

            var previousRatingsList = previousRatings.ToList();

            decimal? previousAverage = previousRatingsList.Any()
                ? previousRatingsList.Average(r => r.RatingValue)
                : null;

            decimal currentAvg = currentAverage ?? 0;
            decimal previousAvg = previousAverage ?? 0;

            decimal change;

            if (previousAvg == 0)
            {
                if (currentAvg == 0)
                    change = 0;
                else
                    change = 100;
            }
            else
            {
                change = ((currentAvg - previousAvg) / previousAvg) * 100;
            }

            return new PlayerDto
            {
                PlayerId = player.PlayerId,
                Name = player.Name,
                Surname = player.Surname,
                TeamId = player.TeamId,
                TeamName = player.Team != null ? player.Team.TeamName : null,
                Age = player.Age,
                Position = player.Position,
                PpUrl = player.PpUrl,
                AverageRating = currentAverage,
                RatingCount = currentCount,
                Change = change,
                Ratings = new List<PlayerRatingDto>()
            };
        }

        private DateTime? GetCurrentPeriodStartDate(string? periodType)
        {
            var now = DateTime.UtcNow;

            return periodType switch
            {
                "daily" => now.AddDays(-1),
                "weekly" => now.AddDays(-7),
                "monthly" => now.AddMonths(-1),
                "3months" => now.AddMonths(-3),
                "1year" => now.AddYears(-1),
                null => null,
                _ => null
            };
        }

        private DateTime? GetPreviousPeriodStartDate(string? periodType)
        {
            var now = DateTime.UtcNow;

            return periodType switch
            {
                "daily" => now.AddDays(-2),
                "weekly" => now.AddDays(-14),
                "monthly" => now.AddMonths(-2),
                "3months" => now.AddMonths(-6),
                "1year" => now.AddYears(-2),
                null => null,
                _ => null
            };
        }

        private DateTime? GetPreviousPeriodEndDate(string? periodType)
        {
            var now = DateTime.UtcNow;

            return periodType switch
            {
                "daily" => now.AddDays(-1),
                "weekly" => now.AddDays(-7),
                "monthly" => now.AddMonths(-1),
                "3months" => now.AddMonths(-3),
                "1year" => now.AddYears(-1),
                null => null,
                _ => null
            };
        }
    }
}