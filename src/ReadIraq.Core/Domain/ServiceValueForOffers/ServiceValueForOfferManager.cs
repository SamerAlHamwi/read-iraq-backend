using Abp.Domain.Repositories;
using Abp.Domain.Services;
using Microsoft.EntityFrameworkCore;
using ReadIraq.Domain.services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ReadIraq.Domain.ServiceValueForOffers
{
    public class ServiceValueForOfferManager : DomainService, IServiceValueForOfferManager
    {
        private readonly IRepository<ServiceValueForOffer, Guid> _serviceValueForOfferRepository;
        private readonly IServiceManager _serviceManager;

        public ServiceValueForOfferManager(IRepository<ServiceValueForOffer, Guid> serviceValueForOfferRepository, IServiceManager serviceManager)
        {
            _serviceValueForOfferRepository = serviceValueForOfferRepository;
            _serviceManager = serviceManager;
        }


        public async Task<bool> CheckIfServiceBelongsToOffer(int servicetId)
        {
            return await _serviceValueForOfferRepository.GetAll().Where(x => x.ServiceId == servicetId).AnyAsync();

        }

        public async Task HardDeleteServiceValuesForOffer(List<ServiceValueForOffer> serviceValuesforOffer)
        {
            try
            {
                foreach (var item in serviceValuesforOffer)
                {

                    await _serviceValueForOfferRepository.HardDeleteAsync(item);

                }
                await UnitOfWorkManager.Current.SaveChangesAsync();

            }
            catch (Exception ex) { throw; }
        }


        public async Task<bool> CheckIfSubServiceBelongsToOffer(int subServiceId)
        {
            return await _serviceValueForOfferRepository.GetAll().Where(x => x.SubServiceId == subServiceId).AnyAsync();
        }


        public async Task<bool> CheckIfToolBelongsToOffer(int toolId)
        {
            return await _serviceValueForOfferRepository.GetAll().Where(x => x.ToolId == toolId).AnyAsync();

        }



    }
}
