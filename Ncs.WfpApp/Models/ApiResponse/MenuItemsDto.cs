namespace Ncs.WpfApp.Models.ApiResponse;

public class MenuItemsDto
{
    public int Id { get; set; }
    public int VendorId { get; set; }
    public required string Name { get; set; }
    public required string Description { get; set; }
    public double Calories { get; set; }
    public double Price { get; set; }
    public required string ImageUrl { get; set; }
    public bool IsActive { get; set; }
    public bool IsDeleted { get; set; }
    public required string CreatedAt { get; set; }
    public int CreatedBy { get; set; }
    public string? UpdatedAt { get; set; }
    public int? UpdatedBy { get; set; }
    public virtual required VendorsDto Vendor { get; set; }
    public virtual required UsersDto CreatedByUser { get; set; }
    public virtual required UsersDto UpdatedByUser { get; set; }
}
