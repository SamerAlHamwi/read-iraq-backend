using Abp.Application.Services.Dto;
using ReadIraq.Cities.Dto;
using System.Collections.Generic;

namespace ReadIraq.Domain.Partners.Dto
{
    public class LitePartnerDto : EntityDto<int>
    {
        public string PartnerPhoneNumber { get; set; }
        public bool IsActive { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string CompanyName { get; set; }
        public string CompanyPhoneNumber { get; set; }
        public List<LiteCityDto> CitiesPartner { get; set; }
    }
}
