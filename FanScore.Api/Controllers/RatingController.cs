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
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(new { message = ex.Message });
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
            catch (Exception)
            {
                return StatusCode(500, new { message = "Oy gönderilirken beklenmeyen bir hata oluştu." });
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
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(new { message = ex.Message });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception)
            {
                return StatusCode(500, new { message = "Oy hakkı kontrol edilirken beklenmeyen bir hata oluştu." });
            }
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
            catch (Exception)
            {
                return StatusCode(500, new { message = "Yorumlar getirilirken beklenmeyen bir hata oluştu." });
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
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(new { message = ex.Message });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (Exception)
            {
                return StatusCode(500, new { message = "Beğeni işlemi yapılırken beklenmeyen bir hata oluştu." });
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
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(new { message = ex.Message });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (Exception)
            {
                return StatusCode(500, new { message = "Dislike işlemi yapılırken beklenmeyen bir hata oluştu." });
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
            {
                throw new UnauthorizedAccessException("Giriş yapmadınız. Lütfen tekrar giriş yapın.");
            }

            return userId;
        }
    }
}