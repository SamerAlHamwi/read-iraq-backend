using System.Collections.Generic;

namespace ReadIraq.Domain.CommissionGroups.Dtos
{
    public class CommissionGroupDto
    {
        public double Commission { get; set; }
    }
    public class CommissionGroupWithCompanyIdsDto
    {
        public string Name { get; set; }

        public List<int> Companies { get; set; }
    }
}
