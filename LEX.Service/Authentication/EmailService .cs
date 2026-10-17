using LEX.Service_Abstraction.IAuthentication;
using Microsoft.Extensions.Configuration;
using System.Net;
using System.Net.Mail;

namespace LEX.Service.Authentication
{
    public class EmailService : IEmailService
    {
        private readonly IConfiguration _configuration;

        public EmailService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task SendEmailAsync(string to, string subject, string body)
        {
            var host = Required("Email:Host");
            var username = Required("Email:Username");
            var password = Required("Email:Password");
            var from = _configuration["Email:From"];
            var fromName = _configuration["Email:FromName"];
            var port = int.TryParse(_configuration["Email:Port"], out var p) ? p : 587;

            using var smtp = new SmtpClient(host)
            {
                Port = port,
                Credentials = new NetworkCredential(username, password),
                EnableSsl = true
            };

            using var message = new MailMessage
            {
                From = new MailAddress(
                    string.IsNullOrWhiteSpace(from) ? username : from,
                    string.IsNullOrWhiteSpace(fromName) ? "LEX Support" : fromName),
                Subject = subject,
                Body = body,
                IsBodyHtml = true,
            };

            message.To.Add(to);

            await smtp.SendMailAsync(message);
        }

        private string Required(string key)
        {
            var value = _configuration[key];
            if (string.IsNullOrWhiteSpace(value))
                throw new InvalidOperationException(
                    $"Missing configuration value '{key}'. Set it with user-secrets or an environment variable.");
            return value;
        }
    }
}
