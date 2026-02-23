using Abp.Application.Services;
using ReadIraq.Domain.LessonSessions.Dto;
using System;

namespace ReadIraq.Domain.LessonSessions
{
    public interface ILessonReportAppService : IAsyncCrudAppService<LessonReportDto, Guid, PagedLessonReportResultRequestDto, CreateLessonReportDto, LessonReportDto>
    {
    }
}
