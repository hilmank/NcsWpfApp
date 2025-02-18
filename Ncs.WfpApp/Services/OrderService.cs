using Ncs.WpfApp.Helpers;
using Ncs.WpfApp.Models;
using Ncs.WpfApp.Models.ApiResponse;
using Ncs.WpfApp.Services.Interfaces;
using Newtonsoft.Json;
using System.Net.Http.Headers;
using System.Net.Http;
using AutoMapper;
using System.Text;
using AutoMapper.Configuration.Conventions;

namespace Ncs.WpfApp.Services
{
    public class OrderService : IOrderService
    {
        private readonly HttpClient _httpClient;
        private readonly IMapper _mapper;
        public OrderService(HttpClient httpClient, IMapper mapper)
        {
            _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
            _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            _mapper = mapper;
        }


        public async Task<ApiResponseModel<OrderModel>> GetOrderAsync(string searchText)
        {
            if (string.IsNullOrEmpty(SessionManager.Token))
            {
                return new ApiResponseModel<OrderModel>
                {
                    Success = false,
                    Message = $"Token is required to fetch data",
                    MessageDetail = $"Token is required to fetch data."
                };
            }
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", SessionManager.Token);
            var response = await _httpClient.GetAsync("/api/orders/menu/daily");
            var (message, messageDetail) = await ApiResponseHandler.HandleApiResponse(response);
            if (message != "Success")
            {
                return new ApiResponseModel<OrderModel>
                {
                    Success = false,
                    Message = message,
                    MessageDetail = messageDetail
                };
            }

            response = await _httpClient.GetAsync("/api/orders/date");
            (message, messageDetail) = await ApiResponseHandler.HandleApiResponse(response);
            if (message != "Success")
            {
                return new ApiResponseModel<OrderModel>
                {
                    Success = false,
                    Message = message,
                    MessageDetail = messageDetail
                };
            }
            var responseMenuContent = await response.Content.ReadAsStringAsync();

            var apiResponseMenu = JsonConvert.DeserializeObject<ApiResponseModel<List<MenuSchedulesDto>>>(responseMenuContent);

            if (apiResponseMenu?.Data == null || apiResponseMenu.Data.Count < 2)
            {
                return new ApiResponseModel<OrderModel>
                {
                    Success = false,
                    Message = "Invalid data received",
                    MessageDetail = "Invalid data received"
                };
            }

            OrderModel retval = new OrderModel
            {
                Menu1Name = apiResponseMenu.Data[0].MenuItem.Name,
                Menu1Available = apiResponseMenu.Data[0].AvailableQuantity.ToString() + " PAX",
                Menu2Name = apiResponseMenu.Data[1].MenuItem.Name,
                Menu2Available = apiResponseMenu.Data[1].AvailableQuantity.ToString() + " PAX"
            };

            var responseContent = await response.Content.ReadAsStringAsync();

            var apiResponse = JsonConvert.DeserializeObject<ApiResponseModel<IEnumerable<OrdersDto>>>(responseContent);

            if (apiResponse == null)
            {
                return new ApiResponseModel<OrderModel>
                {
                    Success = false,
                    Message = $"Fetching data failed: {apiResponse?.Message ?? "Unknown error"}",
                    MessageDetail = $"Fetching data failed: {apiResponse?.Message ?? "Unknown error"}"
                };
            }
            else if (!apiResponse.Success)
            {
                return new ApiResponseModel<OrderModel>
                {
                    Success = false,
                    Message = apiResponse.Message,
                    MessageDetail = apiResponse.MessageDetail
                };
            }

            retval.Orders = apiResponse.Data?.ToList() ?? new List<OrdersDto>();
            if (string.IsNullOrEmpty(searchText))
            {
                return new ApiResponseModel<OrderModel>
                {
                    Success = true,
                    Message = apiResponse.Message,
                    MessageDetail = apiResponse.MessageDetail,
                    Data = retval
                };
            }
            else
            {
                retval.Orders = apiResponse?.Data?.Where(u =>
                        (!string.IsNullOrWhiteSpace(searchText) &&
                        (
                            u.UserOrder.Fullname.Contains(searchText, StringComparison.OrdinalIgnoreCase) ||
                            (u.UserOrder.RfidTag ?? "").Contains(searchText, StringComparison.OrdinalIgnoreCase) ||
                            (u.UserOrder.Company?.Name ?? "").Contains(searchText, StringComparison.OrdinalIgnoreCase) ||
                            (u.OrderType ?? "").Contains(searchText, StringComparison.OrdinalIgnoreCase) ||
                            (u.ReservationGuests?.Fullname ?? "").Contains(searchText, StringComparison.OrdinalIgnoreCase) ||
                            (u.ReservationGuests?.CompanyName ?? "").Contains(searchText, StringComparison.OrdinalIgnoreCase)
                        ))
                    ).ToList() ?? new List<OrdersDto>();

                return new ApiResponseModel<OrderModel>
                {
                    Success = true,
                    Data = retval
                };
            }
        }
        public async Task<ApiResponseModel<bool>> SaveStatusActionAsync(string action, int orderId)
        {
            if (string.IsNullOrEmpty(SessionManager.Token))
            {
                return new ApiResponseModel<bool>
                {
                    Success = false,
                    Message = $"Token is required to update data",
                    MessageDetail = $"Token is required to update data."
                };
            }
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", SessionManager.Token);
            _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("text/plain"));


            var emptyContent = new StringContent("", Encoding.UTF8, "application/json");

            var response = await _httpClient.PutAsync($"/api/orders/{action.ToLower()}/{orderId}", emptyContent);
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
            return apiResponse;
        }
    }
}
