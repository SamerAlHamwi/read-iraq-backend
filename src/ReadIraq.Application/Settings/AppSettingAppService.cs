using Abp.Application.Services;
using Abp.Application.Services.Dto;
using Abp.Authorization;
using Abp.Domain.Repositories;
using Microsoft.EntityFrameworkCore;
using ReadIraq.CrudAppServiceBase;
using ReadIraq.Domain.Settings;
using ReadIraq.Settings.Dto;
using System;
using System.Threading.Tasks;

namespace ReadIraq.Settings
{
    [AbpAuthorize]
    public class AppSettingAppService : ReadIraqAsyncCrudAppService<AppSetting, AppSettingDto, Guid, AppSettingDto, PagedAndSortedResultRequestDto, CreateAppSettingDto, AppSettingDto>, IAppSettingAppService
    {
        public AppSettingAppService(IRepository<AppSetting, Guid> repository)
            : base(repository)
        {
        }

        public async Task<AppSettingDto> GetByKeyAsync(string key)
        {
            var entity = await Repository.FirstOrDefaultAsync(x => x.Key == key);
            return MapToEntityDto(entity);
        }
    }
}
