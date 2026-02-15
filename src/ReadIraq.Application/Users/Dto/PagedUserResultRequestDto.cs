using Abp.Application.Services.Dto;
using static ReadIraq.Enums.Enum;

namespace ReadIraq.Users.Dto
{
    //custom PagedResultRequestDto
    public class PagedUserResultRequestDto : PagedResultRequestDto
    {
        public string Keyword { get; set; }
        public bool? IsActive { get; set; }
        /// <summary>
        /// Admin = 1,
        /// BasicUser = 2,
        /// CompanyUser = 3,
        /// CustomerService = 4
        /// </summary>
        public UserType? UserType { get; set; }
        public bool? GetAdminsAndCustomerServices { get; set; }
        public bool GetBasicUserAndCompanyUsers { get; set; } = false;
        public bool GetBasicUserAndBrokerUsers { get; set; } = false;
        public string MediatorCode { get; set; }
    }
}
