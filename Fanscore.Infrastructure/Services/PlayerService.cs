using Fanscore.Application.DTOs.Player;
using FanScore.Application.DTOs.Admin;
using FanScore.Application.DTOs.Player;
using FanScore.Application.Interfaces.Services;
using FanScore.Domain.Entities;
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
            var player = await _context.Players
                .Include(p => p.Team)
                .Include(p => p.Ratings)
                .FirstOrDefaultAsync(p => p.PlayerId == playerId);

            if (player == null)
                return null;

            var dto = MapPlayerToDto(player, periodType);
            dto.Ratings = new List<PlayerRatingDto>();

            return dto;
        }

        public async Task<PagedResultDto<PlayerDto>> GetAllPlayersPagedAsync(
            string? periodType,
            int page,
            int pageSize)
        {
            page = Math.Max(page, 1);
            pageSize = Math.Clamp(pageSize, 1, 100);

            var query = _context.Players
                .Include(p => p.Team)
                .Include(p => p.Ratings)
                .AsQueryable();

            var totalCount = await query.CountAsync();

            var players = await query.ToListAsync();

            var mappedPlayers = players
                .Select(p => MapPlayerToDto(p, periodType))
                .OrderByDescending(p => p.RatingCount)
                .ThenByDescending(p => p.AverageRating ?? 0)
                .ToList();

            var pagedPlayers = mappedPlayers
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            return new PagedResultDto<PlayerDto>
            {
                Items = pagedPlayers,
                Page = page,
                PageSize = pageSize,
                TotalCount = totalCount,
                HasMore = page * pageSize < totalCount
            };
        }

        public async Task<PagedResultDto<PlayerDto>> SearchPlayersPagedAsync(
            string? searchTerm,
            string? periodType,
            int page,
            int pageSize)
        {
            page = Math.Max(page, 1);
            pageSize = Math.Clamp(pageSize, 1, 100);

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

            var totalCount = await query.CountAsync();

            var players = await query.ToListAsync();

            var mappedPlayers = players
                .Select(p => MapPlayerToDto(p, periodType))
                .OrderByDescending(p => p.RatingCount)
                .ThenByDescending(p => p.AverageRating ?? 0)
                .ToList();

            var pagedPlayers = mappedPlayers
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            return new PagedResultDto<PlayerDto>
            {
                Items = pagedPlayers,
                Page = page,
                PageSize = pageSize,
                TotalCount = totalCount,
                HasMore = page * pageSize < totalCount
            };
        }

        public async Task<PagedResultDto<PlayerDto>> GetShuffledPlayersPagedAsync(
            string? periodType,
            int page,
            int pageSize,
            int shuffleSeed)
        {
            page = Math.Max(page, 1);
            pageSize = Math.Clamp(pageSize, 1, 100);

            var players = await _context.Players
                .Include(p => p.Team)
                .Include(p => p.Ratings)
                .ToListAsync();

            var totalCount = players.Count;

            var random = new Random(shuffleSeed);

            var mappedPlayers = players
                .Select(p => MapPlayerToDto(p, periodType))
                .OrderBy(_ => random.Next())
                .ToList();

            var pagedPlayers = mappedPlayers
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            return new PagedResultDto<PlayerDto>
            {
                Items = pagedPlayers,
                Page = page,
                PageSize = pageSize,
                TotalCount = totalCount,
                HasMore = page * pageSize < totalCount
            };
        }

        public async Task<List<PlayerDto>> GetShuffledPlayersAsync(string? periodType)
        {
            var players = await _context.Players
                .Include(p => p.Team)
                .Include(p => p.Ratings)
                .ToListAsync();

            var random = new Random();

            return players
                .Select(p => MapPlayerToDto(p, periodType))
                .OrderBy(_ => random.Next())
                .ToList();
        }

        public async Task<PlayerDto> CreatePlayerAsync(AdminCreatePlayerDto dto)
        {
            await ValidatePlayerAsync(dto);

            var player = new Player
            {
                Name = dto.Name.Trim(),
                Surname = dto.Surname.Trim(),
                TeamId = dto.TeamId,
                Age = dto.Age,
                Position = string.IsNullOrWhiteSpace(dto.Position) ? null : dto.Position.Trim(),
                PpUrl = string.IsNullOrWhiteSpace(dto.PpUrl) ? null : dto.PpUrl.Trim()
            };

            _context.Players.Add(player);
            await _context.SaveChangesAsync();

            return await GetPlayerByIdAsync(player.PlayerId, null)
                ?? throw new Exception("Oyuncu olusturuldu ama tekrar okunamadi.");
        }

        public async Task<PlayerDto?> UpdatePlayerAsync(int playerId, AdminUpdatePlayerDto dto)
        {
            await ValidatePlayerAsync(dto, playerId);

            var player = await _context.Players.FirstOrDefaultAsync(x => x.PlayerId == playerId);
            if (player == null)
                return null;

            player.Name = dto.Name.Trim();
            player.Surname = dto.Surname.Trim();
            player.TeamId = dto.TeamId;
            player.Age = dto.Age;
            player.Position = string.IsNullOrWhiteSpace(dto.Position) ? null : dto.Position.Trim();
            player.PpUrl = string.IsNullOrWhiteSpace(dto.PpUrl) ? null : dto.PpUrl.Trim();

            await _context.SaveChangesAsync();

            return await GetPlayerByIdAsync(playerId, null);
        }

        public async Task<bool> DeletePlayerAsync(int playerId)
        {
            var player = await _context.Players
                .Include(x => x.Ratings)
                .FirstOrDefaultAsync(x => x.PlayerId == playerId);

            if (player == null)
                return false;

            if (player.Ratings.Any())
                _context.Ratings.RemoveRange(player.Ratings);

            _context.Players.Remove(player);
            await _context.SaveChangesAsync();

            return true;
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

        private async Task ValidatePlayerAsync(AdminCreatePlayerDto dto, int? ignoredPlayerId = null)
        {
            if (string.IsNullOrWhiteSpace(dto.Name))
                throw new Exception("Oyuncu adi zorunlu.");

            if (string.IsNullOrWhiteSpace(dto.Surname))
                throw new Exception("Oyuncu soyadi zorunlu.");

            if (dto.Age.HasValue && (dto.Age.Value < 10 || dto.Age.Value > 60))
                throw new Exception("Oyuncu yasi 10 ile 60 arasinda olmali.");

            if (dto.TeamId.HasValue)
            {
                var teamExists = await _context.Teams.AnyAsync(x => x.TeamId == dto.TeamId.Value);
                if (!teamExists)
                    throw new Exception("Gecersiz takim secildi.");
            }

            var name = dto.Name.Trim();
            var surname = dto.Surname.Trim();

            var duplicateExists = await _context.Players.AnyAsync(x =>
                x.Name == name &&
                x.Surname == surname &&
                x.TeamId == dto.TeamId &&
                (!ignoredPlayerId.HasValue || x.PlayerId != ignoredPlayerId.Value));

            if (duplicateExists)
                throw new Exception("Bu takimda ayni isimde bir oyuncu zaten var.");
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
                change = currentAvg == 0 ? 0 : 100;
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
