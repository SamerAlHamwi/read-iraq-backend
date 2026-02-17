using Abp.Application.Services.Dto;
using ReadIraq.CrudAppServiceBase;
using ReadIraq.Teachers.Dto;
using System;

namespace ReadIraq.Teachers
{
    public interface ITeacherReviewAppService : IReadIraqAsyncCrudAppService<TeacherReviewDto, Guid, TeacherReviewDto, PagedTeacherReviewResultRequestDto, CreateTeacherReviewDto, UpdateTeacherReviewDto>
    {
    }
}
