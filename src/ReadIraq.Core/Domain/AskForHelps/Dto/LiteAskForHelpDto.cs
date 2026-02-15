using Abp.Application.Services.Dto;
using ReadIraq.Domain.UserVerficationCodes;
using System;
using static ReadIraq.Enums.Enum;

namespace ReadIraq.Domain.AskForHelps.Dto
{
    public class LiteAskForHelpDto : EntityDto
    {
        public LiteUserDto User { get; set; }
        public AskForHelpStatues Statues { get; set; }
        public string Message { get; set; }
        public DateTime CreationTime { get; set; }

    }
}
