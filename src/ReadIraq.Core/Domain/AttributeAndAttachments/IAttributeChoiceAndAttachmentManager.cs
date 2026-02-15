using Abp.Domain.Services;
using ReadIraq.Domain.RequestForQuotations.Dto;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ReadIraq.Domain.AttributeAndAttachments
{
    public interface IAttributeChoiceAndAttachmentManager : IDomainService
    {
        Task<List<AttributeChoiceAndAttachmentDetailsDto>> GetAttributeChoiceAndAttachmentDetailsAsyncByRequestId(long requestId);
    }
}
