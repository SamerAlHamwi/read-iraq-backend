using ReadIraq.CrudAppServiceBase;
using ReadIraq.TermService.Dto;

namespace ReadIraq.TermService
{
    public interface ITermAppService : IReadIraqAsyncCrudAppService<TermDetailsDto, int, LiteTermDto, PagedTermResultRequestDto,
         CreateTermDto, UpdateTermDto>
    {
    }
}
