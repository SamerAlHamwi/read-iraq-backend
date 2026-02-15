using System.Collections.Generic;

namespace ReadIraq.Domain.Partners.Dto
{
    public class CreatePartnerDto
    {

        public string PartnerPhoneNumber { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string CompanyName { get; set; }
        public string CompanyPhoneNumber { get; set; }
        // public List<CreateCodeDto> Codes { get; set; }

        public List<int> CitiesIds { get; set; }

    }
}
