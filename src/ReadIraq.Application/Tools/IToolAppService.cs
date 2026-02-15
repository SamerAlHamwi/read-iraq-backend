using ReadIraq.CrudAppServiceBase;
using ReadIraq.Domain.Cities.Dto;
using ReadIraq.Domain.Toolss.Dto;
using System.Threading.Tasks;

namespace ReadIraq.AttributesForSourceType
{
    public interface IToolAppService : IReadIraqAsyncCrudAppService<ToolDetailsDto, int, LiteToolDto
        , PagedToolResultRequestDto, CreateToolDto, UpdateToolDto>
    {
        Task<ToolDetailsDto> SwitchActivationAsync(SwitchActivationInputDto input);
    }
}
