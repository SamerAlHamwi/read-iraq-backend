using Abp.Application.Services.Dto;
using Abp.Authorization;
using Abp.Domain.Repositories;
using Abp.Extensions;
using Abp.UI;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ReadIraq.Authorization;
using ReadIraq.CrudAppServiceBase;
using ReadIraq.Domain.Attachments;
using ReadIraq.Domain.Cities.Dto;
using ReadIraq.Domain.services;
using ReadIraq.Domain.ServiceValueForOffers;
using ReadIraq.Domain.ServiceValues;
using ReadIraq.Domain.SourceTypes;
using ReadIraq.Domain.SubServices;
using ReadIraq.Domain.Toolss;
using ReadIraq.Domain.Toolss.Dto;
using ReadIraq.Localization.SourceFiles;
using System;
using System.Linq;
using System.Threading.Tasks;
using static ReadIraq.Enums.Enum;

namespace ReadIraq.AttributesForSourceType
{
    public class ToolAppService : ReadIraqAsyncCrudAppService<Tool, ToolDetailsDto, int, LiteToolDto
        , PagedToolResultRequestDto, CreateToolDto, UpdateToolDto>, IToolAppService
    {
        private readonly SourceTypeManager _sourceTypeManager;
        private readonly ToolManger _toolManager;
        private readonly ServiceManager _serviceManager;
        private readonly SubServiceManager _subServiceManager;
        private readonly IServiceValueManager _serviceValueManager;
        private readonly IServiceValueForOfferManager _serviceValueForOfferManager;
        private readonly AttachmentManager _attachmentManager;
        private readonly IRepository<Tool> _toolRepository;
        public ToolAppService(IRepository<Tool, int> repository,
            IRepository<Tool> toolRepository,
            SourceTypeManager sourceTypeManager,
            ToolManger toolManager,
            ServiceManager serviceManager,
            SubServiceManager subServiceManager,
            IServiceValueManager serviceValueManager,
            IServiceValueForOfferManager serviceValueForOfferManager,
            AttachmentManager attachmentManager)
            : base(repository)
        {
            _toolRepository = toolRepository;
            _sourceTypeManager = sourceTypeManager;
            _toolManager = toolManager;
            _serviceManager = serviceManager;
            _subServiceManager = subServiceManager;
            _serviceValueManager = serviceValueManager;
            _serviceValueForOfferManager = serviceValueForOfferManager;
            _attachmentManager = attachmentManager;
        }
        [AbpAuthorize(PermissionNames.Tool_FullControl)]
        public override async Task<ToolDetailsDto> CreateAsync(CreateToolDto input)
        {
            var tool = MapToEntity(input);
            tool.SubServiceId = input.SubServiceId;
            tool.IsActive = true;
            await Repository.InsertAndGetIdAsync(tool);
            await UnitOfWorkManager.Current.SaveChangesAsync();
            await _attachmentManager.CheckAndUpdateRefIdAsync(input.AttachmentId, AttachmentRefType.Tool, tool.Id);
            return MapToEntityDto(tool);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public override async Task<ToolDetailsDto> GetAsync(EntityDto<int> input)
        {
            var tool = await _toolManager.GetEntityByIdAsync(input.Id);
            //foreach (var item in tool.Services)
            //{
            //    var attachment = await _attachmentManager.GetElementByRefAsync(item.Id, AttachmentRefType.Service);
            //    if (attachment is not null)
            //    {

            //        item.Attachment = (new LiteAttachmentDto
            //        {
            //            Id = attachment.Id,
            //            Url = _attachmentManager.GetUrl(attachment),
            //            LowResolutionPhotoUrl = _attachmentManager.GetLowResolutionPhotoUrl(attachment),
            //        });
            //    }
            //}
            var attachment = await _attachmentManager.GetElementByRefAsync(tool.Id, AttachmentRefType.Tool);
            if (attachment is not null)
            {
                tool.Attachment = new LiteAttachmentDto
                {
                    Id = attachment.Id,
                    Url = _attachmentManager.GetUrl(attachment),
                    LowResolutionPhotoUrl = _attachmentManager.GetLowResolutionPhotoUrl(attachment),
                };
            }
            return tool;

        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [AbpAllowAnonymous]
        public override async Task<PagedResultDto<LiteToolDto>> GetAllAsync(PagedToolResultRequestDto input)
        {

            var result = await base.GetAllAsync(input);
            var attachments = await _attachmentManager.GetByRefTypeAsync(AttachmentRefType.Tool);
            foreach (var item in result.Items)
            {
                var attachment = attachments.Where(x => x.RefId == item.Id).FirstOrDefault();
                if (attachment is not null)
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

        /// <summary>
        /// 
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [AbpAuthorize(PermissionNames.Tool_FullControl)]
        public override async Task<ToolDetailsDto> UpdateAsync(UpdateToolDto input)
        {
            var tool = await _toolManager.GetFullEntityByIdAsync(input.Id);
            await _toolManager.HardDeleteTranslationAsync(tool.Translations.ToList());
            MapToEntity(input, tool);
            tool.SubServiceId = input.SubServiceId;
            tool.LastModificationTime = DateTime.UtcNow;
            await Repository.UpdateAsync(tool);
            await CurrentUnitOfWork.SaveChangesAsync();
            var oldAttachment = await _attachmentManager.GetElementByRefAsync(input.Id, AttachmentRefType.Tool);
            if (oldAttachment is not null)
            {
                await _attachmentManager.DeleteRefIdAsync(oldAttachment);
            }
            await _attachmentManager.CheckAndUpdateRefIdAsync(input.AttachmentId, AttachmentRefType.Tool, input.Id);

            return MapToEntityDto(tool);

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [ApiExplorerSettings(IgnoreApi = false)]
        [AbpAuthorize(PermissionNames.Tool_FullControl)]
        public override async Task DeleteAsync(EntityDto<int> input)
        {
            CheckDeletePermission();
            var tool = await _toolManager.GetFullEntityByIdAsync(input.Id);

            if (tool is null)
                throw new UserFriendlyException(404, Exceptions.ObjectWasNotFound, Tokens.Tool);

            if (await _serviceValueManager.CheckIfToolBelongsToRequest(input.Id))
                throw new UserFriendlyException(Exceptions.ObjectCantBeDelete, Tokens.Tool);

            if (await _serviceValueManager.CheckIfToolBelongsToCompanyOrCompanyBranch(input.Id))
                throw new UserFriendlyException(Exceptions.ObjectCantBeDelete, Tokens.Tool);

            if (await _serviceValueForOfferManager.CheckIfToolBelongsToOffer(input.Id))
                throw new UserFriendlyException(Exceptions.ObjectCantBeDelete, Tokens.Tool);

            await _toolManager.HardDeleteTranslationAsync(tool.Translations.ToList());
            await Repository.DeleteAsync(tool);

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        protected override IQueryable<Tool> CreateFilteredQuery(PagedToolResultRequestDto input)
        {
            var data = base.CreateFilteredQuery(input);
            data = data.Include(x => x.Translations);
            data = data.Include(c => c.SubService).ThenInclude(x => x.Translations).AsNoTracking();
            if (!input.Keyword.IsNullOrEmpty())
                data = data.Where(x => x.Translations.Where(x => x.Name.Contains(input.Keyword)).Any()).AsNoTracking();
            if (input.SubServiceId.HasValue)
                data = data.Where(x => x.SubServiceId == input.SubServiceId).AsNoTracking();
            if (input.IsActive.HasValue)
                data = data.Where(x => x.IsActive == input.IsActive.Value);
            return data;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="query"></param>
        /// <param name="input"></param>
        /// <returns></returns>
        protected override IQueryable<Tool> ApplySorting(IQueryable<Tool> query, PagedToolResultRequestDto input)
        {
            return query.OrderByDescending(r => r.CreationTime.Date);
        }

        /// <summary>
        /// Switch Activation of A Tool
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [HttpPut]
        [AbpAuthorize(PermissionNames.Tool_FullControl)]
        public async Task<ToolDetailsDto> SwitchActivationAsync(SwitchActivationInputDto input)
        {
            CheckUpdatePermission();
            var tool = await _toolManager.GetLiteEntityByIdAsync(input.Id);
            tool.IsActive = input.IsActive;
            tool.LastModificationTime = DateTime.UtcNow;
            await _toolRepository.UpdateAsync(tool);
            return MapToEntityDto(tool);
        }

    }
}
