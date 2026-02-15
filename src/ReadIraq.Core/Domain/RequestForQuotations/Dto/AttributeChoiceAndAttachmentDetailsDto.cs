using ReadIraq.Domain.AttributeChoices.Dto;
using System.Collections.Generic;

namespace ReadIraq.Domain.RequestForQuotations.Dto
{
    public class AttributeChoiceAndAttachmentDetailsDto
    {
        public AttributeChoiceDetailsDto AttributeChoice { get; set; }
        public List<LiteAttachmentDto> Attachments { get; set; } = new List<LiteAttachmentDto>();
    }
}
