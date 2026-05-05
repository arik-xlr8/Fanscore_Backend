using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Fanscore.Application.DTOs.Player;
using FanScore.Application.DTOs.Admin;
using FanScore.Application.DTOs.Player;

namespace FanScore.Application.Interfaces.Services
{
    public interface IPlayerService
    {
        Task<List<PlayerDto>> GetAllPlayersAsync(string? periodType);
        Task<PlayerDto?> GetPlayerByIdAsync(int playerId, string? periodType);
        Task<List<PlayerDto>> GetShuffledPlayersAsync(string? periodType);
        Task<List<PlayerDto>> SearchPlayersAsync(string? searchTerm, string? periodType);
        Task<PagedResultDto<PlayerDto>> GetAllPlayersPagedAsync(string? periodType, int page, int pageSize);
        Task<PagedResultDto<PlayerDto>> SearchPlayersPagedAsync(string? searchTerm, string? periodType, int page, int pageSize);
        Task<PagedResultDto<PlayerDto>> GetShuffledPlayersPagedAsync(
            string? periodType,
            int page,
            int pageSize,
            int shuffleSeed
        );
        Task<PlayerDto> CreatePlayerAsync(AdminCreatePlayerDto dto);
        Task<PlayerDto?> UpdatePlayerAsync(int playerId, AdminUpdatePlayerDto dto);
        Task<bool> DeletePlayerAsync(int playerId);
    }
}
