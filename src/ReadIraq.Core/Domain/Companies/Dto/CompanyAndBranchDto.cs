using ReadIraq.Domain.CompanyBranches;
using System.Collections.Generic;

namespace ReadIraq.Domain.Companies.Dto
{
    public class CompanyAndBranchDto
    {
        public List<Company> Companies { get; set; }
        public List<CompanyBranch> CompanyBranches { get; set; }
    }
}
