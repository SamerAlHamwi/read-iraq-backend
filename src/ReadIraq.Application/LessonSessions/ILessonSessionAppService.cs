using Abp.Application.Services;
using Abp.Application.Services.Dto;
using ReadIraq.LessonSessions.Dto;
using System;
using System.Threading.Tasks;

namespace ReadIraq.LessonSessions
{
    public interface ILessonSessionAppService : IApplicationService
    {
        Task<LessonSessionDto> GetAsync(EntityDto<Guid> input);
        Task<PagedResultDto<LiteLessonSessionDto>> GetAllAsync(PagedLessonSessionResultRequestDto input);
        Task<LessonSessionDto> CreateAsync(CreateLessonSessionDto input);
        Task<LessonSessionDto> UpdateAsync(UpdateLessonSessionDto input);
        Task DeleteAsync(EntityDto<Guid> input);
    }
}

