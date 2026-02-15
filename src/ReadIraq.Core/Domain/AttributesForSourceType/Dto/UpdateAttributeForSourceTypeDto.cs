using Abp.Application.Services.Dto;

namespace ReadIraq.Domain.AttributesForSourceType.Dto
{
    public class UpdateAttributeForSourceTypeDto : CreateAttributeForSourceTypeDto, IEntityDto<int>
    {
        public int Id { get; set; }
    }
}
