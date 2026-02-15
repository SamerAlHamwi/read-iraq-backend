using Abp.AutoMapper;

namespace ReadIraq.Domain.AttributesForSourceType.Dto
{
    [AutoMap(typeof(AttributeForSourceTypeTranslation))]
    public class AttributeForSourceTypeTranslationDto
    {
        public string Name { get; set; }

        public string Language { get; set; }

    }
}
