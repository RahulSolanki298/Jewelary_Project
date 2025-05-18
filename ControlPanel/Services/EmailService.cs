using System.Net.Mail;
using System.Net;
using System.Threading.Tasks;
using AutoMapper.Configuration;

namespace ControlPanel.Services
{
    public class EmailService
    {
        private readonly IConfiguration _configuration;

        public EmailService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task SendOtpEmailAsync(string email, string otp)
        {
            var fromAddress = new MailAddress("youremail@example.com", "Your App Name");
            var toAddress = new MailAddress(email);
            string fromPassword = "your-email-password"; // Consider using a secure secret manager
            string subject = "Your OTP Code";
            string body = $"Your OTP code is: <b>{otp}</b>";

            var smtp = new SmtpClient
            {
                Host = "smtp.gmail.com", // e.g., Gmail SMTP
                Port = 587,
                EnableSsl = true,
                DeliveryMethod = SmtpDeliveryMethod.Network,
                UseDefaultCredentials = false,
                Credentials = new NetworkCredential(fromAddress.Address, fromPassword)
            };

            using (var message = new MailMessage(fromAddress, toAddress)
            {
                Subject = subject,
                Body = body,
                IsBodyHtml = true
            })
            {
                await smtp.SendMailAsync(message);
            }
        }
    }

}
