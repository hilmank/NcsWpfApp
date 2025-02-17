using AutoMapper;
using Ncs.WfpApp.Models;

namespace Ncs.WfpApp.Helpers
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            // Example mapping
            CreateMap<UserSignInModel, UserSignInModel>();
        }
    }        
}
