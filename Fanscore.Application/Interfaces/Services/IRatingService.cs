using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Fanscore.Application.DTOs.Rating;

namespace Fanscore.Application.Interfaces.Services
{
    public interface IRatingService
    {
        Task<RatingResultDto> CreateRatingAsync(int userId, CreateRatingDto dto);
        Task<VoteAvailabilityDto> CheckVoteAvailabilityAsync(int userId, int playerId, string periodType);
        Task<List<PlayerCommentDto>> GetPlayerCommentsAsync(int playerId);
        Task<RatingReactionResultDto> LikeCommentAsync(int userId, int ratingId);
        Task<RatingReactionResultDto> DislikeCommentAsync(int userId, int ratingId);
    }
}