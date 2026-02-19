using ReadIraq.Cities.Dto;
using ReadIraq.Grades.Dto;
using static ReadIraq.Enums.Enum;

namespace ReadIraq.Authorization.Accounts.Dto
{
    public class UserDetailDto
    {
        public long Id { get; set; }
        public string RegistrationFullName { get; set; }
        public string Name { get; set; }
        public string Surname { get; set; }
        public string EmailAddress { get; set; }
        public string DialCode { get; set; }
        public string PhoneNumber { get; set; }
        public UserType Type { get; set; }
        public bool IsEmailConfirmed { get; set; }
        public bool IsPhoneNumberConfirmed { get; set; }
        public bool IsActive { get; set; }
        public string PIN { get; set; }
        public GradeDto Grade { get; set; }
        public CityDetailsDto Governorate { get; set; }
        public LiteAttachmentDto ProfilePhoto { get; set; }
    }
}
