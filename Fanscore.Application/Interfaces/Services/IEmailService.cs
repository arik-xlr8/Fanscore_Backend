using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Fanscore.Application.Interfaces.Services
{
    public interface IEmailService
    {
        Task SendVerificationEmailAsync(string toEmail, string name, string verificationLink);
    }
}