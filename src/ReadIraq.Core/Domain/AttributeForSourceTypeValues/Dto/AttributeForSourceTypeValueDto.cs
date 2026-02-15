using ReadIraq.Domain.AttributeChoices.Dto;
using ReadIraq.Domain.AttributesForSourceType.Dto;

namespace ReadIraq.Domain.AttributeForSourceTypeValues.Dto
{
    public class AttributeForSourceTypeValueDto
    {
        public AttributeForSourceTypeDetailsDto AttributeForSourcType { get; set; }
        public AttributeChoiceDetailsDto AttributeChoice { get; set; }
    }
}
