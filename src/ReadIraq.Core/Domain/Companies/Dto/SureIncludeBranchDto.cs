using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace ReadIraq.Domain.Companies.Dto
{
    public class SureIncludeBranchDto : RequestIncludeBranchDto
    {
        [Required]
        public int CompanyId { get; set; }
        [Required]
        public string Code { get; set; }
        [AllowNull]
        private new string PinCode { get; set; }
    }
}
