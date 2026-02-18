using Abp.Application.Services;
using Abp.Application.Services.Dto;
using ReadIraq.Gifts.Dto;
using System;
using System.Threading.Tasks;

namespace ReadIraq.Gifts
{
    public interface IGiftAppService : IApplicationService
    {
        Task GrantGiftAsync(GrantGiftDto input);
        Task<PagedResultDto<GiftDto>> GetAllAsync(PagedAndSortedResultRequestDto input);
    }
}
