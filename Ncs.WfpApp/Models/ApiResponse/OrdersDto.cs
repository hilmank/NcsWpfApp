namespace Ncs.WpfApp.Models.ApiResponse
{
    public class OrdersDto
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public int? ReservationGuestsId { get; set; }
        public int MenuItemsId { get; set; }
        public bool IsSpicy { get; set; }
        public int? ReservationsId { get; set; }
        public required string OrderType { get; set; }
        public required string OrderStatus { get; set; }
        public required string OrderDate { get; set; }
        public int Quantity { get; set; }
        public double Price { get; set; }
        public required string CreatedAt { get; set; }
        public int CreatedBy { get; set; }
        public string? UpdatedAt { get; set; }
        public int? UpdatedBy { get; set; }
        public virtual required UsersDto UserOrder { get; set; }
        public virtual required ReservationGuestsDto ReservationGuests { get; set; }
        public virtual required MenuItemsDto MenuItem { get; set; }
        public virtual required ReservationsDto Reservation { get; set; }
        public virtual required UsersDto CreatedByUser { get; set; }
        public virtual required UsersDto UpdatedByUser { get; set; }
        public string MenuVariant => IsSpicy ? "Spicy" : "Regular";
    }
}
