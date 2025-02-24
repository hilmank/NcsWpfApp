using Ncs.WpfApp.Helpers;
using Ncs.WpfApp.Models;
using Ncs.WpfApp.Services.Interfaces;
using Newtonsoft.Json;

namespace Ncs.WpfApp.Services
{
    public class ReservationService : IReservationService
    {

        /// <summary>
        /// Converts a list of ReservationsDto into a list of ReservationListModel.
        /// Each guest in a reservation will be listed as a separate row.
        /// </summary>
        private static List<ReservationListModel> TransformReservationsToList(List<ReservationsDto> reservations)
        {
            var reservationList = new List<ReservationListModel>();

            foreach (var reservation in reservations)
            {
                reservationList.Add(CreateReservationRow(reservation,null,  "", ""));
                if (reservation.Guests != null)
                {
                    // Each guest → Separate row
                    foreach (var guest in reservation.Guests)
                    {
                        reservationList.Add(CreateReservationRow(reservation, guest.Id, guest.Fullname, guest.CompanyName));
                    }
                }
            }
            return reservationList;
        }

        /// <summary>
        /// Creates a single row of ReservationListModel from a ReservationsDto.
        /// </summary>
        private static ReservationListModel CreateReservationRow(ReservationsDto reservation, int? guestId, string guestName, string guestCompany)
        {
            return new ReservationListModel
            {
                ReservationsId = reservation.Id,
                ReservationsUserId = reservation.ReservedBy,
                ReservationDate = reservation.ReservedDate,
                ReservationsUserName = reservation.ReservedByUser.Fullname,
                ReservationsUserCompany = reservation.ReservedByUser.Company?.Name ?? reservation.ReservedByUser.GuestCompanyName,
                ReservationsUserGuestId = guestId,
                ReservationsUserGuestName = guestName,
                ReservationsUserGuestCompany = guestCompany,
                MenuItemsId = reservation.MenuItemsId,
                MenuItemsName = reservation.MenuItem.Name,
                MenuVariant = reservation.MenuVariant,
                ReservationStatus = reservation.Status.Name
            };
        }

        /// <summary>
        /// Applies search and status filters to the reservation list.
        /// </summary>
        private static List<ReservationListModel> ApplyFilters(List<ReservationListModel> reservations, string? searchText, List<string>? reservationStatuss)
        {
            if (reservationStatuss != null)
            {
                reservations = reservations.Where(x => reservationStatuss.Contains(x.ReservationStatus)).ToList();
            }

            if (!string.IsNullOrEmpty(searchText))
            {
                reservations = reservations.Where(u =>
                    u.ReservationsUserName.Contains(searchText, StringComparison.OrdinalIgnoreCase) ||
                    (u.ReservationsUserCompany ?? "").Contains(searchText, StringComparison.OrdinalIgnoreCase) ||
                    (u.ReservationsUserGuestName ?? "").Contains(searchText, StringComparison.OrdinalIgnoreCase) ||
                    (u.ReservationsUserGuestCompany ?? "").Contains(searchText, StringComparison.OrdinalIgnoreCase) ||
                    (u.MenuItemsName ?? "").Contains(searchText, StringComparison.OrdinalIgnoreCase) ||
                    (u.MenuVariant ?? "").Contains(searchText, StringComparison.OrdinalIgnoreCase)
                ).ToList();
            }

            return reservations;
        }

        /// <summary>
        /// Returns an unauthorized API response.
        /// </summary>
        private static ApiResponseModel<IEnumerable<ReservationListModel>> UnauthorizedResponse()
        {
            return new ApiResponseModel<IEnumerable<ReservationListModel>>
            {
                Success = false,
                Message = "Token is required.",
                MessageDetail = "User must be signed in to access this resource."
            };
        }

        /// <summary>
        /// Returns an error response with the given message and details.
        /// </summary>
        private static ApiResponseModel<IEnumerable<ReservationListModel>> ErrorResponse(string message, string messageDetail)
        {
            return new ApiResponseModel<IEnumerable<ReservationListModel>>
            {
                Success = false,
                Message = message,
                MessageDetail = messageDetail
            };
        }

        public async Task<ApiResponseModel<IEnumerable<ReservationListModel>>> GetReservationsTodayAsync(string? searchText, List<string>? reservationStatuss)
        {
            // 🔹 Check if the user is signed in
            if (string.IsNullOrEmpty(SessionManager.AdminToken) && string.IsNullOrEmpty(SessionManager.CustomerToken))
                return UnauthorizedResponse();

            // 🔹 Call API to fetch reservations
            var response = await HttpClientHelper.GetAsync($"{ConfigurationHelper.GetApiVersion()}/reservations/date?startDate={DateTime.Today:yyyy-MM-dd}&endDate={DateTime.Today:yyyy-MM-dd}");

            var (message, messageDetail) = await ApiResponseHandler.HandleApiResponse(response);
            if (message != "Success")
                return ErrorResponse(message, messageDetail);

            // 🔹 Deserialize API response
            var responseContent = await response.Content.ReadAsStringAsync();
            var apiResponse = JsonConvert.DeserializeObject<ApiResponseModel<List<ReservationsDto>>>(responseContent);

            if (apiResponse?.Data == null)
                return ErrorResponse("No data received", "The API response did not contain any data.");

            // 🔹 Convert ReservationsDto to ReservationListModel
            var reservationList = TransformReservationsToList(apiResponse.Data);

            // 🔹 Apply Filters (Search & Status)
            reservationList = ApplyFilters(reservationList, searchText, reservationStatuss);

            return new ApiResponseModel<IEnumerable<ReservationListModel>>
            {
                Success = true,
                Message = apiResponse.Message,
                MessageDetail = apiResponse.MessageDetail,
                Data = reservationList
            };
        }
        public async Task<ApiResponseModel<IEnumerable<ReservationListModel>>> GetReservationsByUserIdAsync(int userId)
        {
            if (string.IsNullOrEmpty(SessionManager.AdminToken) && string.IsNullOrEmpty(SessionManager.CustomerToken))
                return UnauthorizedResponse();

            var reservationsToday = await GetReservationsTodayAsync(null, null);
            if (reservationsToday is null)
                return new ApiResponseModel<IEnumerable<ReservationListModel>>
                {
                    Success = false
                };
            var data = reservationsToday.Data.Where(x => x.ReservationsUserId == userId);
            if (!data.Any())
                return new ApiResponseModel<IEnumerable<ReservationListModel>>
                {
                    Success = false
                };
            return new ApiResponseModel<IEnumerable<ReservationListModel>>
            {
                Success = true,
                Data = data
            };
        }
    }
}
