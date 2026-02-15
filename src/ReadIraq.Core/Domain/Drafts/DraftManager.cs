using Abp.Domain.Repositories;
using Abp.Domain.Services;
using Abp.EntityFrameworkCore.Repositories;
using Abp.UI;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using ReadIraq.Domain.Attachments;
using ReadIraq.Domain.AttributeAndAttachmentsForDrafts;
using ReadIraq.Domain.AttributeForSourceTypeValuesForDrafts;
using ReadIraq.Domain.Cities;
using ReadIraq.Domain.Drafts.Dtos;
using ReadIraq.Domain.RequestForQuotationContacts.Dto;
using ReadIraq.Domain.RequestForQuotationContactsForDrafts;
using ReadIraq.Domain.RequestForQuotations.Dto;
using ReadIraq.Domain.ServiceValuesForDrafts;
using ReadIraq.Domain.SourceTypes;
using ReadIraq.Localization.SourceFiles;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static ReadIraq.Enums.Enum;

namespace ReadIraq.Domain.Drafts
{
    public class DraftManager : DomainService, IDraftManager
    {
        private readonly IRepository<Draft> _darftRepository;
        private readonly IAttachmentManager _attachmentManager;
        private readonly IMapper _mapper;
        private readonly CityManager _cityManager;
        private readonly ISourceTypeManager _sourceTypeManager;
        private readonly IRepository<AttributeAndAttachmentsForDraft> _attributeChoiceAndAttachmentsForDraftRepository;
        private readonly IRepository<RequestForQuotationContactsForDraft> _draftContactRepository;
        private readonly IServiceValuesForDraftManager _serviceValuesForDraftManager;
        private readonly IAttributeForSourceTypeValuesForDraftManager _attributeForSourceTypeValuesForDraftManager;

        public DraftManager(IRepository<Draft> darftRepository,
            IAttachmentManager attachmentManager,
            IMapper mapper,
            CityManager cityManager,
            ISourceTypeManager sourceTypeManager,
            IServiceValuesForDraftManager serviceValuesForDraftManager,
            IAttributeForSourceTypeValuesForDraftManager attributeForSourceTypeValuesForDraftManager,
            IRepository<AttributeAndAttachmentsForDraft> attributeChoiceAndAttachmentsForDraftRepository,
            IRepository<RequestForQuotationContactsForDraft> draftContactRepository)
        {
            _darftRepository = darftRepository;
            _attachmentManager = attachmentManager;
            _mapper = mapper;
            _cityManager = cityManager;
            _sourceTypeManager = sourceTypeManager;
            _attributeChoiceAndAttachmentsForDraftRepository = attributeChoiceAndAttachmentsForDraftRepository;
            _draftContactRepository = draftContactRepository;
            _serviceValuesForDraftManager = serviceValuesForDraftManager;
            _attributeForSourceTypeValuesForDraftManager = attributeForSourceTypeValuesForDraftManager;
        }


