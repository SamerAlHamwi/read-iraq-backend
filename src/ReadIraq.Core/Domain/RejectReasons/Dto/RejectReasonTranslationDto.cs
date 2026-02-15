using Abp.AutoMapper;

namespace ReadIraq.Domain.RejectReasons.Dto
{
    [AutoMap(typeof(RejectReasonTranslation))]
    public class RejectReasonTranslationDto
    {
        public string Description { get; set; }

        public string Language { get; set; }

    }
}
