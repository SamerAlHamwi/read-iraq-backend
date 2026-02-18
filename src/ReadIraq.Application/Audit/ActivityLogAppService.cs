using Abp.Application.Services;
using Abp.Application.Services.Dto;
using Abp.Authorization;
using Abp.Domain.Repositories;
using ReadIraq.CrudAppServiceBase;
using ReadIraq.Domain.Audit;
using ReadIraq.Audit.Dto;
using System;

namespace ReadIraq.Audit
{
    [AbpAuthorize]
    public class ActivityLogAppService : ReadIraqAsyncCrudAppService<ActivityLog, ActivityLogDto, Guid, ActivityLogDto, PagedAndSortedResultRequestDto, CreateActivityLogDto, ActivityLogDto>, IActivityLogAppService
    {
        public ActivityLogAppService(IRepository<ActivityLog, Guid> repository)
            : base(repository)
        {
        }
    }
}
