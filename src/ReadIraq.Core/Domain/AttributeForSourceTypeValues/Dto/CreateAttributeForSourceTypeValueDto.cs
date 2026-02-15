namespace ReadIraq.Domain.AttributeForSourceTypeValues.Dto
{
    public class CreateAttributeForSourceTypeValueDto
    {
        public int AttributeForSourcTypeId { get; set; }
        public int AttributeChoiceId { get; set; }

    }
    public class CreateAttributeForSourceTypeValueForDraftDto
    {
        public int? AttributeForSourcTypeId { get; set; }
        public int? AttributeChoiceId { get; set; }

    }
}
