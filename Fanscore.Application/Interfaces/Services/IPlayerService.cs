using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FanScore.Application.DTOs.Player;

namespace FanScore.Application.Interfaces.Services
{
    public interface IPlayerService
    {
        Task<List<PlayerDto>> GetAllPlayersAsync();
        Task<PlayerDto?> GetPlayerByIdAsync(int playerId);
    }
}