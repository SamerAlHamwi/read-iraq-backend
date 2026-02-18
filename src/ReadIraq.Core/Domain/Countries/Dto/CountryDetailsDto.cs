using Abp.Application.Services.Dto;
using ReadIraq.Cities.Dto;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using static ReadIraq.Enums.Enum;

namespace ReadIraq.Countries.Dto
{
    public class CountryDetailsDto : EntityDto
    {
        public string Name { get; set; }

        public bool IsActive { get; set; }

        public List<LiteCityDto> Cities { get; set; }

        public DateTime CreationTime { get; set; }
        [Required]
        [StringLength(5)]
        public string DialCode { get; set; }
        public List<CountryTranslationDto> Translations { get; set; }

    }
}
