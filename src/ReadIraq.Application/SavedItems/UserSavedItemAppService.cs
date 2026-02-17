using Abp.Authorization;
using Abp.Domain.Repositories;
using Abp.Linq.Extensions;
using ReadIraq.CrudAppServiceBase;
using ReadIraq.Domain.SavedItems;
using ReadIraq.SavedItems.Dto;
using System;
using System.Linq;

namespace ReadIraq.SavedItems
{
    [AbpAuthorize]
    public class UserSavedItemAppService : ReadIraqAsyncCrudAppService<UserSavedItem, UserSavedItemDto, Guid, LiteUserSavedItemDto,
        PagedUserSavedItemResultRequestDto, CreateUserSavedItemDto, UpdateUserSavedItemDto>,
        IUserSavedItemAppService
    {
        public UserSavedItemAppService(IRepository<UserSavedItem, Guid> repository)
            : base(repository)
        {
        }

        protected override IQueryable<UserSavedItem> CreateFilteredQuery(PagedUserSavedItemResultRequestDto input)
        {
            return base.CreateFilteredQuery(input)
                .WhereIf(input.UserId.HasValue, x => x.UserId == input.UserId.Value)
                .WhereIf(input.ItemType.HasValue, x => x.ItemType == input.ItemType.Value)
                .WhereIf(input.ItemId.HasValue, x => x.ItemId == input.ItemId.Value);
        }
    }
}

