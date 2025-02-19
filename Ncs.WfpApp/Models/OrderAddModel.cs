namespace Ncs.WpfApp.Models
{
    public class OrderAddModel
    {
        public int MenuItemsId { get; set; }
        public bool IsSpicy { get; set; }
        public List<int>? ReservationGuestsIds {  get; set; }
    }
}
