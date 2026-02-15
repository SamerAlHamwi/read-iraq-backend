using Abp.Application.Services.Dto;
using ReadIraq.Cities.Dto;
using System;
using System.Collections.Generic;

namespace ReadIraq.Domain.Regions.Dto
{
    public class RegionDetailsDto : EntityDto
    {
        public string Name { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreationTime { get; set; }
        public LiteCityDto City { get; set; }
        public List<RegionTranslationDto> Translations { get; set; }
    }
}
