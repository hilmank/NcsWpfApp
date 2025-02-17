using Ncs.WfpApp.Models;
using Ncs.WfpApp.Services.Interfaces;

namespace Ncs.WfpApp.Services
{
    public class UserService : IUserService
    {
        public async Task<bool> SignInAsync(UserSignInModel user)
        {
            await Task.Delay(500); // Simulate API call delay
            return user.Username == "admin" && user.Password == "12345"; // Mock authentication
        }
    }
}
