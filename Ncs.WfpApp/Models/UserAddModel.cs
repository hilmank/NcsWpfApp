using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ncs.WpfApp.Models
{
    public class UserAddModel
    {
        public required string Username { get; set; }
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
        public required string RfidTag { get; set; }
        public required List<int> RolesIds { get; set; }
    }
}
