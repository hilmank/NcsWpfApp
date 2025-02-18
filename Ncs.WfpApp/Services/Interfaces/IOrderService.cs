using Ncs.WpfApp.Models;
using Ncs.WpfApp.Models.ApiResponse;

namespace Ncs.WpfApp.Services.Interfaces
{
    public interface IOrderService
    {
        Task<ApiResponseModel<OrderModel>> GetOrderAsync(string searchText);
        Task<ApiResponseModel<bool>> SaveStatusActionAsync(string action, int orderId);
    }
}
