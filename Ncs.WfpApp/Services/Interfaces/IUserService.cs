using Ncs.WfpApp.Models;

namespace Ncs.WfpApp.Services.Interfaces
{
    public interface IUserService
    {
        Task<bool> SignInAsync(UserSignInModel user);
    }
}
