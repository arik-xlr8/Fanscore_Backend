using Fanscore.Application.Interfaces.Services;
using Fanscore.Application.DTOs.Rating;
using FanScore.Domain.Entities;
using FanScore.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Fanscore.Domain.Entities;

namespace FanScore.Infrastructure.Services
{
    public class RatingService : IRatingService
    {
        private readonly AppDbContext _context;

        private static readonly HashSet<string> ValidPeriodTypes = new(StringComparer.OrdinalIgnoreCase)
        {
            "daily",
            "weekly",
            "monthly",
            "3months",
            "1year"
        };

        public RatingService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<RatingResultDto> CreateRatingAsync(int userId, CreateRatingDto dto)
        {
            if (dto.PlayerId <= 0)
                throw new ArgumentException("Geçerli bir oyuncu seçilmelidir.");

            if (string.IsNullOrWhiteSpace(dto.PeriodType))
                throw new ArgumentException("PeriodType zorunludur.");

            dto.PeriodType = dto.PeriodType.Trim();

            if (!ValidPeriodTypes.Contains(dto.PeriodType))
                throw new ArgumentException("Geçersiz zaman aralığı.");

            if (dto.RatingValue < 0 || dto.RatingValue > 100)
                throw new ArgumentException("Rating değeri 0 ile 100 arasında olmalıdır.");

            var playerExists = await _context.Players.AnyAsync(p => p.PlayerId == dto.PlayerId);
            if (!playerExists)
                throw new KeyNotFoundException("Oyuncu bulunamadı.");

            var now = DateTime.UtcNow;

            var activeVote = await _context.Ratings
                .Where(r =>
                    r.UserId == userId &&
                    r.PlayerId == dto.PlayerId &&
                    r.PeriodType == dto.PeriodType &&
                    r.ExpiresAt > now)
                .OrderByDescending(r => r.ExpiresAt)
                .FirstOrDefaultAsync();

            if (activeVote != null)
            {
                throw new InvalidOperationException(
                    $"Bu zaman aralığı için tekrar oy veremezsiniz. Sonraki oy hakkı: {activeVote.ExpiresAt:yyyy-MM-dd HH:mm:ss} UTC");
            }

            var expiresAt = CalculateExpiresAt(dto.PeriodType, now);

            var rating = new Rating
            {
                UserId = userId,
                PlayerId = dto.PlayerId,
                RatingValue = dto.RatingValue,
                Comment = string.IsNullOrWhiteSpace(dto.Comment) ? null : dto.Comment.Trim(),
                PeriodType = dto.PeriodType,
                CreatedAt = now,
                ExpiresAt = expiresAt
            };

            _context.Ratings.Add(rating);
            await _context.SaveChangesAsync();

            return new RatingResultDto
            {
                RatingId = rating.RatingId,
                UserId = rating.UserId,
                PlayerId = rating.PlayerId,
                RatingValue = rating.RatingValue,
                PeriodType = rating.PeriodType,
                Comment = rating.Comment,
                CreatedAt = rating.CreatedAt,
                ExpiresAt = rating.ExpiresAt
            };
        }

        public async Task<List<PlayerCommentDto>> GetPlayerCommentsAsync(int playerId)
        {
            if (playerId <= 0)
                throw new ArgumentException("Geçerli bir oyuncu seçilmelidir.");

            var comments = await (
                from r in _context.Ratings
                join u in _context.Users on r.UserId equals u.UserId
                where r.PlayerId == playerId
                    && r.Comment != null
                    && r.Comment.Trim() != ""
                orderby (r.LikeCount + r.DislikeCount) descending, r.CreatedAt descending
                select new PlayerCommentDto
                {
                    RatingId = r.RatingId,
                    UserName = ((u.Name ?? "") + " " + (u.Surname ?? "")).Trim(),
                    Comment = r.Comment!,
                    LikeCount = r.LikeCount,
                    DislikeCount = r.DislikeCount,
                    CreatedAt = r.CreatedAt
                }
            ).ToListAsync();

            return comments;
        }
        public async Task<VoteAvailabilityDto> CheckVoteAvailabilityAsync(int userId, int playerId, string periodType)
        {
            if (playerId <= 0)
                throw new ArgumentException("Geçerli bir oyuncu seçilmelidir.");

            if (string.IsNullOrWhiteSpace(periodType))
                throw new ArgumentException("PeriodType zorunludur.");

            periodType = periodType.Trim();

            if (!ValidPeriodTypes.Contains(periodType))
                throw new ArgumentException("Geçersiz zaman aralığı.");

            var now = DateTime.UtcNow;

            var activeVote = await _context.Ratings
                .Where(r =>
                    r.UserId == userId &&
                    r.PlayerId == playerId &&
                    r.PeriodType == periodType &&
                    r.ExpiresAt > now)
                .OrderByDescending(r => r.ExpiresAt)
                .FirstOrDefaultAsync();

            if (activeVote == null)
            {
                return new VoteAvailabilityDto
                {
                    CanVote = true,
                    PlayerId = playerId,
                    PeriodType = periodType,
                    NextAvailableAt = null,
                    Message = "Bu zaman aralığı için oy verebilirsiniz."
                };
            }

            return new VoteAvailabilityDto
            {
                CanVote = false,
                PlayerId = playerId,
                PeriodType = periodType,
                NextAvailableAt = activeVote.ExpiresAt,
                Message = "Bu zaman aralığı için henüz tekrar oy veremezsiniz."
            };
        }


