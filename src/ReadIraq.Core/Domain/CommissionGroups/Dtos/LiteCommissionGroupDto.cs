using Abp.Application.Services.Dto;
using ReadIraq.Domain.Companies.Dto;
using System.Collections.Generic;

namespace ReadIraq.Domain.CommissionGroups.Dtos
{
    public class LiteCommissionGroupDto : EntityDto
    {
        public string Name { get; set; }
        public List<CompanyDto> Companies { get; set; }
        public bool IsDefault { get; set; }

    }
}
