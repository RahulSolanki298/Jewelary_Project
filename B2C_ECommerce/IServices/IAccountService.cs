using DataAccess.Entities;
using Models;
using System.Threading.Tasks;

namespace B2C_ECommerce.IServices
{
    public interface IAccountService
    {
        Task<ApplicationUser> SupplierSignUpAsync(SupplierRegisterDTO userRequestDTO);

        Task<ApplicationUser> CustomerSignUpAsync(UserRequestDTO userRequestDTO);

        Task<AuthenticationResponseDTO> CustomerSignInAsync(CustomerLoginDTO loginDTO);
    }
}
