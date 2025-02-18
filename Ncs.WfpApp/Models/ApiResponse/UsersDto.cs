using System;

namespace Ncs.WpfApp.Models.ApiResponse;

public class UsersDto
{
    public int Id { get; set; }
    public required string Username { get; set; }
    public required string PasswordHash { get; set; }
    public required string Firstname { get; set; }
    public string? Middlename { get; set; }
    public string? Lastname { get; set; }
    public required string Email { get; set; }
    public required string PhoneNumber { get; set; }
    public required string Address { get; set; }
    public string? EmployeeNumber { get; set; }
    public int? CompanyId { get; set; }
    public int? PersonalTypeId { get; set; }
    public string? PersonalIdNumber { get; set; }
    public string? GuestCompanyName { get; set; }
    public string? RfidTag { get; set; }
    public bool IsActive { get; set; }
    public bool IsDeleted { get; set; }
    public required string CreatedAt { get; set; }
    public string? UpdatedAt { get; set; }
    public required string Fullname { get; set; }
    public virtual required CompaniesDto Company { get; set; }
    public virtual required PersonalIdTypeDto PersonalIdType { get; set; }
    public virtual required UsersDto CreatedByUser { get; set; }
    public virtual required UsersDto UpdatedByUser { get; set; }
    // Many-to-Many Relationship: User has multiple roles
    public List<RolesDto> Roles { get; set; } = [];
}
