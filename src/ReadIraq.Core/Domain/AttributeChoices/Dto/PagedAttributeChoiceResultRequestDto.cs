using Abp.Application.Services.Dto;

namespace ReadIraq.Domain.AttributeChoices.Dto
{
    public class PagedAttributeChoiceResultRequestDto : PagedResultRequestDto
    {
        public string Keyword { get; set; }
        public int? AttributeId { get; set; }
        public bool? IsParent { get; set; }
        public int? ParentId { get; set; }
    }
}
