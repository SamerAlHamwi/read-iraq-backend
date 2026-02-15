using Abp.Domain.Repositories;
using Abp.Domain.Services;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using ReadIraq.Domain.RequestForQuotationContacts.Dto;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ReadIraq.Domain.RequestForQuotationContacts
{
    public class RequestForQuotationContactManager : DomainService, IRequestForQuotationContactManager
    {
        private readonly IRepository<RequestForQuotationContact, long> _reqForQuotationContactRepository;
        private readonly IMapper _mapper;
        public RequestForQuotationContactManager(IRepository<RequestForQuotationContact, long> reqForQuotationContactRepository, IMapper mapper)
        {
            _reqForQuotationContactRepository = reqForQuotationContactRepository;
            _mapper = mapper;
        }

        public async Task<List<RequestForQuotationContactDto>> GetAllContactsByRequestForQuotationId(long requestId)
        {
            var contacts = await _reqForQuotationContactRepository.GetAll().Where(x => x.RequestForQuotationId == requestId).ToListAsync();
            var contactsDto = new List<RequestForQuotationContactDto>();
            contactsDto = _mapper.Map<List<RequestForQuotationContact>, List<RequestForQuotationContactDto>>(contacts);
            return contactsDto;
        }
    }
}
