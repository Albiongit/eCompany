using AutoMapper;
using eCompany.Models;
using eCompany.Models.DTOs.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eCompany.Shared
{
    public  class CompanyDetails : Profile
    {
        public CompanyDetails()
        {
            CreateMap<Company, CompanyDTO>()
                .ForMember(dest => dest.CompanyId, opt => opt.MapFrom(src => src.CompanyId))
                .ForMember(dest => dest.CompanyName, opt => opt.MapFrom(src => src.CompanyName))
                .ForMember(dest => dest.CompanyWeb, opt => opt.MapFrom(src => src.CompanyWeb))
                .ForMember(dest => dest.CompanyPhone, opt => opt.MapFrom(src => src.CompanyPhone))
                .ForMember(dest => dest.CompanyState, opt => opt.MapFrom(src => src.CompanyState));
                


        }
    }
}
