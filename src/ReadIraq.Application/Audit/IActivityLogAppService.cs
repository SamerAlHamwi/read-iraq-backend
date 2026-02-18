using Abp.Application.Services;
using Abp.Application.Services.Dto;
using ReadIraq.Audit.Dto;
using System;

namespace ReadIraq.Audit
{
    public interface IActivityLogAppService : IAsyncCrudAppService<ActivityLogDto, Guid, PagedAndSortedResultRequestDto, CreateActivityLogDto, ActivityLogDto>
    {
    }
}
