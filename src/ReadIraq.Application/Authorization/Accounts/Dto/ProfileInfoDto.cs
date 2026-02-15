namespace ReadIraq.Authorization.Accounts.Dto
{
    public class ProfileInfoDto
    {
        public string RegistrationFullName { get; set; }
        public string Name { get; set; }
        public string Surname { get; set; }
        public string EmailAddress { get; set; }
        public string DialCode { get; set; }
        public string PhoneNumber { get; set; }
        public bool IsEmailConfirmed { get; set; }
        public LiteAttachmentDto ProfilePhoto { get; set; }
        public string PIN { get; set; }
        public bool IsDeleted { get; set; }
        public bool IsActive { get; set; }
        public int CompanyId { get; set; }
        public int CompanyBranchId { get; set; }
    }
}
