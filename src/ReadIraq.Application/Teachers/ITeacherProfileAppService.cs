using Abp.Application.Services.Dto;
using ReadIraq.CrudAppServiceBase;
using ReadIraq.Teachers.Dto;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ReadIraq.Teachers
{
    public interface ITeacherProfileAppService : IReadIraqAsyncCrudAppService<TeacherProfileDto, Guid, LiteTeacherProfileDto, PagedTeacherProfileResultRequestDto, CreateTeacherProfileDto, UpdateTeacherProfileDto>
    {
        Task AssignSubjectsAsync(AssignSubjectsInput input);
        Task<TeacherStatsDto> GetStatsAsync(EntityDto<Guid> input);
        Task ToggleActiveAsync(EntityDto<Guid> input);
        Task FollowAsync(EntityDto<Guid> input);
        Task UnfollowAsync(EntityDto<Guid> input);
        Task<List<PreferredTeachersSectionDto>> GetPreferredTeachersByGradeAsync(int gradeId);
    }
}
