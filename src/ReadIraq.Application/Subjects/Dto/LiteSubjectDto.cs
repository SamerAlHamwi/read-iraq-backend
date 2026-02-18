using Abp.Application.Services.Dto;
using Abp.AutoMapper;
using ReadIraq.Domain.Subjects;
using ReadIraq.Domain.Translations.Dto;
using System;
using System.Collections.Generic;

namespace ReadIraq.Subjects.Dto
{
    [AutoMapFrom(typeof(Subject))]
    public class LiteSubjectDto : EntityDto<Guid>
    {
        public List<TranslationDto> Name { get; set; }
        public LiteAttachmentDto Attachment { get; set; }
    }
}
