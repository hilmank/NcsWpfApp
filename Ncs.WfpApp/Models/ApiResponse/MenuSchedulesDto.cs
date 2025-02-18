
namespace Ncs.WpfApp.Models.ApiResponse
{
    public class MenuSchedulesDto
    {
        public int Id { get; set; }
        public int MenuItemsId { get; set; }

        public required string Scheduledate { get; set; }
        public int AvailableQuantity { get; set; }

        public required string CreatedAt { get; set; }
        public int CreatedBy { get; set; }
        public string? UpdatedAt { get; set; }

        public int? UpdatedBy { get; set; }
        public virtual required MenuItemsDto MenuItem { get; set; }

        public virtual required UsersDto CreatedByUser { get; set; }

        public virtual required UsersDto UpdatedByUser { get; set; }
    }
}
