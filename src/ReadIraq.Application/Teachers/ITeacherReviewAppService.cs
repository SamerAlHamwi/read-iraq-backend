using Abp.Application.Services.Dto;
using ReadIraq.CrudAppServiceBase;
using ReadIraq.Teachers.Dto;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ReadIraq.Teachers
{
    public interface ITeacherReviewAppService : IReadIraqAsyncCrudAppService<TeacherReviewDto, Guid, TeacherReviewDto, PagedTeacherReviewResultRequestDto, CreateTeacherReviewDto, UpdateTeacherReviewDto>
    {
        Task<List<TeacherRatingBreakdownDto>> GetRatingBreakdownAsync(Guid teacherProfileId);
    }
}
