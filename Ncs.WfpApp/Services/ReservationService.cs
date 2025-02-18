using Ncs.WpfApp.Models;
using Ncs.WpfApp.Services.Interfaces;

namespace Ncs.WpfApp.Services
{
    public class ReservationService : IReservationService
    {
        public Task<ApiResponseModel<IEnumerable<ReservationListModel>>> GetReservationsAsync()
        {
            throw new NotImplementedException();
        }

        public Task<ApiResponseModel<IEnumerable<ReservationListModel>>> SearchReservationsAsync(string searchText)
        {
            throw new NotImplementedException();
        }
    }
}
