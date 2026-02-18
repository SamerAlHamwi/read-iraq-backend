using Abp.Application.Services.Dto;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using static ReadIraq.Enums.Enum;

namespace ReadIraq.Countries.Dto
{
    public class CountryDto : EntityDto<int>
    {

        [StringLength(500)]
        public string Name { get; set; }
        public bool IsActive { get; set; }
        [StringLength(5)]
        public string DialCode { get; set; }
        public List<CountryTranslationDto> Translations { get; set; }

    }
}
