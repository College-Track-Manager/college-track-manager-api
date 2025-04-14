using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;

namespace CollegeTrackAPI.Services
{
    public class EmailSender : IEmailSender
    {
        private readonly string _smtpServer = "smtp.your-email-provider.com"; // Update with your SMTP server
        private readonly string _smtpUser = "your-email@example.com"; // Update with your email address
        private readonly string _smtpPassword = "your-email-password"; // Update with your email password

        public async Task SendEmailAsync(string email, string subject, string message)
        {
            using (var client = new SmtpClient(_smtpServer))
            {
                client.Port = 587; // Standard SMTP port for TLS
                client.Credentials = new NetworkCredential(_smtpUser, _smtpPassword);
                client.EnableSsl = true;

                var mailMessage = new MailMessage
                {
                    From = new MailAddress(_smtpUser),
                    Subject = subject,
                    Body = message,
                    IsBodyHtml = true,
                };

                mailMessage.To.Add(email);

                await client.SendMailAsync(mailMessage);
            }
        }
    }
}
