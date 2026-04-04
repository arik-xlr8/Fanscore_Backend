using Fanscore.Application.DTOs.Team;

namespace FanScore.Application.Interfaces
{
    public interface ITeamService
    {
        Task<List<TeamListDto>> GetAllTeamsAsync();
        Task<TeamDetailDto?> GetTeamDetailAsync(int teamId, string periodType);
    }
}