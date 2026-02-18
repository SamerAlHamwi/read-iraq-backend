using Abp.Application.Services.Dto;
using ReadIraq.CrudAppServiceBase;
using ReadIraq.LessonSessions.Dto;
using System;
using System.Threading.Tasks;

namespace ReadIraq.LessonSessions
{
    public interface ILessonSessionAppService : IReadIraqAsyncCrudAppService<LessonSessionDto, Guid, LiteLessonSessionDto,
        PagedLessonSessionResultRequestDto, CreateLessonSessionDto, UpdateLessonSessionDto>
    {
        Task MarkAsCompleteAsync(EntityDto<Guid> input);
        Task ReportIssueAsync(ReportSessionIssueInput input);
        // Comments logic could be here or in a separate CommentAppService
    }
}
