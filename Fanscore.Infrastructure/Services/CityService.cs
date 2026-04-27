using Fanscore.Application.DTOs;
using Fanscore.Application.Interfaces.Services;
using FanScore.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Fanscore.Infrastructure.Services
{
    public class CityService : ICityService
    {
        private readonly AppDbContext _context;

        public CityService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<CityDto>> GetAllCitiesAsync()
        {
            return await _context.Cities
                .OrderBy(c => c.PlateCode)
                .Select(c => new CityDto
                {
                    CityId = c.CityId,
                    PlateCode = c.PlateCode,
                    CityName = c.CityName
                })
                .ToListAsync();
        }
    }
}