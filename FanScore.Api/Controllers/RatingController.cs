using System.Security.Claims;
using Fanscore.Application.DTOs.Rating;
using Fanscore.Application.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FanScore.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class RatingController : ControllerBase
    {
        private readonly IRatingService _ratingService;

        public RatingController(IRatingService ratingService)
        {
            _ratingService = ratingService;
        }

        [HttpPost]
        public async Task<IActionResult> CreateRating([FromBody] CreateRatingDto dto)
        {
            try
            {
                var userId = GetUserIdFromClaims();
                var result = await _ratingService.CreateRatingAsync(userId, dto);
                return Ok(result);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(new { message = ex.Message });
            }
        }

        [HttpGet("can-vote")]
        public async Task<IActionResult> CanVote([FromQuery] int playerId, [FromQuery] string periodType)
        {
            try
            {
                var userId = GetUserIdFromClaims();
                var result = await _ratingService.CheckVoteAvailabilityAsync(userId, playerId, periodType);
                return Ok(result);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }


        private int GetUserIdFromClaims()
        {
            var userIdClaim =
                User.FindFirst(ClaimTypes.NameIdentifier)?.Value ??
                User.FindFirst("nameid")?.Value ??
                User.FindFirst("sub")?.Value ??
                User.FindFirst("userId")?.Value;

            if (string.IsNullOrWhiteSpace(userIdClaim) || !int.TryParse(userIdClaim, out var userId))
                throw new UnauthorizedAccessException("Token içinden kullanıcı bilgisi alınamadı.");

            return userId;
        }

        [AllowAnonymous]
        [HttpGet("comments/{playerId}")]
        public async Task<IActionResult> GetPlayerComments(int playerId)
        {
            try
            {
                var result = await _ratingService.GetPlayerCommentsAsync(playerId);
                return Ok(result);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPost("{ratingId}/like")]
        public async Task<IActionResult> LikeComment(int ratingId)
        {
            try
            {
                var userId = GetUserIdFromClaims();
                var result = await _ratingService.LikeCommentAsync(userId, ratingId);
                return Ok(result);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
        }

        [HttpPost("{ratingId}/dislike")]
        public async Task<IActionResult> DislikeComment(int ratingId)
        {
            try
            {
                var userId = GetUserIdFromClaims();
                var result = await _ratingService.DislikeCommentAsync(userId, ratingId);
                return Ok(result);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
        }
    }
}