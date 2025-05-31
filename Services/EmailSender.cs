using Microsoft.Extensions.Configuration;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;

namespace CollegeTrackAPI.Services
{
    public class EmailSender : IEmailSender
    {
        private readonly IConfiguration _config;

        public EmailSender(IConfiguration config)
        {
            _config = config;
        }

        public async Task SendEmailAsync(string email, string subject, string message)
        {
            var smtpServer = _config["Email:SmtpServer"];
            var smtpUser = _config["Email:SmtpUser"];
            var smtpPassword = _config["Email:SmtpPassword"];
            var smtpPortRaw = _config["Email:SmtpPort"];

            // Validate critical fields
            if (string.IsNullOrWhiteSpace(smtpServer) ||
                string.IsNullOrWhiteSpace(smtpUser) ||
                string.IsNullOrWhiteSpace(smtpPassword) ||
                !int.TryParse(smtpPortRaw, out int smtpPort))
            {
                throw new InvalidOperationException("Invalid SMTP configuration.");
            }

            using var client = new SmtpClient(smtpServer)
            {
                Port = smtpPort,
                Credentials = new NetworkCredential(smtpUser, smtpPassword),
                EnableSsl = true
            };

            var mailMessage = new MailMessage
            {
                From = new MailAddress(smtpUser),
                Subject = subject,
                Body = message,
                IsBodyHtml = true
            };

            mailMessage.To.Add(email);

            try
            {
                await client.SendMailAsync(mailMessage);
            }
            catch (Exception ex)
            {
                Console.WriteLine("EMAIL ERROR: " + ex.Message);
                throw;  // Let the controller decide how to handle
            }
        }

    }
}