        private static DateTime CalculateExpiresAt(string periodType, DateTime createdAtUtc)
        {
            return periodType switch
            {
                "daily" => createdAtUtc.AddDays(1),
                "weekly" => createdAtUtc.AddDays(7),
                "monthly" => createdAtUtc.AddMonths(1),
                "3months" => createdAtUtc.AddMonths(3),
                "1year" => createdAtUtc.AddYears(1),
                _ => throw new ArgumentException("Geçersiz periodType")
            };
        }

        public async Task<RatingReactionResultDto> LikeCommentAsync(int userId, int ratingId)
        {
            if (ratingId <= 0)
                throw new ArgumentException("Geçerli bir rating seçilmelidir.");

            var rating = await _context.Ratings.FirstOrDefaultAsync(r => r.RatingId == ratingId);
            if (rating == null)
                throw new KeyNotFoundException("Yorum bulunamadı.");

            var existingReaction = await _context.RatingReactions
                .FirstOrDefaultAsync(rr => rr.RatingId == ratingId && rr.UserId == userId);

            string userReaction;

            if (existingReaction == null)
            {
                var reaction = new RatingReaction
                {
                    RatingId = ratingId,
                    UserId = userId,
                    ReactionType = "like",
                    CreatedAt = DateTime.UtcNow
                };

                _context.RatingReactions.Add(reaction);
                rating.LikeCount += 1;
                userReaction = "like";
            }
            else if (existingReaction.ReactionType == "like")
            {
                _context.RatingReactions.Remove(existingReaction);
                if (rating.LikeCount > 0)
                    rating.LikeCount -= 1;

                userReaction = "none";
            }
            else
            {
                existingReaction.ReactionType = "like";

                if (rating.DislikeCount > 0)
                    rating.DislikeCount -= 1;

                rating.LikeCount += 1;
                userReaction = "like";
            }

            await _context.SaveChangesAsync();

            return new RatingReactionResultDto
            {
                RatingId = rating.RatingId,
                LikeCount = rating.LikeCount,
                DislikeCount = rating.DislikeCount,
                UserReaction = userReaction
            };
        }

        public async Task<RatingReactionResultDto> DislikeCommentAsync(int userId, int ratingId)
        {
            if (ratingId <= 0)
                throw new ArgumentException("Geçerli bir rating seçilmelidir.");

            var rating = await _context.Ratings.FirstOrDefaultAsync(r => r.RatingId == ratingId);
            if (rating == null)
                throw new KeyNotFoundException("Yorum bulunamadı.");

            var existingReaction = await _context.RatingReactions
                .FirstOrDefaultAsync(rr => rr.RatingId == ratingId && rr.UserId == userId);

            string userReaction;

            if (existingReaction == null)
            {
                var reaction = new RatingReaction
                {
                    RatingId = ratingId,
                    UserId = userId,
                    ReactionType = "dislike",
                    CreatedAt = DateTime.UtcNow
                };

                _context.RatingReactions.Add(reaction);
                rating.DislikeCount += 1;
                userReaction = "dislike";
            }
            else if (existingReaction.ReactionType == "dislike")
            {
                _context.RatingReactions.Remove(existingReaction);
                if (rating.DislikeCount > 0)
                    rating.DislikeCount -= 1;

                userReaction = "none";
            }
            else
            {
                existingReaction.ReactionType = "dislike";

                if (rating.LikeCount > 0)
                    rating.LikeCount -= 1;

                rating.DislikeCount += 1;
                userReaction = "dislike";
            }

            await _context.SaveChangesAsync();

            return new RatingReactionResultDto
            {
                RatingId = rating.RatingId,
                LikeCount = rating.LikeCount,
                DislikeCount = rating.DislikeCount,
                UserReaction = userReaction
            };
        }
    }
}