using Abp.Application.Services.Dto;

namespace ReadIraq.Domain.Drafts.Dtos
{
    public class UpdateDraftDto : CreateDraftDto, IEntityDto
    {
        public int Id { get; set; }
    }
}
