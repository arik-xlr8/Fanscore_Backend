using FanScore.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace FanScore.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TeamController : ControllerBase
    {
        private readonly ITeamService _teamService;

        public TeamController(ITeamService teamService)
        {
            _teamService = teamService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllTeams()
        {
            var teams = await _teamService.GetAllTeamsAsync();
            return Ok(teams);
        }

        [HttpGet("{teamId}/detail")]
        public async Task<IActionResult> GetTeamDetail(int teamId, [FromQuery] string periodType = "daily")
        {
            var team = await _teamService.GetTeamDetailAsync(teamId, periodType);

            if (team == null)
                return NotFound("Takım bulunamadı.");

            return Ok(team);
        }
    }
}