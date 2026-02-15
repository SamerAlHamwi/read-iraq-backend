using AutoMapper;
using ReadIraq.Domain.Terms;
using ReadIraq.TermService.Dto;

namespace ReadIraq.TermService.Mapper
{
    public class TermMapProfile : Profile
    {
        public TermMapProfile()
        {
            CreateMap<CreateTermDto, Term>();
        }
    }
}
