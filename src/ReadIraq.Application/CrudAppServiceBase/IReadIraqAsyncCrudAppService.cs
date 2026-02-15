using Abp.Application.Services;
using Abp.Application.Services.Dto;
using System.Threading.Tasks;

namespace ReadIraq.CrudAppServiceBase
{
    public interface IReadIraqAsyncCrudAppService<TDetailsEntityDto, TPrimaryKey, TLiteEntityDto, in TGetAllInput, in TCreateInput, in TUpdateInput>
       : IApplicationService
       where TDetailsEntityDto : IEntityDto<TPrimaryKey>
       where TLiteEntityDto : IEntityDto<TPrimaryKey>
       where TUpdateInput : IEntityDto<TPrimaryKey>
    {
        Task<TDetailsEntityDto> GetAsync(EntityDto<TPrimaryKey> input);

        Task<PagedResultDto<TLiteEntityDto>> GetAllAsync(TGetAllInput input);

        Task<TDetailsEntityDto> CreateAsync(TCreateInput input);

        Task<TDetailsEntityDto> UpdateAsync(TUpdateInput input);

        Task DeleteAsync(EntityDto<TPrimaryKey> input);




    }

}
