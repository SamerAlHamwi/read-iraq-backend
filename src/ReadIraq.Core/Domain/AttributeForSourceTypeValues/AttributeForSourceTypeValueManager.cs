using Abp.Domain.Repositories;
using Abp.Domain.Services;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using ReadIraq.Domain.AttributeForSourceTypeValues.Dto;
using ReadIraq.Domain.AttributeForSourcTypeValues;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ReadIraq.Domain.AttributeForSourceTypeValues
{
    public class AttributeForSourceTypeValueManager : DomainService, IAttributeForSourceTypeValueManager
    {
        private readonly IRepository<AttributeForSourceTypeValue> _attributeForSourceTypeValueRepository;
        private readonly IMapper _mapper;
        public AttributeForSourceTypeValueManager(IRepository<AttributeForSourceTypeValue> attributeForSourceTypeValueRepository, IMapper mapper)
        {
            _attributeForSourceTypeValueRepository = attributeForSourceTypeValueRepository;
            _mapper = mapper;
        }

        public async Task<List<AttributeForSourceTypeValueDto>> GetAllAttributeForSourceTypeValuesByRequestForQuotationId(long quotationId)
        {
            List<AttributeForSourceTypeValue> values = await _attributeForSourceTypeValueRepository.GetAll().AsNoTracking()
                .Include(x => x.AttributeForSourcType).ThenInclude(x => x.Translations).AsNoTracking()
                .Include(x => x.AttributeChoice).ThenInclude(x => x.Translations).AsNoTracking()
                .Where(x => x.RequestForQuotationId == quotationId).ToListAsync();
            var valuesDto = new List<AttributeForSourceTypeValueDto>();
            valuesDto = _mapper.Map<List<AttributeForSourceTypeValue>, List<AttributeForSourceTypeValueDto>>(values);
            return valuesDto;
        }



        public async Task HardDeleteEntity(List<AttributeForSourceTypeValue> attributeForSourceTypeValues)
        {
            try
            {
                foreach (var item in attributeForSourceTypeValues)
                {

                    await _attributeForSourceTypeValueRepository.HardDeleteAsync(item);
                }
            }
            catch (Exception ex) { throw; }
        }

    }
}
