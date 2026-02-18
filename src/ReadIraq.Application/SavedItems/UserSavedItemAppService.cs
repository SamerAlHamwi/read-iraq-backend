using Abp.Authorization;
using Abp.Domain.Repositories;
using Abp.Linq.Extensions;
using Abp.UI;
using Microsoft.EntityFrameworkCore;
using ReadIraq.CrudAppServiceBase;
using ReadIraq.Domain.SavedItems;
using ReadIraq.SavedItems.Dto;
using System;
using System.Linq;
using System.Threading.Tasks;

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

        public override async Task<UserSavedItemDto> CreateAsync(CreateUserSavedItemDto input)
        {
            var exists = await Repository.GetAll().AnyAsync(x => x.UserId == input.UserId && x.ItemType == input.ItemType && x.ItemId == input.ItemId);
            if (exists)
            {
                throw new UserFriendlyException(L("AlreadySaved"));
            }

            return await base.CreateAsync(input);
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
