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
        private readonly IMapper _mapper;
        public OrderService(IMapper mapper)
        {
            _mapper = mapper;
        }


        public async Task<ApiResponseModel<bool>> SaveStatusActionAsync(string action, int orderId)
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
            string apiUrl = $"{ConfigurationHelper.GetApiVersion()}/orders/{action.ToLower()}/{orderId}";

            var response = await HttpClientHelper.PutAsync(apiUrl, new { });
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
            return apiResponse ?? new ApiResponseModel<bool>
            {
                Success = false,
                Message = "Unknown error",
                MessageDetail = "Unknown error"
            };
        }
        public async Task<ApiResponseModel<IEnumerable<MenuSchedulesDto>>> GetTodayMenus()
        {

            var response = await HttpClientHelper.GetAsync($"{ConfigurationHelper.GetApiVersion()}/orders/menu/daily");
            var (message, messageDetail) = await ApiResponseHandler.HandleApiResponse(response);
            if (message != "Success")
            {
                return new ApiResponseModel<IEnumerable<MenuSchedulesDto>>
                {
                    Success = false,
                    Message = message,
                    MessageDetail = messageDetail
                };
            }
            var responseMenuContent = await response.Content.ReadAsStringAsync();

            var apiResponse = JsonConvert.DeserializeObject<ApiResponseModel<IEnumerable<MenuSchedulesDto>>>(responseMenuContent);

            if (apiResponse?.Data == null || apiResponse.Data.ToList().Count < 2)
            {
                return new ApiResponseModel<IEnumerable<MenuSchedulesDto>>
                {
                    Success = false,
                    Message = "Invalid data received",
                    MessageDetail = "Invalid data received"
                };
            }
            return apiResponse ?? new ApiResponseModel<IEnumerable<MenuSchedulesDto>>
            {
                Success = false,
                Message = "Unknown error",
                MessageDetail = "Unknown error"
            };
        }
        public async Task<ApiResponseModel<bool>> SaveOrderCustomerActionAsync(OrderAddModel orderItem)
        {
            if (string.IsNullOrEmpty(SessionManager.CustomerToken))
            {
                return new ApiResponseModel<bool>
                {
                    Success = false,
                    Message = "Token is required.",
                    MessageDetail = "User must be signed in to access this resource."
                };
            }

            var response = await HttpClientHelper.PostAsync($"{ConfigurationHelper.GetApiVersion()}/orders", orderItem);
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

            
            return apiResponse ?? new ApiResponseModel<bool>
            {
                Success = false,
                Message = "Unknown error",
                MessageDetail = "Unknown error"
            };
        }
        public async Task<ApiResponseModel<OrderModel>> GetOrderTodayAsync(string? searchText, List<string>? orderStatuss)
        {
            if (string.IsNullOrEmpty(SessionManager.AdminToken) && string.IsNullOrEmpty(SessionManager.CustomerToken))
            {
                return new ApiResponseModel<OrderModel>
                {
                    Success = false,
                    Message = "Token is required.",
                    MessageDetail = "User must be signed in to access this resource."
                };
            }
            var response = await HttpClientHelper.GetAsync($"{ConfigurationHelper.GetApiVersion()}/orders/menu/daily");

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

            response = await HttpClientHelper.GetAsync($"{ConfigurationHelper.GetApiVersion()}/orders/today");
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

            retval.Orders = apiResponse.Data?.OrderByDescending(x => x.CreatedAt).ToList() ?? [];
            if (orderStatuss is not null)
                retval.Orders = [.. retval.Orders.Where(x => orderStatuss.Contains(x.OrderStatus))];
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
    }
}
