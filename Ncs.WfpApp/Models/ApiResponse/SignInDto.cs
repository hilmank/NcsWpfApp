namespace Ncs.WpfApp.Models.ApiResponse
{
    public class SignInDto
    {
        public required string Token { get; set; }
        public required UsersDto User { get; set; }
    }
}
