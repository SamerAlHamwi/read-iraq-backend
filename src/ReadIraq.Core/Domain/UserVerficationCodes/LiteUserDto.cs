using Abp.Application.Services.Dto;
using Abp.AutoMapper;
using ReadIraq.Authorization.Users;
using System;
using static ReadIraq.Enums.Enum;

namespace ReadIraq.Domain.UserVerficationCodes
{
    [AutoMap(typeof(User))]
    public class LiteUserDto : EntityDto<long>
    {
        public string UserName { get; set; }

        public string Name { get; set; }

        public string Surname { get; set; }

        public string EmailAddress { get; set; }

        public bool IsActive { get; set; }

        public string FullName { get; set; }

        public DateTime CreationTime { get; set; }
        public UserType Type { get; set; }
        public string PhoneNumber { get; set; }
        public string PIN { get; set; }
        public string RegistrationFullName { get; set; }
    }
}
