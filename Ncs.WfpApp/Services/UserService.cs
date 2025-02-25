using AutoMapper;
using Ncs.WpfApp.Helpers;
using Ncs.WpfApp.Models;
using Ncs.WpfApp.Models.ApiResponse;
using Ncs.WpfApp.Services.Interfaces;
using Newtonsoft.Json;
using System.Net.Http;
using System.Text;

namespace Ncs.WpfApp.Services
{
    public class UserService : IUserService
    {
        private readonly IMapper _mapper;
        public UserService(IMapper mapper)
        {
            _mapper = mapper;
        }

        public async Task<ApiResponseModel<UserSignInResponseModel>> SignInAsync(UserSignInModel user)
        {
            //var requestBody = new { UsernameOrEmail = user.UsernameOrEmail, Password = user.Password };
            //var content = new StringContent(JsonConvert.SerializeObject(requestBody), Encoding.UTF8, "application/json");

            var response = await HttpClientHelper.PostAsync($"{ConfigurationHelper.GetApiVersion()}/auth/login", user);

            var (message, messageDetail) = await ApiResponseHandler.HandleApiResponse(response);
            if (message != "Success")
            {
                return new ApiResponseModel<UserSignInResponseModel>
                {
                    Success = false,
                    Message = message,
                    MessageDetail = messageDetail
                };
            }

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
            SessionManager.SetAdminToken(apiResponse.Data.Token, apiResponse.Data.Token);
            return apiResponse;
        }
        public async Task<ApiResponseModel<UserSignInResponseModel>> RfidSignInAsync(string rfidTag)
        {
            var requestBody = new { RfidTagId = rfidTag };
            var response = await HttpClientHelper.PostAsync($"{ConfigurationHelper.GetApiVersion()}/auth/rfid-login", requestBody);
            var (message, messageDetail) = await ApiResponseHandler.HandleApiResponse(response);
            if (message != "Success")
            {
                return new ApiResponseModel<UserSignInResponseModel>
                {
                    Success = false,
                    Message = message,
                    MessageDetail = messageDetail
                };
            }

            var responseContent = await response.Content.ReadAsStringAsync();
            var apiResponse = JsonConvert.DeserializeObject<ApiResponseModel<UserSignInResponseModel>>(responseContent);
            if (apiResponse == null)
            {
                return new ApiResponseModel<UserSignInResponseModel>()
                {
                    Success = false,
                    Message = "Failed to deserialize API response.",
                    MessageDetail = "Failed to deserialize API response."
                };
            }
            SessionManager.SetCustomerToken(apiResponse.Data.Token, apiResponse.Data.Token);
            return apiResponse;
        }

        public async Task<ApiResponseModel<IEnumerable<UsersDto>>> GetUsersAsync(string searchText)
        {
            if (string.IsNullOrEmpty(SessionManager.AdminToken) && string.IsNullOrEmpty(SessionManager.CustomerToken))
            {
                return new ApiResponseModel<IEnumerable<UsersDto>>
                {
                    Success = false,
                    Message = "Token is required to fetch user information.",
                    MessageDetail = "User must be signed in to access this resource."
                };
            }

            var response = await HttpClientHelper.GetAsync($"{ConfigurationHelper.GetApiVersion()}/users");
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
            if (string.IsNullOrEmpty(SessionManager.AdminToken) && string.IsNullOrEmpty(SessionManager.CustomerToken))
            {
                return new ApiResponseModel<IEnumerable<CompaniesDto>>
                {
                    Success = false,
                    Message = "Token is required.",
                    MessageDetail = "User must be signed in to access this resource."
                };
            }

            var response = await HttpClientHelper.GetAsync($"{ConfigurationHelper.GetApiVersion()}/masters/companies");

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
            if (string.IsNullOrEmpty(SessionManager.AdminToken) && string.IsNullOrEmpty(SessionManager.CustomerToken))
            {
                return new ApiResponseModel<IEnumerable<PersonalIdTypeDto>>
                {
                    Success = false,
                    Message = "Token is required.",
                    MessageDetail = "User must be signed in to access this resource."
                };
            }

            var response = await HttpClientHelper.GetAsync($"{ConfigurationHelper.GetApiVersion()}/masters/personal-types");

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
            if (string.IsNullOrEmpty(SessionManager.AdminToken) && string.IsNullOrEmpty(SessionManager.CustomerToken))
            {
                return new ApiResponseModel<IEnumerable<RolesDto>>
                {
                    Success = false,
                    Message = "Token is required.",
                    MessageDetail = "User must be signed in to access this resource."
                };
            }

            var response = await HttpClientHelper.GetAsync($"{ConfigurationHelper.GetApiVersion()}/masters/roles");
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
            if (string.IsNullOrEmpty(SessionManager.AdminToken))
            {
                return new ApiResponseModel<bool>
                {
                    Success = false,
                    Message = "Token is required.",
                    MessageDetail = "User must be signed in to access this resource."
                };
            }

            var response = await HttpClientHelper.PostAsync($"{ConfigurationHelper.GetApiVersion()}/users", user);
            var (message, messageDetail) = await ApiResponseHandler.HandleApiResponse(response);
            if (message != "Success")
            {
                return new ApiResponseModel<bool>
                {
                    Success = false,
                    Message = message,
                    MessageDetail = messageDetail
                };
            }
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
