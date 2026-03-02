using Abp.Domain.Repositories;
using Abp.Domain.Services;
using Abp.UI;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static ReadIraq.Enums.Enum;

namespace ReadIraq.Domain.Attachments
{
    public class AttachmentManager : DomainService, IAttachmentManager
    {
        private readonly IRepository<Attachment, long> _repository;
        private readonly string _appBaseUrl;

        public AttachmentManager(IRepository<Attachment, long> repository, IConfiguration configuration)

        {
            _repository = repository;
            _appBaseUrl = configuration[ReadIraqConsts.AppServerRootAddressKey] ?? "/";
            LocalizationSourceName = ReadIraqConsts.LocalizationSourceName;
        }

        public async Task<Attachment> GetByIdAsync(long id)
        {
            var attachment = await _repository.FirstOrDefaultAsync(id);

            if (attachment == null)
                throw new UserFriendlyException(L("AttachmentIsNotFound"), $"Id: {id}");

            return attachment;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="ids"></param>
        /// <returns></returns>
        public async Task<List<Attachment>> GetListByIdsAsync(List<long> ids)
        {
            return await _repository.GetAllListAsync(x => ids.Contains(x.Id));
        }
        public async Task<Attachment> GetAndCheckAsync(long id, AttachmentRefType refType, bool IsForDraft = false)
        {
            var attachment = await GetByIdAsync(id);
            if (attachment.RefType != refType)
                throw new UserFriendlyException(L("InvalidAttachmentRefType"),
                    $"Id: {id}, RefType: {attachment.RefType} and should be {(byte)refType}");

            return attachment;
        }

        public string GetUrl(Attachment attachment)
        {
            if (attachment == null) return null;

            var baseUri = new Uri(_appBaseUrl);
            if (!string.IsNullOrWhiteSpace(attachment.Url))
            {
                return attachment.Url;
            }

            if (string.IsNullOrWhiteSpace(attachment.StorageKey))
            {
                return null;
            }

            return (new Uri(baseUri, attachment.StorageKey)).AbsoluteUri;
        }
        public string GetLowResolutionPhotoUrl(Attachment attachment)
        {
            if (attachment == null || string.IsNullOrWhiteSpace(attachment.LowResolutionPhotoRelativePath))
            {
                return null;
            }

            var baseUri = new Uri(_appBaseUrl);
            return (new Uri(baseUri, attachment.LowResolutionPhotoRelativePath)).AbsoluteUri;
        }
        public async Task UpdateRefIdAsync(Attachment attachment, string refId, bool IsForDraft = false, bool IsForRequest = true)
        {
            if (attachment.RefId != null && attachment.RefId != refId && !IsForDraft)
                throw new UserFriendlyException(L("AttachmentAlreadyRelatedToEntity"),
                    $"Id: {attachment.Id}, RefType: {attachment.RefType}");

            if (attachment.RefId == refId) return;

            attachment.RefId = refId;
            await _repository.UpdateAsync(attachment);
        }

        public async Task<Attachment> CheckAndUpdateRefIdAsync(long id, AttachmentRefType refType, string refId, bool IsForDraft = false, bool IsForRequest = true)
        {
            //Check if type is correct and update refId
            var attachment = await GetAndCheckAsync(id, refType, IsForDraft);
            await UpdateRefIdAsync(attachment, refId, IsForDraft, IsForRequest);

            return attachment;
        }

        public async Task DeleteRefIdAsync(Attachment attachment)
        {
            attachment.RefId = null;
            await _repository.UpdateAsync(attachment);
        }

        public async Task DeleteAllRefIdAsync(string refId, AttachmentRefType refType)
        {
            foreach (var attachment in await GetByRefAsync(refId, refType))
            {
                attachment.RefId = null;
                await _repository.UpdateAsync(attachment);
            }
        }

        public void CheckAttachmentRefType(AttachmentRefType refType, MediaType fileType)
        {
            if (!AcceptedTypesFor(refType).Contains(fileType))
                throw new UserFriendlyException(L("FileTypeIncompatibleWithRefType"),
                    $"Type:{fileType.ToString()}, RefType:{refType.ToString()}");
        }

        public async Task<List<Attachment>> GetByRefAsync(string refId, AttachmentRefType refType)
        {
            return await _repository.GetAllListAsync(x => x.RefId == refId && x.RefType == refType);
        }
        public async Task<List<Attachment>> GetByRefAsync(List<string> refIds, AttachmentRefType refType)
        {
            return await _repository.GetAllListAsync(x => refIds.Contains(x.RefId) && x.RefType == refType);
        }
        public async Task<List<Attachment>> GetByRefTypeAsync(AttachmentRefType refType)
        {
            return await _repository.GetAllListAsync(x => x.RefType == refType);
        }
        public async Task<List<Attachment>> GetByRefIdAsync(string refId)
        {
            return await _repository.GetAllListAsync(x => x.RefId == refId);
        }

        private static IEnumerable<MediaType> AcceptedTypesFor(AttachmentRefType refType)
        {
            switch (refType)
            {
                case AttachmentRefType.Profile:
                    return AllAcceptedTypes;
                case AttachmentRefType.TeacherProfile:
                    return ImagesAcceptedTypes;
                case AttachmentRefType.Advertisiment:
                    return ImagesAcceptedTypes;
                case AttachmentRefType.Subject:
                    return ImagesAcceptedTypes;
                case AttachmentRefType.LessonSessionThumbnail:
                    return ImagesAcceptedTypes;
                case AttachmentRefType.LessonSessionVideo:
                    return VideosAcceptedTypes;
                case AttachmentRefType.LessonSessionOther:
                    return AllAcceptedTypes;
            }

            return new MediaType[] { };
        }



        public Task<bool> CreateAttachments(List<Attachment> attachments)
        {
#pragma warning disable CS0168 // Variable is declared but never used
            try
            {
                foreach (var attachment in attachments)
                {
                    _repository.InsertAsync(attachment);
                }
                return Task.FromResult(true);
            }
            catch (Exception e)
            {
                throw new UserFriendlyException(L("ErrorOnInsertingAttachments"));
            }
#pragma warning restore CS0168 // Variable is declared but never used
        }

        public async Task<Attachment> CreateAttachment(Attachment attachment)
        {
#pragma warning disable CS0168 // Variable is declared but never used
            try
            {
                var attachmentResult = await _repository.InsertAsync(attachment);
                await CurrentUnitOfWork.SaveChangesAsync();
                return attachmentResult;
            }
            catch (Exception e)
            {
                throw new UserFriendlyException(L("ErrorOnInsertingAttachment"));
            }
#pragma warning restore CS0168 // Variable is declared but never used
        }

        public async Task<Attachment> GetElementByRefAsync(string refId, AttachmentRefType refType)
        {
            return await _repository.FirstOrDefaultAsync(x => x.RefId == refId && x.RefType == refType);

        }

        private static readonly MediaType[] AllAcceptedTypes =
            { MediaType.Image, MediaType.Pdf, MediaType.Video, MediaType.Audio, MediaType.Other };

        private static readonly MediaType[] ImagesAcceptedTypes =
            { MediaType.Image };

        private static readonly MediaType[] VideosAcceptedTypes =
            { MediaType.Video };

        public async Task CreateOrUpdateAttachmentAsync(string partnerId, string relativePath, string name)
        {
            
                var attachment = new Attachment
                {
                    FileName = name,
                    Type = MediaType.Image,
                    StorageKey = relativePath,
                    RefId = partnerId,
                    RefType = AttachmentRefType.Profile
                };

                await _repository.InsertAsync(attachment);
            
        }

        public async Task CopyNewAttachmentForCompany(long attachmentId, string companyId, AttachmentRefType refType)
        {
            var attachment = await GetAndCheckAsync(attachmentId, refType);
            var attachmentToInsert = new Attachment()
            {
                RefType = refType,
                StorageKey = attachment.StorageKey,
                Type = attachment.Type,
                RefId = companyId,
                LowResolutionPhotoRelativePath = attachment.LowResolutionPhotoRelativePath
            };
            await _repository.InsertAsync(attachmentToInsert);
            await UnitOfWorkManager.Current.SaveChangesAsync();
        }

       
        public async Task UpdteAllRefIdAsync(string companyId, List<long> attachmentIds)
        {
            await _repository.GetAll()
                .Where(x => attachmentIds.Contains(x.Id))
             .ExecuteUpdateAsync(s => s.SetProperty(x => x.RefId, companyId));
            await UnitOfWorkManager.Current.SaveChangesAsync();
        }
    }
}
