using Abp.Application.Services.Dto;
using ReadIraq.Countries.Dto;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ReadIraq.Cities.Dto
{
    public class CityDto : EntityDto<int>
    {
        [Required]
        [StringLength(500)]
        public string Name { get; set; }
        public int CountryId { get; set; }
        public bool IsActive { get; set; }
        public CountryDto Country { get; set; }
        public List<CityTranslationDto> Translations { get; set; }
    }
}
