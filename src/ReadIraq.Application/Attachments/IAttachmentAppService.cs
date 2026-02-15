using Abp.Application.Services;
using Abp.Application.Services.Dto;
using ReadIraq.Attachments.Dto;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ReadIraq.Attachments
{
    /// <summary>
    /// IAttachmentAppService
    /// </summary>
    public interface IAttachmentAppService : IApplicationService
    {
        /// <summary>
        /// Get All Attachments
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        Task<ListResultDto<AttachmentDto>> GetAllAsync(PagedAttachmentResultRequestDto input);
        /// <summary>
        /// Get Attachment
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        Task<AttachmentDto> GetAsync(EntityDto input);
        /// <summary>
        /// Delete Attachment
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        Task DeleteAsync(EntityDto input);
        /// <summary>
        /// Upload Attachment
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        Task<AttachmentDto> UploadAsync(UploadAttachmentInputDto input);
        Task<List<AttachmentDto>> UploadMultiAttachmentAsync(UploadMultiAttachmentInputDto input);




    }
}
