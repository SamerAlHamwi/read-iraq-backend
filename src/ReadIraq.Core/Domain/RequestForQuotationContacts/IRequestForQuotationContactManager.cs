using Abp.Domain.Services;
using ReadIraq.Domain.RequestForQuotationContacts.Dto;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ReadIraq.Domain.RequestForQuotationContacts
{
    public interface IRequestForQuotationContactManager : IDomainService
    {
        Task<List<RequestForQuotationContactDto>> GetAllContactsByRequestForQuotationId(long requestId);
    }

}
