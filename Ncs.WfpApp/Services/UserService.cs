using AutoMapper;
using Ncs.WpfApp.Helpers;
using Ncs.WpfApp.Models;
using Ncs.WpfApp.Models.ApiResponse;
using Ncs.WpfApp.Services.Interfaces;
using Newtonsoft.Json;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;

namespace Ncs.WpfApp.Services
{
    public class UserService : IUserService
    {
        private readonly HttpClient _httpClient;
        private readonly IMapper _mapper;
        public UserService(HttpClient httpClient, IMapper mapper)
        {
            _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
            _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            _mapper = mapper;
        }

        public async Task<ApiResponseModel<UserSignInResponseModel>> SignInAsync(UserSignInModel user)
        {
            var requestBody = new { UsernameOrEmail = user.Username, Password = user.Password };
            var content = new StringContent(JsonConvert.SerializeObject(requestBody), Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync("/api/auth/login", content);
            // Read response content as string
            var responseContent = await response.Content.ReadAsStringAsync();

            // Deserialize into generic response type
            var apiResponse = JsonConvert.DeserializeObject<ApiResponseModel<UserSignInResponseModel>>(responseContent);

            // Ensure apiResponse is not null
            if (apiResponse == null)
            {
                return new ApiResponseModel<UserSignInResponseModel>()
                {
                    Success=false,
                    Message= "Failed to deserialize API response.",
                    MessageDetail= "Failed to deserialize API response."
                };
            }

            return apiResponse;
        }
        public async Task<ApiResponseModel<IEnumerable<UsersDto>>> GetUsersAsync(string searchText)
        {
            if (string.IsNullOrEmpty(SessionManager.Token))
            {
                return new ApiResponseModel<IEnumerable<UsersDto>>
                {
                    Success = false,
                    Message = $"Token is required to fetch user information.",
                    MessageDetail = $"Token is required to fetch user information."
                };
            }
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", SessionManager.Token);
            var response = await _httpClient.GetAsync("/api/users");

            var responseContent = await response.Content.ReadAsStringAsync();

            var apiResponse = JsonConvert.DeserializeObject<ApiResponseModel<IEnumerable<UsersDto>>>(responseContent);

            if (apiResponse == null)
            {
                return new ApiResponseModel<IEnumerable<UsersDto>>
                {
                    Success = false,
                    Message = $"Fetching users failed: {apiResponse?.Message ?? "Unknown error"}",
                    MessageDetail = $"Fetching users failed: {apiResponse?.Message ?? "Unknown error"}"
                };
            }
            else if (!apiResponse.Success)
            {
                return new ApiResponseModel<IEnumerable<UsersDto>>
                {
                    Success = false,
                    Message = apiResponse.Message,
                    MessageDetail = apiResponse.MessageDetail
                };
            }
            if (string.IsNullOrEmpty(searchText))
            {
                return apiResponse;
            }
            else
            {
                var filteredUsers = apiResponse?.Data?.Where(u =>
                        (!string.IsNullOrWhiteSpace(searchText) &&
                        (
                            u.Fullname.Contains(searchText, StringComparison.OrdinalIgnoreCase) ||
                            (u.Company?.Name ?? "").Contains(searchText, StringComparison.OrdinalIgnoreCase) ||
                            (u.GuestCompanyName ?? "").Contains(searchText, StringComparison.OrdinalIgnoreCase) ||
                            (u.RfidTag ?? "").Contains(searchText, StringComparison.OrdinalIgnoreCase)
                        ))
                    ).ToList();

                return new ApiResponseModel<IEnumerable<UsersDto>>
                {
                    Success = filteredUsers != null,
                    Data = filteredUsers,
                    Message = filteredUsers != null ? "Users found." : "No users found matching the criteria."
                };
            }
        }
        public async Task<ApiResponseModel<IEnumerable<CompaniesDto>>> GetCompaniesAsync()
        {
            if (string.IsNullOrEmpty(SessionManager.Token))
            {
                return new ApiResponseModel<IEnumerable<CompaniesDto>>
                {
                    Success = false,
                    Message = $"Token is required to fetch data information.",
                    MessageDetail = $"Token is required to fetch data information."
                };
            }
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", SessionManager.Token);
            var response = await _httpClient.GetAsync("/api/masters/companies");

            var responseContent = await response.Content.ReadAsStringAsync();

            var apiResponse = JsonConvert.DeserializeObject<ApiResponseModel<IEnumerable<CompaniesDto>>>(responseContent);

            if (apiResponse == null)
            {
                return new ApiResponseModel<IEnumerable<CompaniesDto>>
                {
                    Success = false,
                    Message = $"Fetching data failed: {apiResponse?.Message ?? "Unknown error"}",
                    MessageDetail = $"Fetching data failed: {apiResponse?.Message ?? "Unknown error"}"
                };
            }
            return apiResponse;
        }
        public async Task<ApiResponseModel<IEnumerable<PersonalIdTypeDto>>> GetPersonalIdTypesAsync()
        {
            if (string.IsNullOrEmpty(SessionManager.Token))
            {
                return new ApiResponseModel<IEnumerable<PersonalIdTypeDto>>
                {
                    Success = false,
                    Message = $"Token is required to fetch data information.",
                    MessageDetail = $"Token is required to fetch data information."
                };
            }
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", SessionManager.Token);
            var response = await _httpClient.GetAsync("/api/masters/personal-types");

            var responseContent = await response.Content.ReadAsStringAsync();

            var apiResponse = JsonConvert.DeserializeObject<ApiResponseModel<IEnumerable<PersonalIdTypeDto>>>(responseContent);

            if (apiResponse == null)
            {
                return new ApiResponseModel<IEnumerable<PersonalIdTypeDto>>
                {
                    Success = false,
                    Message = $"Fetching data failed: {apiResponse?.Message ?? "Unknown error"}",
                    MessageDetail = $"Fetching data failed: {apiResponse?.Message ?? "Unknown error"}"
                };
            }
            return apiResponse;
        }
        public async Task<ApiResponseModel<IEnumerable<RolesDto>>> GetRolesAsync()
        {
            if (string.IsNullOrEmpty(SessionManager.Token))
            {
                return new ApiResponseModel<IEnumerable<RolesDto>>
                {
                    Success = false,
                    Message = $"Token is required to fetch data information.",
                    MessageDetail = $"Token is required to fetch data information."
                };
            }
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", SessionManager.Token);
            var response = await _httpClient.GetAsync("/api/masters/roles");

            var responseContent = await response.Content.ReadAsStringAsync();

            var apiResponse = JsonConvert.DeserializeObject<ApiResponseModel<IEnumerable<RolesDto>>>(responseContent);

            if (apiResponse == null)
            {
                return new ApiResponseModel<IEnumerable<RolesDto>>
                {
                    Success = false,
                    Message = $"Fetching data failed: {apiResponse?.Message ?? "Unknown error"}",
                    MessageDetail = $"Fetching data failed: {apiResponse?.Message ?? "Unknown error"}"
                };
            }
            return apiResponse;
        }

        public async Task<ApiResponseModel<bool>> SaveUserAsync(UserAddModel user)

        {
            if (string.IsNullOrEmpty(SessionManager.Token))
            {
                return new ApiResponseModel<bool>
                {
                    Success = false,
                    Message = $"Token is required to fetch data information.",
                    MessageDetail = $"Token is required to fetch data information."
                };
            }
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", SessionManager.Token);

            var jsonContent = JsonConvert.SerializeObject(user);
            var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync("/api/users", content);
            var responseContent = await response.Content.ReadAsStringAsync();
            var apiResponse = JsonConvert.DeserializeObject<ApiResponseModel<bool>>(responseContent);

            if (apiResponse == null)
            {
                return new ApiResponseModel<bool>
                {
                    Success = false,
                    Message = $"Save users failed: {apiResponse?.Message ?? "Unknown error"}",
                    MessageDetail = $"Save users failed: {apiResponse?.Message ?? "Unknown error"}"
                };
            }
            else if (!apiResponse.Success)
            {
                return new ApiResponseModel<bool>
                {
                    Success = false,
                    Message = apiResponse.Message,
                    MessageDetail = apiResponse.MessageDetail
                };
            }


            return apiResponse;
        }

    }
}
