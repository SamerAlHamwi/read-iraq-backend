using Abp.Application.Services.Dto;

namespace ReadIraq.Attachments.Dto
{
    /// <summary>
    /// PagedAttachmentResultRequestDto
    /// </summary>
    public class PagedAttachmentResultRequestDto : PagedAndSortedResultRequestDto
    {
        /// <summary>
        /// Related entity Id
        /// </summary>
        public long RefId { get; set; }

        /// <summary>
        /// Identification = 1,
        /// Referral = 2,
        /// AnatomyReport = 3,
        /// Analysis = 4,
        /// RadialImage = 5,
        /// Message = 6,
        /// Other = 7
        /// </summary>
        public byte RefType { get; set; }
    }
}