namespace Ncs.WpfApp.Models.ApiResponse;

public class ReservationGuestsDto
{
    public int Id { get; set; }
    public int ReservationsId { get; set; } // Linked to the reservation
    public required string Fullname { get; set; }
    public required string CompanyName { get; set; }
    public int PersonalIdTypeId { get; set; }
    public required string PersonalIdNumber { get; set; }
    public int MenuItemsId { get; set; }
    public bool IsSpicy { get; set; }

    // Navigation properties
    public virtual required ReservationsDto Reservation { get; set; }
    public virtual required PersonalIdTypeDto PersonalType { get; set; }
    public virtual required MenuItemsDto MenuItem { get; set; }
}
