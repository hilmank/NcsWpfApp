using Ncs.WpfApp.Models;

namespace Ncs.WpfApp.Services.Interfaces
{
    public interface IReservationService
    {
        Task<ApiResponseModel<IEnumerable<ReservationListModel>>> GetReservationsAsync();
        Task<ApiResponseModel<IEnumerable<ReservationListModel>>> SearchReservationsAsync(string searchText);
    }
}
