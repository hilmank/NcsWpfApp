using AutoMapper;
using Ncs.WpfApp.Models;
using Ncs.WpfApp.Models.ApiResponse;

namespace Ncs.WpfApp.Helpers
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            // Example mapping
            CreateMap<UserSignInModel, UserSignInModel>();

            // ✅ Explicitly map `UsersDto` to `UserListModel`
            CreateMap<UsersDto, UserListModel>()
                .ForMember(dest => dest.UsersId, opt => opt.MapFrom(src => src.Id.ToString()))
                .ForMember(dest => dest.UsersName, opt => opt.MapFrom(src => src.Fullname))
                .ForMember(dest => dest.UsersRfidTag, opt => opt.MapFrom(src => src.RfidTag ?? "N/A"))
                .ForMember(dest => dest.UsersEmail, opt => opt.MapFrom(src => src.Email))
                .ForMember(dest => dest.UsersPhoneNumber, opt => opt.MapFrom(src => src.PhoneNumber))
                .ForMember(dest => dest.UsersIdType, opt => opt.MapFrom(src => src.PersonalIdType != null ? src.PersonalIdType.Name : "Employee Number"))
                .ForMember(dest => dest.UsersIdNumber, opt => opt.MapFrom(src => src.PersonalIdNumber != null ? src.PersonalIdNumber : src.EmployeeNumber))
                .ForMember(dest => dest.UsersRole, opt => opt.MapFrom(src => string.Join(", ", src.Roles.Select(r => r.Name))))
                .ForMember(dest => dest.UsersCompany, opt => opt.MapFrom(src => src.Company != null ? src.Company.Name : src.GuestCompanyName))
                .ForMember(dest => dest.UsersStatus, opt => opt.MapFrom(src => src.IsActive ? "Active" : "Inactive"));

            CreateMap<OrdersDto, OrderListModel>()
                .ForMember(dest => dest.OrdersId, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.OrdersUserId, opt => opt.MapFrom(src => src.UserId))
                .ForMember(dest => dest.OrdersUserName, opt => opt.MapFrom(src => src.UserOrder.Fullname))
                .ForMember(dest => dest.OrdersUserCompany, opt => opt.MapFrom(src => src.UserOrder.Company != null ? src.UserOrder.Company.Name : src.UserOrder.GuestCompanyName))
                .ForMember(dest => dest.ReservationGuestsId, opt => opt.MapFrom(src => src.ReservationGuestsId))
                .ForMember(dest => dest.OrdersUserGuestName, opt => opt.MapFrom(src => src.ReservationGuests != null ? src.ReservationGuests.Fullname : "N/A"))
                .ForMember(dest => dest.OrdersUserGuestCompany, opt => opt.MapFrom(src => src.ReservationGuests != null ? src.ReservationGuests.CompanyName : "N/A"))
                .ForMember(dest => dest.MenuItemsName, opt => opt.MapFrom(src => src.MenuItem.Name))
                .ForMember(dest => dest.MenuVariant, opt => opt.MapFrom(src => src.MenuVariant)) // Auto-property dari IsSpicy
                .ForMember(dest => dest.OrderType, opt => opt.MapFrom(src => src.OrderType))
                .ForMember(dest => dest.OrderStatus, opt => opt.MapFrom(src => src.OrderStatus))
                .ForMember(dest => dest.OrderDate, opt => opt.MapFrom(src => src.CreatedAt))
                .ForMember(dest => dest.ReservationDate, opt => opt.MapFrom(src => src.Reservation != null ? src.Reservation.ReservedDate : "N/A"));

        }
    }
}
