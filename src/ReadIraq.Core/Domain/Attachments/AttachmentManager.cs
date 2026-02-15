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
            if (IsForDraft)
            {
                if (attachment.RefType != AttachmentRefType.Draft && attachment.RefType != AttachmentRefType.RequestForQuotation)
                    throw new UserFriendlyException(L("InvalidAttachmentRefType"),
                     $"Id: {id}, RefType: {attachment.RefType} and should be {(byte)refType}");
                return attachment;
            }
            if (attachment.RefType != refType)
                throw new UserFriendlyException(L("InvalidAttachmentRefType"),
                    $"Id: {id}, RefType: {attachment.RefType} and should be {(byte)refType}");

            return attachment;
        }

        public string GetUrl(Attachment attachment)
        {
            var baseUri = new Uri(_appBaseUrl);
            return (new Uri(baseUri, attachment.RelativePath)).AbsoluteUri;
        }
        public string GetLowResolutionPhotoUrl(Attachment attachment)
        {
            var baseUri = new Uri(_appBaseUrl);
            return (new Uri(baseUri, attachment.LowResolutionPhotoRelativePath)).AbsoluteUri;
        }
        public async Task UpdateRefIdAsync(Attachment attachment, long refId, bool IsForDraft = false, bool IsForRequest = true)
        {
            if (attachment.RefId != null && !IsForDraft)
                throw new UserFriendlyException(L("AttachmentAlreadyRelatedToEntity"),
                    $"Id: {attachment.Id}, RefType: {attachment.RefType}");
            if (IsForDraft && !IsForRequest)
                attachment.RefType = AttachmentRefType.Draft;
            if (IsForDraft && IsForRequest)
                attachment.RefType = AttachmentRefType.RequestForQuotation;
            attachment.RefId = refId;
            await _repository.UpdateAsync(attachment);
        }

        public async Task<Attachment> CheckAndUpdateRefIdAsync(long id, AttachmentRefType refType, long refId, bool IsForDraft = false, bool IsForRequest = true)
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

        public async Task DeleteAllRefIdAsync(long refId, AttachmentRefType refType)
        {
            foreach (var attachment in await GetByRefAsync(refId, refType))
            {
                attachment.RefId = null;
                await _repository.UpdateAsync(attachment);
            }
        }

        public void CheckAttachmentRefType(AttachmentRefType refType, AttachmentType fileType)
        {
            if (!AcceptedTypesFor(refType).Contains(fileType))
                throw new UserFriendlyException(L("FileTypeIncompatibleWithRefType"),
                    $"Type:{fileType.ToString()}, RefType:{refType.ToString()}");
        }

        public async Task<List<Attachment>> GetByRefAsync(long refId, AttachmentRefType refType)
        {
            var refTypeString = ((byte)refType).ToString();
            return await _repository.GetAllListAsync(x => x.RefId == refId && x.RefType == refType);
        }
        public async Task<List<Attachment>> GetByRefAsync(List<long> refIds, AttachmentRefType refType)
        {
            var refTypeString = ((byte)refType).ToString();
            return await _repository.GetAllListAsync(x => refIds.Contains(x.Id) && x.RefType == refType);
        }
        public async Task<List<Attachment>> GetByRefTypeAsync(AttachmentRefType refType)
        {
            var refTypeString = ((byte)refType).ToString();
            return await _repository.GetAllListAsync(x => x.RefType == refType);
        }
        public async Task<List<Attachment>> GetByRefIdAsync(long refId)
        {
            return await _repository.GetAllListAsync(x => x.RefId == refId);
        }

        private static IEnumerable<AttachmentType> AcceptedTypesFor(AttachmentRefType refType)
        {
            switch (refType)
            {
                case AttachmentRefType.Profile:
                    return AllAcceptedTypes;
                case AttachmentRefType.Advertisiment:
                    return ImagesAcceptedTypes;
                case AttachmentRefType.QR:
                    return ImagesAcceptedTypes;
                case AttachmentRefType.RequestForQuotation:
                    return ImagesAcceptedTypes;
                case AttachmentRefType.SourceTypeIcon:
                    return ImagesAcceptedTypes;
                case AttachmentRefType.ContactUs:
                    return ImagesAcceptedTypes;
                case AttachmentRefType.Service:
                    return ImagesAcceptedTypes;
                case AttachmentRefType.CompanyProfile:
                    return ImagesAcceptedTypes;
                case AttachmentRefType.CompanyOwnerIdentity:
                    return AllAcceptedTypes;
                case AttachmentRefType.CompanyCommercialRegister:
                    return AllAcceptedTypes;
                case AttachmentRefType.AdditionalAttachment:
                    return AllAcceptedTypes;
                case AttachmentRefType.SubService:
                    return ImagesAcceptedTypes;
                case AttachmentRefType.Tool:
                    return ImagesAcceptedTypes;
                case AttachmentRefType.FinishedRequestByCompany:
                    return ImagesAcceptedTypes;
            }

            return new AttachmentType[] { };
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

        public async Task<Attachment> GetElementByRefAsync(long refId, AttachmentRefType refType)
        {
            var refTypeString = ((byte)refType).ToString();
            return await _repository.FirstOrDefaultAsync(x => x.RefId == refId && x.RefType == refType);

        }

        private static readonly AttachmentType[] AllAcceptedTypes =
            { AttachmentType.JPEG, AttachmentType.JPG, AttachmentType.PDF, AttachmentType.PNG, AttachmentType.WORD };

        private static readonly AttachmentType[] ImagesAcceptedTypes =
            { AttachmentType.JPEG, AttachmentType.JPG, AttachmentType.PNG };

        public async Task CreateOrUpdateAttachmentAsync(int partnerId, string relativePath, string name)
        {
            var oldAttachment = await _repository.FirstOrDefaultAsync(x =>
                    x.RefId == partnerId && x.RefType == AttachmentRefType.QR);
            if (oldAttachment != null)
            {
                oldAttachment.Name = name;
                oldAttachment.RelativePath = relativePath;
                await _repository.UpdateAsync(oldAttachment);
            }
            else
            {
                var attachment = new Attachment
                {
                    Name = name,
                    Type = AttachmentType.PNG,
                    RelativePath = relativePath,
                    RefId = partnerId,
                    RefType = AttachmentRefType.QR
                };

                await _repository.InsertAsync(attachment);
            }
        }

        public async Task CopyNewAttachmentForCompany(long attachmentId, int companyId, AttachmentRefType refType)
        {
            var attachment = await GetAndCheckAsync(attachmentId, refType);
            var attachmentToInsert = new Attachment()
            {
                RefType = refType,
                RelativePath = attachment.RelativePath,
                Type = attachment.Type,
                RefId = companyId,
                LowResolutionPhotoRelativePath = attachment.LowResolutionPhotoRelativePath
            };
            await _repository.InsertAsync(attachmentToInsert);
            await UnitOfWorkManager.Current.SaveChangesAsync();
        }

        public async Task DeleteAllRefIdForCompanyAsync(int companyId)
        {
            await _repository.GetAll()
                .Where(x => x.RefId.HasValue && x.RefId == companyId &&
                (x.RefType == AttachmentRefType.CompanyProfile ||
                x.RefType == AttachmentRefType.CompanyOwnerIdentity ||
                x.RefType == AttachmentRefType.CompanyCommercialRegister ||
                x.RefType == AttachmentRefType.AdditionalAttachment))
             .ExecuteUpdateAsync(s => s.SetProperty(x => x.IsDeleted, true)
             .SetProperty(x => x.RefId, 0));
            await UnitOfWorkManager.Current.SaveChangesAsync();
        }
        public async Task UpdteAllRefIdAsync(int companyId, List<long> attachmentIds)
        {
            await _repository.GetAll()
                .Where(x => attachmentIds.Contains(x.Id))
             .ExecuteUpdateAsync(s => s.SetProperty(x => x.RefId, companyId));
            await UnitOfWorkManager.Current.SaveChangesAsync();
        }
    }
}
