using ClientManagement.Models.Models.Authentication;
using Microsoft.AspNetCore.Identity;

namespace ClientManagement.BusinessLogic.Repository.Contracts
{
    public interface IAccountRepository
    {
        Task<IdentityResult> SignUpAsync(SignUpModel signUpModel);

        Task<string> LoginAsync(SignInModel signInModel);
    }
}