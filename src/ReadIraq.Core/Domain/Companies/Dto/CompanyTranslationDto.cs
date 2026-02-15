using Abp.AutoMapper;
using System.ComponentModel.DataAnnotations;

namespace ReadIraq.Domain.Companies.Dto
{
    [AutoMap(typeof(CompanyTranslation))]
    public class CompanyTranslationDto
    {
        [Required]
        public string Name { get; set; }
        [Required]
        public string Bio { get; set; }
        public string Address { get; set; }
        public string Language { get; set; }

    }
}
