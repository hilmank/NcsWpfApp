using Ncs.WpfApp.Models;

namespace Ncs.WpfApp.Services.Interfaces
{
    public interface IReservationService
    {
        Task<ApiResponseModel<IEnumerable<ReservationListModel>>> GetReservationsTodayAsync(string? searchText, List<string>? reservationStatuss);
    }
}
