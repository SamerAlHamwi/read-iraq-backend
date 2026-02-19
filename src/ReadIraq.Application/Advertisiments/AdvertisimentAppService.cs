using Abp.Application.Services.Dto;
using Abp.Authorization;
using Abp.Domain.Repositories;
using Abp.UI;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ReadIraq.Advertisiments.Dto;
using ReadIraq.Authorization.Users;
using ReadIraq.CrudAppServiceBase;
using ReadIraq.Domain.Advertisiments;
using ReadIraq.Domain.Attachments;
using ReadIraq.Localization.SourceFiles;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static ReadIraq.Enums.Enum;

namespace ReadIraq.Advertisiments
{
    public class AdvertisimentAppService : ReadIraqAsyncCrudAppService<Advertisiment, AdvertisimentDetailsDto, int, LiteAdvertisimentDto,
          PagedAdvertisimentResultRequestDto, CreateAdvertisimentDto, UpdateAdvertisimentDto>,
          IAdvertisimentAppService
    {
        private readonly IAttachmentManager _attachmentManager;
        private readonly UserManager _userManager;
        private readonly IAdvertisimentManager _advertisimentManager;

        public AdvertisimentAppService(
            IRepository<Advertisiment> repository,
            IAttachmentManager attachmentManager,
            UserManager userManager,
            IAdvertisimentManager advertisimentManager)
         : base(repository)
        {
            _attachmentManager = attachmentManager;
            _userManager = userManager;
            _advertisimentManager = advertisimentManager;
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        public override async Task<AdvertisimentDetailsDto> UpdateAsync(UpdateAdvertisimentDto input)
        {
            CheckUpdatePermission();
            var advertisiment = await Repository.GetAsync(input.Id);

            if (advertisiment is null)
            {
                throw new UserFriendlyException(string.Format(Exceptions.ObjectWasNotFound, Tokens.Advertisiment));
            }

            MapToEntity(input, advertisiment);
            advertisiment.LastModificationTime = DateTime.UtcNow;
            await Repository.UpdateAsync(advertisiment);

            return MapToEntityDto(advertisiment);
        }

        [AbpAllowAnonymous]
        public override async Task<PagedResultDto<LiteAdvertisimentDto>> GetAllAsync(PagedAdvertisimentResultRequestDto input)
        {
            var result = await base.GetAllAsync(input);

            foreach (LiteAdvertisimentDto item in result.Items)
            {
                if (item.CreatorUserId.HasValue)
                {
                    var user = await _userManager.GetUserByIdAsync((long)item.CreatorUserId);
                    item.CreatorUserName = user.FullName;
                }

                var attachment = await _attachmentManager.GetElementByRefAsync(item.Id.ToString(), AttachmentRefType.Advertisiment);
                if (attachment != null)
                {
                    item.Attachment = new LiteAttachmentDto
                    {
                        Id = attachment.Id,
                        Url = _attachmentManager.GetUrl(attachment),
                        LowResolutionPhotoUrl = _attachmentManager.GetLowResolutionPhotoUrl(attachment),
                    };
                }
            }
            return result;
        }

        [AbpAuthorize]
        public override async Task<AdvertisimentDetailsDto> CreateAsync(CreateAdvertisimentDto input)
        {
            CheckCreatePermission();
            var advertisiment = ObjectMapper.Map<Advertisiment>(input);
            advertisiment.CreatorUserId = AbpSession.UserId.Value;
            advertisiment.IsActive = true;
            advertisiment.CreationTime = DateTime.UtcNow;

            var advertismentId = await Repository.InsertAndGetIdAsync(advertisiment);
            await UnitOfWorkManager.Current.SaveChangesAsync();
            await _attachmentManager.CheckAndUpdateRefIdAsync(input.AttachmentId, AttachmentRefType.Advertisiment, advertismentId.ToString());
            return MapToEntityDto(advertisiment);
        }

        [HttpPut]
        public async Task<AdvertisimentDetailsDto> SwitchActivationAsync(IEntityDto<int> input)
        {
            CheckUpdatePermission();
            var advertisiment = await Repository.GetAsync(input.Id);
            if (advertisiment is null)
            {
                throw new UserFriendlyException(string.Format(Exceptions.ObjectWasNotFound, Tokens.Advertisiment));
            }
            advertisiment.IsActive = !advertisiment.IsActive;
            advertisiment.LastModificationTime = DateTime.UtcNow;
            await Repository.UpdateAsync(advertisiment);
            return MapToEntityDto(advertisiment);
        }

        protected override IQueryable<Advertisiment> CreateFilteredQuery(PagedAdvertisimentResultRequestDto input)
        {
            var data = base.CreateFilteredQuery(input);
            data = data.Where(x => !x.IsDeleted);
            if (input.IsActive.HasValue)
            {
                data = data.Where(x => x.IsActive == input.IsActive);
            }
            return data;
        }

        public override async Task<AdvertisimentDetailsDto> GetAsync(EntityDto<int> input)
        {
            var advertisiment = await Repository.GetAsync(input.Id);
            if (advertisiment is null)
                throw new UserFriendlyException(string.Format(Exceptions.ObjectWasNotFound, Tokens.Advertisiment));

            var user = await _userManager.GetUserByIdAsync((long)advertisiment.CreatorUserId);
            AdvertisimentDetailsDto advertisimentDto = MapToEntityDto(advertisiment);
            advertisimentDto.CreatorUserName = user.FullName;

            var attachment = await _attachmentManager.GetElementByRefAsync(advertisimentDto.Id.ToString(), AttachmentRefType.Advertisiment);
            if (attachment != null)
            {
                advertisimentDto.Attachment = new LiteAttachmentDto
                {
                    Id = attachment.Id,
                    Url = _attachmentManager.GetUrl(attachment),
                    LowResolutionPhotoUrl = _attachmentManager.GetLowResolutionPhotoUrl(attachment),
                };
            }

            return advertisimentDto;
        }

        protected override IQueryable<Advertisiment> ApplySorting(IQueryable<Advertisiment> query, PagedAdvertisimentResultRequestDto input)
        {
            return query.OrderByDescending(r => r.CreationTime);
        }
    }
}
