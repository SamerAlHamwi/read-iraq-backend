using Abp.AutoMapper;
using System.ComponentModel.DataAnnotations;

namespace ReadIraq.Domain.CompanyBranches.Dto
{
    [AutoMap(typeof(CompanyBranchTranslation))]
    public class CompanyBranchTranslationDto
    {
        [Required]
        public string Name { get; set; }
        [Required]
        public string Bio { get; set; }
        public string Address { get; set; }
        public string Language { get; set; }

    }
}
