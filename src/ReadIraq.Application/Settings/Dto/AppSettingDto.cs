using Abp.Application.Services.Dto;
using Abp.AutoMapper;
using ReadIraq.Domain.Settings;
using System;
using System.ComponentModel.DataAnnotations;

namespace ReadIraq.Settings.Dto
{
    [AutoMapFrom(typeof(AppSetting))]
    public class AppSettingDto : EntityDto<Guid>
    {
        public string Key { get; set; }
        public string Value { get; set; }
    }

    [AutoMapTo(typeof(AppSetting))]
    public class CreateAppSettingDto
    {
        [Required]
        public string Key { get; set; }
        public string Value { get; set; }
    }
}
