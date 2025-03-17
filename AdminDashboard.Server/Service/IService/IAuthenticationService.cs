using Models;
using System.Threading.Tasks;

namespace AdminDashboard.Server.Service.IService
{
    public interface IAuthenticationService
    {
        Task<AuthenticationResponseDTO> AdminSignInAsync(AdminLoginModel loginDTO);

        Task LogoutAsync();

        Task<bool> IsUserAuthenticated();
    }
}
