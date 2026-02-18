using Abp.Application.Services;
using Abp.Application.Services.Dto;
using ReadIraq.Settings.Dto;
using System;
using System.Threading.Tasks;

namespace ReadIraq.Settings
{
    public interface IAppSettingAppService : IAsyncCrudAppService<AppSettingDto, Guid, PagedAndSortedResultRequestDto, CreateAppSettingDto, AppSettingDto>
    {
        Task<AppSettingDto> GetByKeyAsync(string key);
    }
}
