using AutoMapper;
using Shortly.API.Dtos;
using Shortly.API.Models;

namespace Shortly.API.Mappings
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<UrlMappingCreateDto, UrlMapping>();

            CreateMap<UrlMapping, UrlMappingResponseDto>();
        }
    }
}
