using System.Collections.Generic;

namespace ReadIraq.Domain.ServiceValues.Dto
{
    public class ServiceIdsAndRequestIdsDto
    {
        public long RequestId { get; set; }
        public List<int> ServiceIds { get; set; }
    }
    public class ServiceIdsAndCompanyIdsDto
    {
        public int CompanyId { get; set; }
        public List<int> ServiceIds { get; set; }
    }
    public class ServiceIdsAndCompanyBranchIdsDto
    {
        public int CompanyBranchId { get; set; }
        public List<int> ServiceIds { get; set; }
    }
}
