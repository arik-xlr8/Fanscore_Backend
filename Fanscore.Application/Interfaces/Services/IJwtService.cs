using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FanScore.Application.Interfaces.Services
{
    public interface IJwtService
    {
        string CreateToken(int userId, string email, string role);
    }
}