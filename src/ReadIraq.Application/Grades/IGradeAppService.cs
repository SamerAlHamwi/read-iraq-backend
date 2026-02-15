using Abp.Application.Services.Dto;
using ReadIraq.CrudAppServiceBase;
using ReadIraq.Grades.Dto;
using System.Threading.Tasks;

namespace ReadIraq.Grades
{
    public interface IGradeAppService : IReadIraqAsyncCrudAppService<GradeDto, int, LiteGradeDto, PagedGradeResultRequestDto, CreateGradeDto, UpdateGradeDto>
    {
    }
}
