using Abp.Domain.Services;
using ReadIraq.Domain.AttributeForSourceTypeValues.Dto;
using ReadIraq.Domain.AttributeForSourcTypeValues;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ReadIraq.Domain.AttributeForSourceTypeValues
{
    public interface IAttributeForSourceTypeValueManager : IDomainService
    {
        Task<List<AttributeForSourceTypeValueDto>> GetAllAttributeForSourceTypeValuesByRequestForQuotationId(long quotationId);
        Task HardDeleteEntity(List<AttributeForSourceTypeValue> attributeForSourceTypeValues);
    }
}
