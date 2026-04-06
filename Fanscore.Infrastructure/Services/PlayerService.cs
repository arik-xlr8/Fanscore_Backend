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
            var players = await _context.Players
                .Include(p => p.Team)
                .Include(p => p.Ratings)
                .ToListAsync();

            return players
                .Select(p => MapPlayerToDto(p, periodType))
                .OrderByDescending(p => p.RatingCount)
                .ThenByDescending(p => p.AverageRating ?? 0)
                .ToList();
        }

        public async Task<List<PlayerDto>> SearchPlayersAsync(string? searchTerm, string? periodType)
        {
            var query = _context.Players
                .Include(p => p.Team)
                .Include(p => p.Ratings)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                var term = searchTerm.Trim().ToLower();

                query = query.Where(p =>
                    (p.Name != null && p.Name.ToLower().Contains(term)) ||
                    (p.Surname != null && p.Surname.ToLower().Contains(term)) ||
                    ((p.Name + " " + p.Surname).ToLower().Contains(term)) ||
                    (p.Team != null && p.Team.TeamName != null && p.Team.TeamName.ToLower().Contains(term))
                );
            }

            var players = await query.ToListAsync();

            return players
                .Select(p => MapPlayerToDto(p, periodType))
                .OrderByDescending(p => p.RatingCount)
                .ThenByDescending(p => p.AverageRating ?? 0)
                .ToList();
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

        public async Task<List<PlayerDto>> GetShuffledPlayersAsync(string? periodType)
        {
            var currentStart = GetCurrentPeriodStartDate(periodType);
            var previousStart = GetPreviousPeriodStartDate(periodType);
            var previousEnd = GetPreviousPeriodEndDate(periodType);

            var players = await _context.Players
                .Include(p => p.Team)
                .Include(p => p.Ratings)
                .ToListAsync();

            var random = new Random();

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
            })
            .OrderBy(x => random.Next())
            .ToList();

            return result;
        }

        private PlayerDto MapPlayerToDto(Domain.Entities.Player p, string? periodType)
        {
            var currentStart = GetCurrentPeriodStartDate(periodType);
            var previousStart = GetPreviousPeriodStartDate(periodType);
            var previousEnd = GetPreviousPeriodEndDate(periodType);

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
        }
    }
}