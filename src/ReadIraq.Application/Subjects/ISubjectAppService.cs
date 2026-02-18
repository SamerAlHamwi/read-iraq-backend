using Abp.Application.Services.Dto;
using ReadIraq.CrudAppServiceBase;
using ReadIraq.Subjects.Dto;
using System;
using System.Threading.Tasks;

namespace ReadIraq.Subjects
{
    public interface ISubjectAppService : IReadIraqAsyncCrudAppService<SubjectDto, Guid, LiteSubjectDto, PagedSubjectResultRequestDto, CreateSubjectDto, UpdateSubjectDto>
    {
        Task ToggleActiveAsync(EntityDto<Guid> input);
    }
}
