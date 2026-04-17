using AutoMapper;
using LEX.Domain.Entities;
using LEX.Shared.DTOs.AuthenticationRegister;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LEX.Service.MappingProfiles
{
    public class AuthService : Profile
    {
        public AuthService()
        {
            CreateMap<RegisterDto, Customer>();

            CreateMap<RegisterDto, Lawyer>()
                .ForMember(dest => dest.Start_Practice, opt => opt.MapFrom(src => src.StartPractice))
                .ForMember(dest => dest.Specialization, opt => opt.MapFrom(src => src.Specialization));
        }
    }
}
