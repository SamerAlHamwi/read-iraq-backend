using Abp.Domain.Services;
using ReadIraq.Domain.services.Dto;
using ReadIraq.Domain.Services.Dto;
using ReadIraq.Domain.ServiceValueForOffers;
using ReadIraq.Domain.ServiceValues.Dto;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ReadIraq.Domain.services
{
    public interface IServiceManager : IDomainService
    {
        // Task<bool> CheckServicesIsCorrect(List<ServiceValuesDto> serviceValues);
        //public Task<ServiceDetailsDto> GetServiceByRequestForQuotationIdAsync(long requestId);
        Task<bool> CheckServiceIfExict(List<ServiceTranslationDto> translations);
        Task<ServiceDetailsDto> GetEntityByIdAsync(int id);
        Task HardDeleteForEntityTranslation(List<ServiceTranslation> translations);
        Task<List<ServiceDetailsDto>> GetFullServicesForUserOrCompanyOrOffer(List<int> servicesIds, List<int> subservicesIds, List<int> toolIdsForSubServices);
        Task<List<ServiceDto>> GetAllServicesDtosAsync();
        Task<bool> CheckServicesIsCorrect(List<ServiceValuesDto> serviceValues);
        Task<List<ServiceDetailsDto>> GetFullServicesForOffer(List<ServiceValueForOffer> servicesValues);

    }
}
