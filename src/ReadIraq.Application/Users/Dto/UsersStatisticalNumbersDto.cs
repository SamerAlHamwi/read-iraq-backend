using System.Collections.Generic;

namespace ReadIraq.Users.Dto
{
    public class UsersStatisticalNumbersDto
    {
        public int TotalCount { get; set; }
        public int Admins { get; set; }
        public int Users { get; set; }
        public int ActiveUsers { get; set; }
        public int DeActiveUsers { get; set; }
        public int CompanyUser { get; set; }
        public int CustomerService { get; set; }
        public int CompanyBranchUser { get; set; }
        public int MediatorUser { get; set; }
        public List<InfoForUserChart> ChartPoints { get; set; }
    }

    public class InfoForUserChart
    {
        public int Month { get; set; }
        public int UserCount { get; set; }
    }
}
