using Microsoft.AspNetCore.Identity;
using Models;
using System.Threading.Tasks;

namespace Business.Repository.IRepository
{
    public interface IAccountRepository
    {
        Task<IdentityResult> CustomerRegisterAsync(CustomerRegisterDTO customerRegister);

        Task<SignInResult> CustomerLoginAsync(CustomerLoginDTO customerLogin);

        Task<IdentityResult> UpsertBusinessAccountAsync(BusinessAccountRegisterDTO customerRegister);


    }
}
