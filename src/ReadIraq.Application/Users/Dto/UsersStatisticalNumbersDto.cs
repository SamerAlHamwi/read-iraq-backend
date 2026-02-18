using System.Collections.Generic;

namespace ReadIraq.Users.Dto
{
    public class UsersStatisticalNumbersDto
    {
        public int TotalCount { get; set; }
        public int SuperAdmins { get; set; }
        public int Students { get; set; }
        public int Teachers { get; set; }
        public int ActiveUsers { get; set; }
        public int DeActiveUsers { get; set; }
        public List<InfoForUserChart> ChartPoints { get; set; }
    }

    public class InfoForUserChart
    {
        public int Month { get; set; }
        public int UserCount { get; set; }
    }
}
