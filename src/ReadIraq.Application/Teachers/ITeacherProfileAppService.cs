using Abp.Application.Services.Dto;
using ReadIraq.CrudAppServiceBase;
using ReadIraq.Teachers.Dto;
using System;
using System.Threading.Tasks;

namespace ReadIraq.Teachers
{
    public interface ITeacherProfileAppService : IReadIraqAsyncCrudAppService<TeacherProfileDto, Guid, LiteTeacherProfileDto, PagedTeacherProfileResultRequestDto, CreateTeacherProfileDto, UpdateTeacherProfileDto>
    {
    }
}
