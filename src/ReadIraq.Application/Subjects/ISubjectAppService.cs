using Abp.Application.Services.Dto;
using ReadIraq.CrudAppServiceBase;
using ReadIraq.Subjects.Dto;
using ReadIraq.Teachers.Dto;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ReadIraq.Subjects
{
    public interface ISubjectAppService : IReadIraqAsyncCrudAppService<SubjectDto, Guid, LiteSubjectDto, PagedSubjectResultRequestDto, CreateSubjectDto, UpdateSubjectDto>
    {
        Task ToggleActiveAsync(EntityDto<Guid> input);
        Task<List<SubjectTeachersDto>> GetTeachersBySubjectsAsync(List<Guid> subjectIds, int? gradeId);
        Task<List<LiteTeacherProfileDto>> GetTeachersBySubjectWithPriorityAsync(Guid subjectId);
    }
}
