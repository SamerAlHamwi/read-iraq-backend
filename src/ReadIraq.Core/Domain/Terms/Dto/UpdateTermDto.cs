using Abp.Application.Services.Dto;

namespace ReadIraq.TermService.Dto
{
    public class UpdateTermDto : CreateTermDto, IEntityDto
    {
        public int Id { get; set; }
    }
}