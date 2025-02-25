using Ncs.WpfApp.Models;
using Ncs.WpfApp.Models.ApiResponse;

namespace Ncs.WpfApp.Services.Interfaces
{
    public interface IOrderService
    {
        Task<ApiResponseModel<bool>> SaveStatusActionAsync(string action, int orderId);
        Task<ApiResponseModel<bool>> SaveStatussActionAsync(string action, List<int> orderIds);

        Task<ApiResponseModel<IEnumerable<MenuSchedulesDto>>> GetTodayMenus();
        Task<ApiResponseModel<bool>> SaveOrderCustomerActionAsync(OrderAddModel orderItem);
        Task<ApiResponseModel<OrderModel>> GetOrdersTodayAsync(string? searchText, List<string>? orderStatuss);
        Task<ApiResponseModel<List<OrdersDto>?>> GetReservationOrdersAsync(int orderId);
    }
}
