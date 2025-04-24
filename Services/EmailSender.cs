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
            var smtpPort = int.Parse(_config["Email:SmtpPort"] ?? "587");

            using (var client = new SmtpClient(smtpServer))
            {
                client.Port = smtpPort;
                client.Credentials = new NetworkCredential(smtpUser, smtpPassword);
                client.EnableSsl = true;

                var mailMessage = new MailMessage
                {
                    From = new MailAddress(smtpUser),
                    Subject = subject,
                    Body = message,
                    IsBodyHtml = true,
                };

                mailMessage.To.Add(email);
                try
                {
                    await client.SendMailAsync(mailMessage);
                }
                catch (Exception ex)
                {
                    Console.WriteLine("EMAIL ERROR: " + ex.Message);
                    throw;  // or return BadRequest(ex.Message) in your controller
                }
            }
        }
    }
}
