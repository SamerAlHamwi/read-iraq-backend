using Abp.Application.Services.Dto;
using ReadIraq.CrudAppServiceBase;
using ReadIraq.LessonSessions.Dto;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ReadIraq.LessonSessions
{
    public interface ILessonSessionAppService : IReadIraqAsyncCrudAppService<LessonSessionDto, Guid, LiteLessonSessionDto,
        PagedLessonSessionResultRequestDto, CreateLessonSessionDto, UpdateLessonSessionDto>
    {
        Task MarkAsCompleteAsync(EntityDto<Guid> input);
        Task UpdateProgressAsync(UpdateLessonProgressInput input);
        Task ReportIssueAsync(ReportSessionIssueInput input);
        Task<ListResultDto<LessonSessionDto>> GetByIdsAsync(GetLessonsByIdsInput input);
    }
}
