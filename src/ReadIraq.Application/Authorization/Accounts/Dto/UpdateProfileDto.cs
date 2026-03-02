using Abp.Authorization.Users;
using System;
using System.ComponentModel.DataAnnotations;

namespace ReadIraq.Authorization.Accounts.Dto
{
    public class UpdateProfileDto
    {
        [Required]
        public string Name { get; set; }

        [Required]
        [StringLength(AbpUserBase.MaxEmailAddressLength)]
        public string EmailAddress { get; set; }

        public int? GradeId { get; set; }

        public int? GovernorateId { get; set; }

        public long? ProfilePhoto { get; set; }
    }
}
