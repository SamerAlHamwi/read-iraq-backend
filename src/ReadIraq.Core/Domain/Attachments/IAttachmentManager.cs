using Abp.Domain.Services;
using System.Collections.Generic;
using System.Threading.Tasks;
using static ReadIraq.Enums.Enum;

namespace ReadIraq.Domain.Attachments
{
    public interface IAttachmentManager : IDomainService
    {
        /// <summary>
        /// Get Attachment by Id.
        /// Throws exception if there is error.
        /// </summary>
        /// <param name="id">Id of the Attachment</param>
        /// <returns>Attachment Entity</returns>
        Task<Attachment> GetByIdAsync(long id);
        /// <summary>
        /// Get List Of Attachments by Ids.
        /// </summary>
        /// <param name="id">Id of the Attachment</param>
        /// <returns>Attachment Entity</returns>
        Task<List<Attachment>> GetListByIdsAsync(List<long> ids);

        /// <summary>
        /// Check if attachment id exists, and has a specific related entity type.
        /// Throws exception if there is error.
        /// </summary>
        /// <param name="id">Id of the Attachment</param>
        /// <param name="refType">Type of related Entity</param>
        /// <returns>Attachment Entity</returns>
        Task<Attachment> GetAndCheckAsync(long id, AttachmentRefType refType, bool IsForDraft = false);

        /// <summary>
        /// Get navigable Url of an attachment,
        /// using attachment relative path and a configured base uri.
        /// </summary>
        /// <param name="attachment">Attachment Entity</param>
        /// <returns>Url for the attachment</returns>
        string GetUrl(Attachment attachment);
        string GetLowResolutionPhotoUrl(Attachment attachment);

        /// <summary>
        /// Update RefId to passed refId, so attachment is now related to entity.
        /// </summary>
        /// <param name="attachment">Attachment Entity</param>
        /// <param name="refId">Id of related entity</param>
        Task UpdateRefIdAsync(Attachment attachment, long refId, bool IsForDraft = false, bool IsForRequest = true);

        /// <summary>
        /// Check if attachment id exists, and has a specific related entity type.
        /// Update RefId of the attachment to refId.
        /// </summary>
        /// <param name="id">Id of the Attachment</param>
        /// <param name="refType">Type of related Entity</param>
        /// <param name="refId">Id of related entity</param>
        /// <returns>The Attachment after modification</returns>
        Task<Attachment> CheckAndUpdateRefIdAsync(long id, AttachmentRefType refType, long refId, bool IsForDraft = false, bool IsForRequest = true);

        /// <summary>
        /// Update RefId to be null, so it can be removed by background Service.
        /// </summary>
        /// <param name="attachment">Attachment Entity</param>
        Task DeleteRefIdAsync(Attachment attachment);

        /// <summary>
        /// Update RefId of all attachments of refId and refType to be null,
        /// so they can be removed by background Service.
        /// </summary>
        /// <param name="refId">Id of related entity</param>
        /// <param name="refType">Type of related Entity</param>
        Task DeleteAllRefIdAsync(long refId, AttachmentRefType refType);

        /// <summary>
        /// Checks if file type is compatible with related entity type.
        /// </summary>
        /// <param name="refType">Type of related Entity</param>
        /// <param name="fileType">Type of Attachment (file type)</param>
        void CheckAttachmentRefType(AttachmentRefType refType, AttachmentType fileType);

        /// <summary>
        /// Get list of attachments that are related to specific Entity Id and Type.
        /// </summary>
        /// <param name="refId">Id of related entity</param>
        /// <param name="refType">Type of related Entity</param>
        /// <returns>List of Attachment Entities</returns>
        Task<List<Attachment>> GetByRefAsync(long refId, AttachmentRefType refType);
        Task<List<Attachment>> GetByRefAsync(List<long> refIds, AttachmentRefType refType);
        Task<List<Attachment>> GetByRefTypeAsync(AttachmentRefType refType);
        Task<List<Attachment>> GetByRefIdAsync(long refId);



        Task<Attachment> GetElementByRefAsync(long refId, AttachmentRefType refType);


        /// <summary>
        /// CreateAttachments
        /// </summary>
        /// <param name="attachments"></param>
        Task<bool> CreateAttachments(List<Attachment> attachments);

        /// <summary>
        /// CreateAttachment
        /// </summary>
        /// <param name="attachment"></param>
        /// <returns></returns>
        Task<Attachment> CreateAttachment(Attachment attachment);
        Task CopyNewAttachmentForCompany(long attachmentId, int companyId, AttachmentRefType refType);
        Task DeleteAllRefIdForCompanyAsync(int companyId);
    }
}
