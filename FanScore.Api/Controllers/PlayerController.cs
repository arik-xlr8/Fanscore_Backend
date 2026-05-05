using FanScore.Application.Interfaces.Services;
using Microsoft.AspNetCore.Mvc;

namespace FanScore.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PlayerController : ControllerBase
    {
        private readonly IPlayerService _playerService;

        public PlayerController(IPlayerService playerService)
        {
            _playerService = playerService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllPlayers([FromQuery] string? periodType)
        {
            var players = await _playerService.GetAllPlayersAsync(periodType);
            return Ok(players);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetPlayerById(int id, [FromQuery] string? periodType)
        {
            var player = await _playerService.GetPlayerByIdAsync(id, periodType);

            if (player == null)
                return NotFound(new { message = "Oyuncu bulunamadı." });

            return Ok(player);
        }

        [HttpGet("shuffle")]
        public async Task<IActionResult> GetShuffledPlayers([FromQuery] string? periodType)
        {
            var players = await _playerService.GetShuffledPlayersAsync(periodType);
            return Ok(players);
        }

        [HttpGet("search")]
        public async Task<IActionResult> SearchPlayers([FromQuery] string? searchTerm, [FromQuery] string? periodType)
        {
            var players = await _playerService.SearchPlayersAsync(searchTerm, periodType);
            return Ok(players);
        }

        [HttpGet("paged")]
        public async Task<IActionResult> GetAllPlayersPaged(
            [FromQuery] string? periodType,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 100)
        {
            var result = await _playerService.GetAllPlayersPagedAsync(periodType, page, pageSize);
            return Ok(result);
        }

        [HttpGet("search-paged")]
        public async Task<IActionResult> SearchPlayersPaged(
            [FromQuery] string? searchTerm,
            [FromQuery] string? periodType,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 100)
        {
            var result = await _playerService.SearchPlayersPagedAsync(searchTerm, periodType, page, pageSize);
            return Ok(result);
        }

        [HttpGet("shuffle-paged")]
        public async Task<IActionResult> GetShuffledPlayersPaged(
            [FromQuery] string? periodType,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 100,
            [FromQuery] int shuffleSeed = 1)
        {
            var result = await _playerService.GetShuffledPlayersPagedAsync(
                periodType,
                page,
                pageSize,
                shuffleSeed
            );

            return Ok(result);
        }
    }
}