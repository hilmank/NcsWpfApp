using Ncs.WpfApp.Models.ApiResponse;

namespace Ncs.WpfApp.Models
{
    public class OrderModel
    {
        public string? Menu1Name { get; set; }
        public string? Menu1Available { get; set; }
        public string? Menu2Name { get; set; }
        public string? Menu2Available { get; set; }
        public List<OrdersDto>? Orders { get; set; }

    }
}
