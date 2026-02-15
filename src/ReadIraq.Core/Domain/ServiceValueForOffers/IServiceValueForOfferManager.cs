using Abp.Domain.Services;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ReadIraq.Domain.ServiceValueForOffers
{
    public interface IServiceValueForOfferManager : IDomainService
    {
        Task HardDeleteServiceValuesForOffer(List<ServiceValueForOffer> serviceValuesforOffer);
        Task<bool> CheckIfServiceBelongsToOffer(int servicetId);
        Task<bool> CheckIfSubServiceBelongsToOffer(int subServiceId);
        Task<bool> CheckIfToolBelongsToOffer(int toolId);


    }
}
