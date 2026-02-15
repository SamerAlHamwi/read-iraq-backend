using AutoMapper;
using ReadIraq.Domain.Toolss;
using ReadIraq.Domain.Toolss.Dto;

namespace ReadIraq.AttributesForSourceType.Mapper
{
    public class ToolMapProfile : Profile
    {
        public ToolMapProfile()
        {
            CreateMap<CreateToolDto, Tool>();
            CreateMap<UpdateToolDto, Tool>();
            CreateMap<LiteToolDto, Tool>();
            CreateMap<ToolDetailsDto, Tool>();

        }
    }
}
