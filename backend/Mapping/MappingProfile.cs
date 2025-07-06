using AutoMapper;
using backend.Models.External;
using backend.Models.Internal;

namespace backend.Mapping
{

    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<ExternalRequest, Request>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.ServiceId, opt => opt.MapFrom(src => src.ServiceId))
                .ForMember(dest => dest.Title, opt => opt.MapFrom(src => src.Title))
                .ForMember(dest => dest.CreationDate, opt => opt.MapFrom(src => src.CreationDate))
                .ForMember(dest => dest.ClientName, opt => opt.MapFrom(src => src.ClientName))
                .ForMember(dest => dest.ShortDescr, opt => opt.MapFrom(src => src.ShortDescr))
                .ForMember(dest => dest.DescriptionRtf4096, opt => opt.MapFrom(src => src.DescriptionRtf4096))
                .ForMember(dest => dest.isCritical, opt => opt.Ignore());
        }
    }
}