namespace Ncs.WpfApp.Models
{
    public class ReservationListModel
    {
        public int ReservationsId { get; set; }
        public int ReservationsUserId { get; set; }
        public required string ReservationDate { get; set; }
        public required string ReservationsUserName { get; set; }
        public string ReservationsUserCompany { get; set; }
        public int? ReservationsUserGuestId { get; set; }
        public string ReservationsUserGuestName { get; set; }
        public string ReservationsUserGuestCompany { get; set; }
        public int MenuItemsId { get; set; }
        public string MenuItemsName { get; set; }
        public required string MenuVariant { get; set; }
        public required string ReservationStatus { get; set; }
    }
}
