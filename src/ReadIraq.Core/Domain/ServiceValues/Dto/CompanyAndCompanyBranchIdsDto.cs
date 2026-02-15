using System.Collections.Generic;

namespace ReadIraq.Domain.ServiceValues.Dto
{
    public class CompanyAndCompanyBranchIdsDto
    {
        public List<int> CompanyIds { get; set; } = new List<int>();
        public List<int> CompanyBranchIds { get; set; } = new List<int>();
    }
}
