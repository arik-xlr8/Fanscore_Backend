using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FanScore.Application.DTOs.Player;

namespace FanScore.Application.Interfaces.Services
{
    public interface IPlayerService
    {
        Task<List<PlayerDto>> GetAllPlayersAsync(string? periodType);
        Task<PlayerDto?> GetPlayerByIdAsync(int playerId, string? periodType);
        Task<List<PlayerDto>> GetShuffledPlayersAsync(string? periodType);
        Task<List<PlayerDto>> SearchPlayersAsync(string? searchTerm, string? periodType);
    }
}