        public async Task<DraftDetailsDto> GetEntityDtoByIdAsync(int id)
        {
            var draft = await _darftRepository.GetAll()
                .AsNoTracking()
                .Include(x => x.AttributeForSourceTypeValues)
                .Include(x => x.Services)
                .Include(x => x.RequestForQuotationContacts)
                .Include(x => x.AttributeChoiceAndAttachments).ThenInclude(x => x.Attachments)
                .Where(x => x.Id == id)
                .FirstOrDefaultAsync();
            if (draft is null)
                throw new UserFriendlyException(Exceptions.ObjectWasNotFound, Tokens.Draft);
            var draftDto = _mapper.Map(draft, new DraftDetailsDto());
            var attachments = await _attachmentManager.GetByRefAsync(id, AttachmentRefType.Draft);
            var attchmentWithChoices = draftDto.AttributeChoiceAndAttachments.SelectMany(x => x.AttachmentIds).ToList();
            var attachmentOnlyIds = attachments.Where(x => !attchmentWithChoices.Contains(x.Id)).Select(x => x.Id).ToList();
            draftDto.AttributeChoiceAndAttachments.Add(new AttributeChoiceAndAttachmentForDraftDto
            {
                AttachmentIds = attachmentOnlyIds
            });
            foreach (var item in draftDto.AttributeChoiceAndAttachments)
            {
                foreach (var attachmentId in item.AttachmentIds)
                {
                    var attachment = attachments.Where(x => x.Id == attachmentId).FirstOrDefault();
                    if (attachment != null)
                    {
                        item.Attachments.Add(new LiteAttachmentDto
                        {
                            Id = attachmentId,
                            Url = _attachmentManager.GetUrl(attachment),
                            LowResolutionPhotoUrl = _attachmentManager.GetLowResolutionPhotoUrl(attachment)
                        });
                    }
                }
            }
            if (draft.SourceTypeId.HasValue)
                if (draft.SourceTypeId.Value != 0)
                    draftDto.SourceType = await _sourceTypeManager.GetEntityDtoByIdAsync(draft.SourceTypeId.Value);
            if (draft.SourceCityId.HasValue)
                if (draft.SourceCityId.Value != 0)
                    draftDto.SourceCity = await _cityManager.GetEntityDtoByIdAsync(draft.SourceCityId.Value);
            if (draft.DestinationCityId.HasValue)
                if (draft.DestinationCityId.Value != 0)
                    draftDto.DestinationCity = await _cityManager.GetEntityDtoByIdAsync(draft.DestinationCityId.Value);
            //draftDto.AttributeForSourceTypeValues = await _attributeForSourceTypeValuesForDraftManager.GetAllAttributeForSourceTypeValuesByDraftId(draft.Id);
            draftDto.RequestForQuotationContacts = await GetAllContactsByDraftId(draft.Id);
            draftDto.Services = await _serviceValuesForDraftManager.GetFullServicesByDraftIdAsync(draft.Id);
            return draftDto;
        }
        public async Task DeleteAllOldDrafts()
        {
            var date = DateTime.UtcNow;
            await _darftRepository.GetAll()
                .Where(x => x.CreationTime.Date.AddDays(15) <= date.Date)
                .ExecuteUpdateAsync(se => se.SetProperty(x => x.IsDeleted, true));

        }

        public async Task<bool> CheckIfUserCanAddNewDraft(long userId)
        {
            if (await _darftRepository.GetAll().Where(x => x.UserId == userId && x.IsDeleted == false).CountAsync() <= 3)
                return true;
            return false;
        }
        public async Task<Draft> GetEntityById(int id)
        {
            var draft = await _darftRepository.GetAll()
                .Include(x => x.AttributeForSourceTypeValues)
                .Include(x => x.Services)
                .Include(x => x.RequestForQuotationContacts)
                .Where(x => x.Id == id).FirstOrDefaultAsync();
            if (draft is null)
                throw new UserFriendlyException(Exceptions.ObjectWasNotFound, Tokens.Draft);
            return draft;
        }



