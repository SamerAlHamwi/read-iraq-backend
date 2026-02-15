using ReadIraq.Domain.Regions.Dto;
using ReadIraq.Domain.UserVerficationCodes;

namespace ReadIraq.Domain.Companies.Dto
{
    public class CompanyBranchAndUserDto
    {
        public LiteUserDto? User { get; set; }
        public string Address { get; set; }
        public LiteRegionDto Region { get; set; }
        public CompanyContactDetailsDto CompanyContact { get; set; }
    }
}
