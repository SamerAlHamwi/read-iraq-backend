using Abp.Application.Services.Dto;
using Abp.Authorization;
using Abp.Domain.Repositories;
using Abp.UI;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ReadIraq;
using ReadIraq.Advertisiments;
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

namespace ClinicSystem.Advertisiments
{
    public class AdvertisimentAppService : ReadIraqAsyncCrudAppService<Advertisiment, AdvertisimentDetailsDto, int, LiteAdvertisimentDto,
          PagedAdvertisimentResultRequestDto, CreateAdvertisimentDto, UpdateAdvertisimentDto>,
          IAdvertisimentAppService
    {

        private readonly IAttachmentManager _attachmentManager;
        private readonly UserManager _userManager;
        private readonly IAdvertisimentManager _advertisimentManager;
        public AdvertisimentAppService(IRepository<Advertisiment> repository
            , IAttachmentManager attachmentManager, UserManager userManager
            , IAdvertisimentManager advertisimentManager
)
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
            var Advertisiment = await Repository.GetAsync(input.Id);

            if (Advertisiment is null)
            {
                throw new UserFriendlyException(string.Format(Exceptions.ObjectWasNotFound, Tokens.Advertisiment));
            }

            MapToEntity(input, Advertisiment);
            Advertisiment.LastModificationTime = DateTime.UtcNow;
            await Repository.UpdateAsync(Advertisiment);
            var oldAttachments = await _attachmentManager.GetByRefAsync(input.Id.ToString(), AttachmentRefType.Advertisiment);
            /*var attachmentsToDelete = oldAttachments.Where(x => !input.AttachmentIds.Contains(x.Id));
            var attachmentIdsToAdd = input.AttachmentIds.Except(oldAttachments.Select(x => x.Id).ToList());*/
            /* foreach (var attachment in attachmentsToDelete)
             {
                 await _attachmentManager.DeleteRefIdAsync(attachment);
             }*/
            /*foreach (var attachmentId in attachmentIdsToAdd)
            {
                await _attachmentManager.CheckAndUpdateRefIdAsync(
                    attachmentId, AttachmentRefType.Advertisiment, input.Id);
            }*/
            var AdvertisimentDto = MapToEntityDto(Advertisiment);
            return AdvertisimentDto;
        }
        [HttpPost]
        public async Task AddAdvertisimentPositionToAdvertisiment(AddAdvertisimentPositionDto input)
        {
            var Advertisiment = await Repository.GetAsync(input.AdvertisimentId);
            if (Advertisiment is null)
                throw new UserFriendlyException(string.Format(Exceptions.ObjectWasNotFound, Tokens.Advertisiment));
            AdvertisimentPosition advertisimentPossition = ObjectMapper.Map<AdvertisimentPosition>(input);

            await _advertisimentManager.AddPositionToAdvertisimentAsync(advertisimentPossition);
        }

        [AbpAllowAnonymous]
        public override async Task<PagedResultDto<LiteAdvertisimentDto>> GetAllAsync(PagedAdvertisimentResultRequestDto input)
        {

            var result = await base.GetAllAsync(input);

            foreach (LiteAdvertisimentDto item in result.Items)
            {
                var user = await _userManager.GetUserByIdAsync((long)item.CreatorUserId);
                item.CreatorUserName = user.FullName;

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
            advertisiment.AdvertisimentPositions = ObjectMapper.Map<List<AdvertisimentPosition>>(input.AdvertisimentPositions);
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
            var PostDto = MapToEntityDto(advertisiment);
            return PostDto;
        }
        protected override IQueryable<Advertisiment> CreateFilteredQuery(PagedAdvertisimentResultRequestDto input)
        {
            var data = base.CreateFilteredQuery(input);
            data = data.Include(x => x.AdvertisimentPositions);
            if (input.Position.HasValue)
            {
                data = data.Where(x => x.AdvertisimentPositions.Where(x => x.Position == input.Position.Value).Any());
            }
            data = data.Where(x => !x.IsDeleted);
            if (input.IsActive.HasValue)
            {
                data = data.Where(x => x.IsActive == input.IsActive);
            }

            return data;


        }

        /// <summary>
        /// GetAsync
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public override async Task<AdvertisimentDetailsDto> GetAsync(EntityDto<int> input)
        {

            var Advertisiment = await Repository.GetAsync(input.Id);
            if (Advertisiment is null)
                throw new UserFriendlyException(string.Format(Exceptions.ObjectWasNotFound, Tokens.Advertisiment));
            var user = await _userManager.GetUserByIdAsync((long)Advertisiment.CreatorUserId);
            AdvertisimentDetailsDto advertisimentDto = MapToEntityDto(Advertisiment);
            advertisimentDto.CreatorUserName = user.FullName;
            advertisimentDto.AdvertisimentPositions = ObjectMapper.Map<List<AdvertisimentPositionDto>>(await _advertisimentManager.GetAdvertisimentPositionsAsync(input.Id));

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
            return query.OrderBy(r => r.CreationTime);
        }

    }
}
