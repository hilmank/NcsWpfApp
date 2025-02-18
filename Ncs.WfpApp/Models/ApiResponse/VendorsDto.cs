namespace Ncs.WpfApp.Models.ApiResponse
{
    public class VendorsDto
    {
        public int Id { get; set; }
        public required string Name { get; set; }
        public required string ContactInfo { get; set; }
        public required string Address { get; set; }
        public required string PhoneNumber { get; set; }
        public required string Email { get; set; }
        public bool IsActive { get; set; }
        public bool IsDeleted { get; set; }
        public required string CreatedAt { get; set; }
        public string? UpdatedAt { get; set; }
        public virtual required UsersDto CreatedByUser { get; set; }

        public virtual required UsersDto UpdatedByUser { get; set; }
    }
}
