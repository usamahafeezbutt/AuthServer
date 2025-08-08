using AuthServer.Domain.Entities;
using AutoMapper;
using AuthServer.Application.Services.Account.Dto;
using Microsoft.AspNetCore.Identity;
using AuthServer.Application.Common.Models.Identity;

namespace AuthServer.Application.Common.Mappings
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<RegistrationRequestDto, IdentityUser>();

            CreateMap<RegistrationRequestDto, ApplicationUser>();

            CreateMap<ExternalLoginAuthResponse, AuthResponse>().ReverseMap();

            CreateMap<ApplicationUser, AccountDetailDto>();
        }
    }
}