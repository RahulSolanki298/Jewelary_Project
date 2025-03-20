using Models;
using System.Threading.Tasks;

namespace B2C_ECommerce.IServices
{
    public interface IAccountService
    {
        Task<RegisterationResponseDTO> CustomerSignUpAsync(UserRequestDTO userRequestDTO);

        Task<AuthenticationResponseDTO> CustomerSignInAsync(CustomerLoginDTO loginDTO);

        Task<RegisterationResponseDTO> SupplierSignUpAsync(UserRequestDTO userRequestDTO);
    }
}
