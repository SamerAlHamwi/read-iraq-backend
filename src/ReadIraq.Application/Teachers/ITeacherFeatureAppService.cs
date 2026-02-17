using Abp.Application.Services.Dto;
using ReadIraq.CrudAppServiceBase;
using ReadIraq.Teachers.Dto;
using System;
using System.Threading.Tasks;

namespace ReadIraq.Teachers
{
    public interface ITeacherFeatureAppService : IReadIraqAsyncCrudAppService<TeacherFeatureDto, Guid, LiteTeacherFeatureDto, PagedTeacherFeatureResultRequestDto, CreateTeacherFeatureDto, UpdateTeacherFeatureDto>
    {
    }
}
