using Abp.Application.Services;
using Abp.Application.Services.Dto;
using ReadIraq.SavedItems.Dto;
using System;
using System.Threading.Tasks;

namespace ReadIraq.SavedItems
{
    public interface IUserSavedItemAppService : IApplicationService
    {
        Task<UserSavedItemDto> GetAsync(EntityDto<Guid> input);
        Task<PagedResultDto<LiteUserSavedItemDto>> GetAllAsync(PagedUserSavedItemResultRequestDto input);
        Task<UserSavedItemDto> CreateAsync(CreateUserSavedItemDto input);
        Task<UserSavedItemDto> UpdateAsync(UpdateUserSavedItemDto input);
        Task DeleteAsync(EntityDto<Guid> input);
        Task UnsaveAsync(UnsaveItemInput input);
    }

    public class UnsaveItemInput
    {
        public Guid ItemId { get; set; }
        public Enums.Enum.SavedItemType ItemType { get; set; }
    }
}
