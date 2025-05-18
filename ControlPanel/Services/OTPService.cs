using ControlPanel.Services.IServices;
using System;
using System.Threading.Tasks;

namespace ControlPanel.Services
{
    public class OTPService : IOTPService
    {
        public Task SendOtpEmailAsync(string email, string otp)
        {
            Console.WriteLine($"Sending OTP {otp} to Email: {email}");
            return Task.CompletedTask;
        }

        public Task SendOtpSmsAsync(string phoneNumber, string otp)
        {
            Console.WriteLine($"Sending OTP {otp} to Phone: {phoneNumber}");
            return Task.CompletedTask;

        }
    }
}
