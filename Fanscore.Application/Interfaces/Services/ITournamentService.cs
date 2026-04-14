using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Fanscore.Application.DTOs.Tournament;
using Fanscore.Application.DTOs.Tournement;

namespace Fanscore.Application.Interfaces.Services
{
    public interface ITournamentService
    {
        Task<List<TournamentListDto>> GetAllAsync();
        Task<TournamentDetailDto?> GetByIdAsync(int id);
        Task<TournamentDetailDto> CreateAsync(int userId, TournamentCreateDto dto);
        Task<bool> UpdateAsync(int id, int userId, TournamentUpdateDto dto);
        Task<bool> DeleteAsync(int id, int userId);
    }
}