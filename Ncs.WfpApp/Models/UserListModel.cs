using System.Reflection.PortableExecutable;
using System.Windows.Controls;
using System.Windows.Data;

namespace Ncs.WpfApp.Models
{
    public class UserListModel
    {
        public required string UsersId { get; set; }
        public required string UsersName { get; set; }
        public required string UsersRfidTag { get; set; }
        public required string UsersEmail { get; set; }
        public required string UsersPhoneNumber { get; set; }
        public required string UsersIdType { get; set; }
        public required string UsersIdNumber { get; set; }
        public required string UsersRole { get; set; }
        public required string UsersCompany { get; set; }
        public required string UsersStatus { get; set; }
    }
}
