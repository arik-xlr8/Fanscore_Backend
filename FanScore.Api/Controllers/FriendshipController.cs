using System.Security.Claims;
using Fanscore.Application.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FanScore.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class FriendshipController : ControllerBase
    {
        private readonly IFriendshipService _friendshipService;

        public FriendshipController(IFriendshipService friendshipService)
        {
            _friendshipService = friendshipService;
        }

        [HttpGet("search")]
        public async Task<IActionResult> SearchUsers([FromQuery] string? q)
        {
            return Ok(await _friendshipService.SearchUsersAsync(GetUserId(), q));
        }

        [HttpGet]
        public async Task<IActionResult> GetFriends()
        {
            return Ok(await _friendshipService.GetFriendsAsync(GetUserId()));
        }

        [HttpGet("requests")]
        public async Task<IActionResult> GetIncomingRequests()
        {
            return Ok(await _friendshipService.GetIncomingRequestsAsync(GetUserId()));
        }

        [HttpPost("{friendUserId}")]
        public async Task<IActionResult> AddFriend(int friendUserId)
        {
            try
            {
                var result = await _friendshipService.AddFriendAsync(GetUserId(), friendUserId);
                if (!result)
                    return NotFound(new { message = "Kullanici bulunamadi." });

                return Ok(new { message = "Arkadaslik istegi gonderildi." });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPut("requests/{requesterUserId}/accept")]
        public async Task<IActionResult> AcceptFriendRequest(int requesterUserId)
        {
            var result = await _friendshipService.AcceptFriendRequestAsync(GetUserId(), requesterUserId);
            if (!result)
                return NotFound(new { message = "Arkadaslik istegi bulunamadi." });

            return Ok(new { message = "Arkadaslik istegi kabul edildi." });
        }

        [HttpDelete("requests/{requesterUserId}/reject")]
        public async Task<IActionResult> RejectFriendRequest(int requesterUserId)
        {
            var result = await _friendshipService.RejectFriendRequestAsync(GetUserId(), requesterUserId);
            if (!result)
                return NotFound(new { message = "Arkadaslik istegi bulunamadi." });

            return Ok(new { message = "Arkadaslik istegi reddedildi." });
        }

        [HttpDelete("{friendUserId}")]
        public async Task<IActionResult> RemoveFriend(int friendUserId)
        {
            var result = await _friendshipService.RemoveFriendAsync(GetUserId(), friendUserId);
            if (!result)
                return NotFound(new { message = "Arkadaslik bulunamadi." });

            return Ok(new { message = "Arkadas cikarildi." });
        }

        [HttpGet("{friendUserId}/ratings")]
        public async Task<IActionResult> GetFriendRatings(int friendUserId)
        {
            try
            {
                return Ok(await _friendshipService.GetFriendRatingHistoryAsync(GetUserId(), friendUserId));
            }
            catch (UnauthorizedAccessException ex)
            {
                return Forbid(ex.Message);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
        }

        private int GetUserId()
        {
            var value = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrWhiteSpace(value) || !int.TryParse(value, out var userId))
                throw new UnauthorizedAccessException("Token icinden kullanici bilgisi alinamadi.");

            return userId;
        }
    }
}
