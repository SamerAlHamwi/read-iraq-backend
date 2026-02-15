using Abp.Domain.Repositories;
using Abp.Domain.Services;
using Abp.EntityFrameworkCore.Repositories;
using Microsoft.EntityFrameworkCore;
using ReadIraq.Domain.services;
using ReadIraq.Domain.services.Dto;
using ReadIraq.Domain.ServiceValues.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ReadIraq.Domain.ServiceValuesForDrafts
{
    public class ServiceValuesForDraftManager : DomainService, IServiceValuesForDraftManager
    {
        private readonly IRepository<ServiceValuesForDraft> _serviceValuesForDraftRepository;
        private readonly IServiceManager _serviceManager;

        public ServiceValuesForDraftManager(IRepository<ServiceValuesForDraft> serviceValuesForDraftRepository, IServiceManager serviceManager)
        {
            _serviceValuesForDraftRepository = serviceValuesForDraftRepository;
            _serviceManager = serviceManager;
        }


        public async Task<bool> InsertServiceValuesForDraft(List<ServiceValuesForDraftDto> serviceValues, int draftId)
        {

            List<ServiceValuesForDraft> servicesValuesForInsert = new List<ServiceValuesForDraft>();

            foreach (var serviceValue in serviceValues)
            {
                servicesValuesForInsert.Add(new ServiceValuesForDraft
                {
                    DraftId = draftId,
                    ServiceId = serviceValue.ServiceId,
                    SubServiceId = serviceValue.SubServiceId,
                    ToolId = serviceValue.ToolId
                });
            }
            await _serviceValuesForDraftRepository.InsertRangeAsync(servicesValuesForInsert);
            await UnitOfWorkManager.Current.SaveChangesAsync();
            return true;
        }
        public async Task HardDeleteServiceValues(List<ServiceValuesForDraft> serviceValues)
        {
            try
            {
                foreach (var item in serviceValues)
                {

                    await _serviceValuesForDraftRepository.HardDeleteAsync(item);

                }
                await UnitOfWorkManager.Current.SaveChangesAsync();

            }
            catch (Exception ex) { throw; }
        }

        public async Task DeleteServiceValuesByDraftsIds(List<int> draftsIds)
        {
            var sevicesValues = await _serviceValuesForDraftRepository.GetAll().Where(x => draftsIds.Contains(x.DraftId)).ToListAsync();
            foreach (var item in sevicesValues)
            {
                await _serviceValuesForDraftRepository.DeleteAsync(item);
            }

        }

        public async Task DeleteServiceValuesByDraftId(int draftId)
        {
            var sevicesValues = await _serviceValuesForDraftRepository.GetAll().Where(x => x.DraftId == draftId).FirstOrDefaultAsync();

            await _serviceValuesForDraftRepository.DeleteAsync(sevicesValues);
        }

        public async Task<List<ServiceDetailsDto>> GetFullServicesByDraftIdAsync(int id)
        {
            var services = await _serviceValuesForDraftRepository.GetAll().AsNoTracking().Where(x => x.DraftId == id).ToListAsync();

            var servicesIds = services
                .Where(x => x.ServiceId.HasValue)
                .Select(x => x.ServiceId.Value)
                .Distinct()
                .ToList();
            var subservicesIds = services
                .Where(x => x.SubServiceId.HasValue)
                .Select(x => x.SubServiceId.Value)
                .Distinct()
                .ToList();
            var toolIdsForSubServices = services.
                 Where(x => x.ToolId.HasValue)
                .Select(x => x.ToolId.Value)
                .Distinct()
                .ToList();
            return await _serviceManager.GetFullServicesForUserOrCompanyOrOffer(servicesIds, subservicesIds, toolIdsForSubServices);
        }
    }
}
