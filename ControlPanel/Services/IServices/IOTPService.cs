using System.Threading.Tasks;

namespace ControlPanel.Services.IServices
{
    public interface IOTPService
    {
        Task SendOtpEmailAsync(string email, string otp);

        Task SendOtpSmsAsync(string phoneNumber, string otp);
    }
}
