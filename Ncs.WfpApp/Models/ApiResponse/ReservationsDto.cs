using Ncs.WpfApp.Models.ApiResponse;

public class ReservationsDto
{
    public int Id { get; set; }
    public int ReservedBy { get; set; } // User ID
    public required string ReservedDate { get; set; }
    public int MenuItemsId { get; set; }
    public bool IsSpicy { get; set; }
    public int StatusId { get; set; }
    public required string CreatedAt { get; set; }
    public int CreatedBy { get; set; }
    public string? UpdatedAt { get; set; }
    public int? UpdatedBy { get; set; }

    // Navigation properties
    public virtual required UsersDto ReservedByUser { get; set; }
    public virtual required MenuItemsDto MenuItem { get; set; }
    public virtual required ReservationsStatusDto Status { get; set; }
    public virtual required UsersDto CreatedByUser { get; set; }
    public virtual required UsersDto UpdatedByUser { get; set; }

    // List of guests in the reservation
    public List<ReservationGuestsDto> Guests { get; set; } = new List<ReservationGuestsDto>();

    public string MenuVariant => IsSpicy ? "Spicy" : "Regular";
}
