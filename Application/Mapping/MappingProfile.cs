using AutoMapper;
using UsersAuthorization.Application.DTO;
using UsersAuthorization.Domain.Entities;

namespace UsersAuthorization.Application.Mapping
{
    public class MappingProfile: Profile
    {
        public MappingProfile() {
            CreateMap<ApplicationUser, UserDTO>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name))
                .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.Email))
                .ReverseMap();
        }

    }
}
