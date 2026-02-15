using Abp.Application.Services.Dto;
using System.Collections.Generic;
using static ReadIraq.Enums.Enum;

namespace ReadIraq.Domain.RejectReasons.Dto
{
    public class RejectReasonDetailsDto : EntityDto
    {
        public string Description { get; set; }
        public bool IsActive { get; set; }
        public PossibilityPotentialClient PossibilityPotentialClient { get; set; }
        public List<RejectReasonTranslationDto> Translations { get; set; } = new List<RejectReasonTranslationDto>();


    }
}