        public async Task UpdateAttacmentForDraft(int draftId, List<AttributeChoiceAndAttachmentDto> AttributeChoiceAndAttachments)
        {
            try
            {
                var attrbuiteChoiceAndattachmentIds = AttributeChoiceAndAttachments.Select(x => x.AttributeChoiceId).ToList();
                var oldAttributeChoiceAndAttachments = _attributeChoiceAndAttachmentsForDraftRepository.GetAll().Where(x => x.DraftId == draftId).ToList();
                var attrbuiteChoiceAndattachmentToDelete = oldAttributeChoiceAndAttachments.Where(x => !attrbuiteChoiceAndattachmentIds.Contains(x.AttributeChoiceId)).ToList();
                var allNewAttachmentsIds = AttributeChoiceAndAttachments.Select(x => x.AttachmentIds).SelectMany(list => list).ToList();
                var currentAttachments = await _attachmentManager.GetByRefAsync(draftId, AttachmentRefType.Draft);
                var attachmentsToDelete = currentAttachments.Where(x => !allNewAttachmentsIds.Contains((x.Id))).ToList();

                if (attrbuiteChoiceAndattachmentToDelete.Count > 0 || attachmentsToDelete.Count > 0)
                    await DeleteAttachmentsAndAttributeChoicesForDraft(attrbuiteChoiceAndattachmentToDelete, attachmentsToDelete);

                var attributesAndAttachmentsToAdd = new List<AttributeAndAttachmentsForDraft>();
                foreach (var item in AttributeChoiceAndAttachments)
                {
                    List<Attachment> attachments = new List<Attachment>();
                    var imagesattachmentIdsToAdd = item.AttachmentIds.Except(currentAttachments.Select(x => x.Id).ToList());
                    foreach (var newAttachmentId in item.AttachmentIds)
                    {
                        var attachment = imagesattachmentIdsToAdd.Contains(newAttachmentId)
                        ? await _attachmentManager.CheckAndUpdateRefIdAsync(newAttachmentId, AttachmentRefType.RequestForQuotation, draftId, true, false)
                        : await _attachmentManager.GetByIdAsync(newAttachmentId);
                        attachments.Add(attachment);
                    }
                    if (item.AttributeChoiceId != null)
                    {
                        if (!(oldAttributeChoiceAndAttachments.Select(x => x.AttributeChoiceId).ToList()).Contains(item.AttributeChoiceId.Value))
                        {
                            attributesAndAttachmentsToAdd.Add(new AttributeAndAttachmentsForDraft()
                            {
                                AttributeChoiceId = item.AttributeChoiceId.Value,
                                DraftId = draftId,
                                Attachments = attachments
                            });
                        }
                        else
                        {
                            var attributeChoiceAndAttachmentToUpdate = oldAttributeChoiceAndAttachments.Where(x => x.AttributeChoiceId == item.AttributeChoiceId).FirstOrDefault();
                            attributeChoiceAndAttachmentToUpdate.Attachments = attachments;
                            await _attributeChoiceAndAttachmentsForDraftRepository.UpdateAsync(attributeChoiceAndAttachmentToUpdate);
                        }
                    }
                    else
                        attachments.ForEach(x => x.AttributeChoiceAndAttachmentId = null);
                    continue;
                }
                if (attributesAndAttachmentsToAdd is not null && attributesAndAttachmentsToAdd.Count > 0)
                    await _attributeChoiceAndAttachmentsForDraftRepository.InsertRangeAsync(attributesAndAttachmentsToAdd);
                await UnitOfWorkManager.Current.SaveChangesAsync();
            }
            catch (Exception ex) { throw; }

        }

        public async Task UpdateContcatForDraft(List<CreateRequestForQuotationContactDto> draftContacts, Draft draft)
        {
            var oldDraftContact = draft.RequestForQuotationContacts.ToList();
            var newDrfatContact = _mapper.Map<List<CreateRequestForQuotationContactDto>, List<RequestForQuotationContactsForDraft>>(draftContacts);
            foreach (var item in newDrfatContact)
            {
                if (item.RequestForQuotationContactType == RequestForQuotationContactType.Source)
                    item.Id = oldDraftContact.Where(x => x.RequestForQuotationContactType == RequestForQuotationContactType.Source).Select(x => x.Id).FirstOrDefault();
                else if (item.RequestForQuotationContactType == RequestForQuotationContactType.Destination)
                    item.Id = oldDraftContact.Where(x => x.RequestForQuotationContactType == RequestForQuotationContactType.Destination).Select(x => x.Id).FirstOrDefault();
                item.DraftId = draft.Id;
                await _draftContactRepository.UpdateAsync(item);

            }
            await UnitOfWorkManager.Current.SaveChangesAsync();
        }

        private async Task DeleteAttachmentsAndAttributeChoicesForDraft(List<AttributeAndAttachmentsForDraft> attrbuiteChoiceAndattachmentToDelete, List<Attachment> attachmentsToDelete)
        {
            foreach (var item in attrbuiteChoiceAndattachmentToDelete)
            {
                await _attributeChoiceAndAttachmentsForDraftRepository.DeleteAsync(item);
            }
            foreach (var existingAttachment in attachmentsToDelete)
            {
                await _attachmentManager.DeleteRefIdAsync(existingAttachment);
            }
        }
        public async Task HardDeleteDraftById(int draftId)
        {

            var draft = await _darftRepository.GetAll().Where(x => x.Id == draftId).FirstOrDefaultAsync();
            await _darftRepository.DeleteAsync(draft);

        }

        private async Task<List<RequestForQuotationContactDto>> GetAllContactsByDraftId(int draftId)
        {


            var contacts = await _draftContactRepository.GetAll().Where(x => x.DraftId == draftId).ToListAsync();

            return _mapper.Map<List<RequestForQuotationContactsForDraft>, List<RequestForQuotationContactDto>>(contacts);

        }
    }
}
