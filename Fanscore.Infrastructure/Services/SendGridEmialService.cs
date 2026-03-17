using Fanscore.Application.Interfaces.Services;
using Microsoft.Extensions.Configuration;
using SendGrid;
using SendGrid.Helpers.Mail;

namespace FanScore.Infrastructure.Services
{
    public class SendGridEmailService : IEmailService
    {
        private readonly IConfiguration _configuration;

        public SendGridEmailService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task SendVerificationEmailAsync(string toEmail, string name, string verificationLink)
        {
            var apiKey = _configuration["SendGrid:ApiKey"];
            var fromEmail = _configuration["SendGrid:FromEmail"];
            var fromName = _configuration["SendGrid:FromName"];
            var templateId = _configuration["SendGrid:VerificationTemplateId"];

            if (string.IsNullOrWhiteSpace(apiKey))
                throw new Exception("SendGrid ApiKey bulunamadı.");

            if (string.IsNullOrWhiteSpace(templateId))
                throw new Exception("SendGrid VerificationTemplateId bulunamadı.");

            if (string.IsNullOrWhiteSpace(fromEmail))
                throw new Exception("SendGrid FromEmail bulunamadı.");

            var client = new SendGridClient(apiKey);

            var from = new EmailAddress(fromEmail, fromName);
            var to = new EmailAddress(toEmail);

            var msg = MailHelper.CreateSingleTemplateEmail(
                from,
                to,
                templateId,
                new
                {
                    name = name,
                    verification_link = verificationLink
                });

            var response = await client.SendEmailAsync(msg);

            if ((int)response.StatusCode >= 400)
            {
                var body = await response.Body.ReadAsStringAsync();
                throw new Exception($"Mail gönderilemedi. Status: {response.StatusCode}, Detail: {body}");
            }
        }
    }
}