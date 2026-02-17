using ReadIraq.CrudAppServiceBase;
using ReadIraq.Domain.Grades.Dto;
using System;

namespace ReadIraq.Grades
{
    public interface IGradeGroupAppService : IReadIraqAsyncCrudAppService<GradeGroupDto, Guid, GradeGroupDto, PagedGradeGroupResultRequestDto, CreateGradeGroupDto, UpdateGradeGroupDto>
    {
    }
}
