using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Fanscore.Application.DTOs;

namespace Fanscore.Application.Interfaces.Services
{
    public interface ICityService
    {
        Task<List<CityDto>> GetAllCitiesAsync();
    }
}