namespace Ncs.WpfApp.Models
{
        public class OrderListModel
        {
            public int OrdersId { get; set; }
            public int OrdersUserId { get; set; }
            public required string OrdersUserName { get; set; }
            public required string OrdersUserCompany { get; set; }
            public int? ReservationGuestsId { get; set; }
            public required string OrdersUserGuestName { get; set; }
            public required string OrdersUserGuestCompany { get; set; }
            public required string MenuItemsName { get; set; }
            public required string MenuVariant { get; set; }
            public required string OrderType { get; set; }
            public required string OrderStatus { get; set; }
            public required string OrderDate { get; set; }
            public required string ReservationDate { get; set; }

        public OrderStatusParameter OrderParameter => new OrderStatusParameter
        {
            OrderStatus = OrderStatus,
            OrderId = OrdersId
        };
    }
}
