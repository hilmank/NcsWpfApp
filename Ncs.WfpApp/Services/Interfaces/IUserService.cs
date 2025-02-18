using Ncs.WpfApp.Models;
using Ncs.WpfApp.Models.ApiResponse;

namespace Ncs.WpfApp.Services.Interfaces
{
    public interface IUserService
    {
        Task<ApiResponseModel<UserSignInResponseModel>> SignInAsync(UserSignInModel user);
        Task<ApiResponseModel<IEnumerable<UsersDto>>> GetUsersAsync(string searchText);
        Task<ApiResponseModel<IEnumerable<CompaniesDto>>>  GetCompaniesAsync();
        Task<ApiResponseModel<IEnumerable<PersonalIdTypeDto>>> GetPersonalIdTypesAsync();
        Task<ApiResponseModel<IEnumerable<RolesDto>>> GetRolesAsync();

        Task<ApiResponseModel<bool>> SaveUserAsync(UserAddModel user);

    }
}
