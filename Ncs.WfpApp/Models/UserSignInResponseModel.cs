using Ncs.WpfApp.Models.ApiResponse;

namespace Ncs.WpfApp.Models
{
    public class UserSignInResponseModel
    {
        public string Token { get; set; }
        public UsersDto User { get; set; }
    }
}
