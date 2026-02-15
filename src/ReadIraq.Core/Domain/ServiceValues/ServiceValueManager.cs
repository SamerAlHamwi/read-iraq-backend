using Abp.Domain.Repositories;
using Abp.Domain.Services;
using Abp.EntityFrameworkCore.Repositories;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using ReadIraq.Domain.Attachments;
using ReadIraq.Domain.Companies;
using ReadIraq.Domain.Companies.Dto;
using ReadIraq.Domain.CompanyBranches;
using ReadIraq.Domain.RequestForQuotations;
using ReadIraq.Domain.RequestForQuotations.Dto;
using ReadIraq.Domain.services;
using ReadIraq.Domain.services.Dto;
using ReadIraq.Domain.ServiceValues.Dto;
using ReadIraq.Domain.SubServices;
using ReadIraq.Domain.Toolss;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading.Tasks;
using static ReadIraq.Enums.Enum;

namespace ReadIraq.Domain.ServiceValues
{
    public class ServiceValueManager : DomainService, IServiceValueManager
    {
        private readonly IRepository<ServiceValue, long> _serviceValueRepository;
        private readonly ServiceManager _serviceManager;
        private readonly AttachmentManager _attachmentManager;
        private readonly IMapper _mapper;
        private readonly CompanyManager _companyManager;
        private readonly IRepository<Tool> _toolRepository;
        private readonly IRepository<SubService> _subServiceRepository;
        private readonly IRepository<RequestForQuotation, long> _requestForQuotationRepository;
        private readonly ToolManger _toolManger;
        private readonly ICompanyBranchManager _companyBranchManager;
        public ServiceValueManager(IRepository<ServiceValue, long> serviceValueRepository,
            ServiceManager serviceManager,
            AttachmentManager attachmentManager,
            IMapper mapper, CompanyManager companyManager,
            IRepository<Tool> toolRepository,
            IRepository<SubService> subServiceRepository,
            ToolManger toolManger, ICompanyBranchManager companyBranchManager,
            IRepository<RequestForQuotation, long> requestForQuotationRepository)
        {
            _serviceValueRepository = serviceValueRepository;
            _serviceManager = serviceManager;
            _attachmentManager = attachmentManager;
            _mapper = mapper;
            _companyManager = companyManager;
            _toolRepository = toolRepository;
            _subServiceRepository = subServiceRepository;
            _toolManger = toolManger;
            _companyBranchManager = companyBranchManager;
            _requestForQuotationRepository = requestForQuotationRepository;
        }

        public async Task<bool> InsertServiceValuesForCompany(List<ServiceValuesDto> serviceValues, int companyId)
        {
            // await _serviceManager.CheckServicesIsCorrect(serviceValues);
            //  await _toolManger.CheckIfToolsIsCorrect(serviceValues.SelectMany(x => x.ToolIds).ToList());

            //var branchCompanyIds = await _companyManager.GetCompanyBranchesByCompanyId(companyId);
            var serviceValuesForInsert = ObjectMapper.Map(serviceValues, new List<ServiceValue>());
            foreach (var item in serviceValuesForInsert)
            {
                item.ServiceValueType = ServiceValueType.ForCompany;
                item.CompanyId = companyId;
                //servicesValues.Add(item);
            }

            await _serviceValueRepository.InsertRangeAsync(serviceValuesForInsert);
            return true;
        }
        public async Task<bool> InsertServiceValuesForCompanyBranch(List<ServiceValuesDto> serviceValues, int companyId, int companyBracnhId)
        {

            var serviceValuesForInsert = ObjectMapper.Map(serviceValues, new List<ServiceValue>());
            foreach (ServiceValue item in serviceValuesForInsert)
            {
                item.ServiceValueType = ServiceValueType.ForCompanyBranch;
                item.CompanyBranchId = companyBracnhId;
                if (companyId > 0)
                    item.CompanyId = companyId;
            }

            await _serviceValueRepository.InsertRangeAsync(serviceValuesForInsert);
            return true;
        }

        public async Task<bool> InsertServiceValuesForUser(List<ServiceValuesDto> serviceValues, long requestForQuotationId)
        {
            List<ServiceValue> servicesValuesForInsert = new List<ServiceValue>();
            foreach (var serviceValue in serviceValues)
            {
                servicesValuesForInsert.Add(new ServiceValue
                {
                    RequestForQuotationId = requestForQuotationId,
                    ServiceValueType = ServiceValueType.ForUser,
                    ServiceId = serviceValue.ServiceId,
                    SubServiceId = serviceValue.SubServiceId,
                    ToolId = serviceValue.ToolId
                });
            }
            await _serviceValueRepository.InsertRangeAsync(servicesValuesForInsert);
            await UnitOfWorkManager.Current.SaveChangesAsync();
            return true;
        }

        public async Task DeleteServiceValuesForCompanyBranch(int branchId)
        {
            var serviceValues = await _serviceValueRepository.GetAllListAsync(sv => sv.CompanyBranchId == branchId && sv.ServiceValueType == ServiceValueType.ForCompanyBranch);
            foreach (var serviceValue in serviceValues)
            {
                await _serviceValueRepository.DeleteAsync(serviceValue);
            }

        }


        public async Task<CompanyAndCompanyBranchIdsDto> FilterCompatibleCompaniesWithRequestByServices(long requestId, bool forCompany = true, bool getCompanyonlyAcceptPossibleRequest = false)
        {
            var cityIds = new List<int>();
            if (forCompany)
                cityIds = await _companyManager.GetCompanyIdsThatContainsSameCitiesInRequest(requestId);
            else
                cityIds = await _companyBranchManager.GetCompanyBranchIdsThatContainsSameCitiesInRequest(requestId);
            if (forCompany && getCompanyonlyAcceptPossibleRequest)
                cityIds = await _companyManager.FilterCompanyIdsThatOnlyAcceptPossibleRequest(cityIds);
            else if (!forCompany && getCompanyonlyAcceptPossibleRequest)
                cityIds = await _companyBranchManager.FilterCompanyBranchIdsThatOnlyAcceptPossibleRequest(cityIds);
            var serviceIdsForRequest = await GetServiceIdsForCompnayOrRequestAsync(requestId, false);
            return await GetCompanyOrCompanyBracnhIdsCompatibleWithRequest(serviceIdsForRequest, cityIds, forCompany);
        }

        public async Task<CompanyAndCompanyBranchIdsDto> GetCompanyOrCompanyBracnhIdsCompatibleWithRequest(List<int> serviceIds, List<int> cityIds, bool forComnpany = true)
        {
            var distinctCompanyIds = new List<int>();
            var distinctCompanyBranchIds = new List<int>();
            if (forComnpany)
            {
                distinctCompanyIds = await _serviceValueRepository.GetAll()
                                             .Where(x => x.ServiceValueType == ServiceValueType.ForCompany && serviceIds.Contains(x.ServiceId.Value))
                                             .GroupBy(x => x.CompanyId.Value) // Group by CompanyId
                                             .OrderByDescending(group => group.Count()) // Order by the count of each group in descending order
                                             .Select(group => group.Key) // Select the CompanyId
                                                                         //.Distinct() // Ensure distinct CompanyIds
                                             .ToListAsync();
                return new CompanyAndCompanyBranchIdsDto { CompanyIds = distinctCompanyIds.Where(x => cityIds.Contains(x)).ToList() };
            }
            else
            {
                distinctCompanyBranchIds = await _serviceValueRepository.GetAll()
                                             .Where(x => x.ServiceValueType == ServiceValueType.ForCompanyBranch && serviceIds.Contains(x.ServiceId.Value))
                                             .GroupBy(x => x.CompanyBranchId.Value) // Group by CompanyBranchId
                                             .OrderByDescending(group => group.Count()) // Order by the count of each group in descending order
                                             .Select(group => group.Key) // Select the CompanyBranchId
                                                                         //.Distinct() // Ensure distinct CompanyBranchIds
                                             .ToListAsync();
                return new CompanyAndCompanyBranchIdsDto { CompanyBranchIds = distinctCompanyBranchIds.Where(x => cityIds.Contains(x)).ToList() };
            }
        }

        /// <summary>
        /// Return Full Services With Subs And Tools For Company OR Request , Default Value For Request 
        /// </summary>
        /// <param name="id"></param>
        /// <param name="forCompany"></param>
        /// <returns></returns>
        public async Task<List<ServiceDetailsDto>> GetFullServicesByCompanyOrRequestIdAsync(long id, bool forCompany = false, bool forCompanyBranch = false)
        {
            var services = new List<ServiceValue>();

            if (forCompany)
                services = await _serviceValueRepository.GetAll().AsNoTracking().Where(x => x.CompanyId == id).ToListAsync();
            else if (forCompanyBranch)
                services = await _serviceValueRepository.GetAll().AsNoTracking().Where(x => x.CompanyBranchId == id).ToListAsync();
            else
                services = await _serviceValueRepository.GetAll().AsNoTracking().Where(x => x.RequestForQuotationId == id).ToListAsync();

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

        public async Task<List<int>> GetServiceIdsForCompnayOrRequestAsync(long id, bool forCompany = false)
        {
            var services = new List<ServiceValue>();

            if (forCompany)
                services = await _serviceValueRepository.GetAll().AsNoTracking().Where(x => x.CompanyId == id).ToListAsync();
            else
                services = await _serviceValueRepository.GetAll().AsNoTracking().Where(x => x.RequestForQuotationId == id).ToListAsync();

            return services.Where(x => x.ServiceId.HasValue).Select(x => x.ServiceId.Value).Distinct().ToList();
        }

        public async Task<List<ServiceIdsAndRequestIdsDto>> GetServicesIdsAndRequestIdsAsync(List<long> requestIds)
        {
            List<ServiceValue> serviceValues = await _serviceValueRepository.GetAll().AsNoTracking().Where(x => x.RequestForQuotationId.HasValue && requestIds.Contains(x.RequestForQuotationId.Value))
               .ToListAsync();
            List<ServiceIdsAndRequestIdsDto> serviceIdsAndRequestIds = new List<ServiceIdsAndRequestIdsDto>();
            foreach (var id in requestIds)
            {
                serviceIdsAndRequestIds.Add(new ServiceIdsAndRequestIdsDto()
                {
                    RequestId = id,
                    ServiceIds = serviceValues.Where(x => x.RequestForQuotationId == id && x.ServiceId.HasValue).Select(x => x.ServiceId.Value).ToList(),
                });
            }
            return serviceIdsAndRequestIds;
        }

        public async Task<List<ServiceIdsAndCompanyIdsDto>> GetServicesIdsAndCompanyIdsAsync(List<int> companyIds)
        {
            var serviceValues = await _serviceValueRepository.GetAll().AsNoTracking().Where(x => x.CompanyId.HasValue && companyIds.Contains(x.CompanyId.Value))
               .ToListAsync();
            List<ServiceIdsAndCompanyIdsDto> serviceIdsAndCompanyIds = new List<ServiceIdsAndCompanyIdsDto>();
            foreach (var id in companyIds)
            {
                serviceIdsAndCompanyIds.Add(new ServiceIdsAndCompanyIdsDto()
                {
                    CompanyId = id,
                    ServiceIds = serviceValues.Where(x => x.CompanyId == id && x.ServiceId.HasValue).Select(x => x.ServiceId.Value).ToList(),
                });
            }
            return serviceIdsAndCompanyIds;
        }
        public async Task<List<ServiceIdsAndCompanyBranchIdsDto>> GetServicesIdsAndCompanyBranchIdsAsync(List<int> companyBranchIds)
        {
            var serviceValues = await _serviceValueRepository.GetAll().AsNoTracking().Where(x => x.CompanyBranchId.HasValue && companyBranchIds.Contains(x.CompanyBranchId.Value))
               .ToListAsync();
            List<ServiceIdsAndCompanyBranchIdsDto> serviceIdsAndCompanyBranchIds = new List<ServiceIdsAndCompanyBranchIdsDto>();
            foreach (var id in companyBranchIds)
            {
                serviceIdsAndCompanyBranchIds.Add(new ServiceIdsAndCompanyBranchIdsDto()
                {
                    CompanyBranchId = id,
                    ServiceIds = serviceValues.Where(x => x.CompanyBranchId == id && x.ServiceId.HasValue).Select(x => x.ServiceId.Value).ToList(),
                });
            }
            return serviceIdsAndCompanyBranchIds;
        }

        public async Task HardDeleteServiceValues(List<ServiceValue> serviceValues)
        {
            try
            {
                var ids = serviceValues.Select(x => x.Id).ToList();
                await _serviceValueRepository.GetAll().Where(x => ids.Contains(x.Id))
                    .ExecuteUpdateAsync(se => se.SetProperty(x => x.IsDeleted, true));
                //foreach (var item in serviceValues)
                //{

                //    await _serviceValueRepository.HardDeleteAsync(item);

                //}
                await UnitOfWorkManager.Current.SaveChangesAsync();

            }
            catch (Exception ex) { throw; }
        }
        public async Task DeleteServiceValuesForCompanyAsync(int companyId)
        {
            await _serviceValueRepository.GetAll().AsTracking().Where(x => x.CompanyId == companyId).ExecuteDeleteAsync();
            await UnitOfWorkManager.Current.SaveChangesAsync();

        }
        public async Task DeleteServiceValuesForCompanyBranchAsync(int companyBranchId)
        {
            await _serviceValueRepository.GetAll().AsTracking().Where(x => x.CompanyBranchId == companyBranchId).ExecuteDeleteAsync();
            await UnitOfWorkManager.Current.SaveChangesAsync();

        }

        public async Task<bool> CheckIfServiceBelongsToRequest(int servicetId)
        {
            return await _serviceValueRepository.GetAll().Where(x => x.ServiceId == servicetId && x.RequestForQuotationId.HasValue).AnyAsync();

        }
        public async Task<bool> CheckIfServiceBelongsToCompanyOrCompanyBranch(int servicetId)
        {
            return await _serviceValueRepository.GetAll().Where(x => x.ServiceId == servicetId && (x.CompanyBranchId.HasValue || x.CompanyId.HasValue)).AnyAsync();
        }

        public async Task<bool> CheckIfSubServiceBelongsToRequest(int subServicetId)
        {
            return await _serviceValueRepository.GetAll().Where(x => x.SubServiceId == subServicetId && x.RequestForQuotationId.HasValue).AnyAsync();
        }

        public async Task<bool> CheckIfSubServiceBelongsToCompanyOrCompanyBranch(int subServiceId)
        {
            return await _serviceValueRepository.GetAll().Where(x => x.SubServiceId == subServiceId && (x.CompanyBranchId.HasValue || x.CompanyId.HasValue)).AnyAsync();
        }

        public async Task<bool> CheckIfToolBelongsToRequest(int toolId)
        {
            return await _serviceValueRepository.GetAll().Where(x => x.ToolId == toolId && x.RequestForQuotationId.HasValue && x.IsDeleted == false).AnyAsync();

        }

        public async Task<bool> CheckIfToolBelongsToCompanyOrCompanyBranch(int toolId)
        {
            return await _serviceValueRepository.GetAll().Where(x => x.ToolId == toolId && (x.CompanyBranchId.HasValue || x.CompanyId.HasValue)).AnyAsync();
        }

        public async Task<List<ServiceStatisticsForRequestsDto>> GetServicesStatisticsForRequestsDto()
        {
            var result = await _serviceValueRepository.GetAll().Where(item => item.ServiceId.HasValue && item.RequestForQuotationId.HasValue && item.IsDeleted == false)
                 .GroupBy(item => item.ServiceId)
                 .Select(group => new ServiceStatisticsForRequestsDto
                 {
                     ServiceId = group.Key.Value,
                     RequestForQuotationCount = group.Select(item => item.RequestForQuotationId).Distinct().Count()
                 }).OrderByDescending(r => r.RequestForQuotationCount).ToListAsync();

            foreach (var item in result)
            {
                item.Service = _mapper.Map<LiteServiceDto>(await _serviceManager.GetLiteEntityAsyn(item.ServiceId));
                if (item.Service is null) result.Remove(item);

            }
            if (result is null) return new List<ServiceStatisticsForRequestsDto>();
            return result;

        }

        public async Task<List<ServiceValuesDto>> GetAllServiceValuesForCompany(int companyId)
        {
            return ObjectMapper.Map(await _serviceValueRepository.GetAll().Where(x => x.CompanyId.HasValue && x.CompanyId == companyId).ToListAsync(), new List<ServiceValuesDto>());
        }

        public async Task<int> GetCountOfServicesForRequest(long requestId)
        {
            return await _serviceValueRepository.GetAll()
                 .AsNoTrackingWithIdentityResolution()
                 .Where(x => x.RequestForQuotationId == requestId && x.ServiceValueType == ServiceValueType.ForUser && x.ServiceId.HasValue)
                 .GroupBy(x => x.ServiceId.Value)
                 .CountAsync();
        }

        public async Task<List<CompanyWithServiceCountDto>> GetCountOfServicesForCompanies(List<int> companyIds)
        {
            return await _serviceValueRepository
                            .GetAll()
                            .AsNoTrackingWithIdentityResolution()
                            .Where(x =>
                                x.CompanyId.HasValue &&
                                companyIds.Contains(x.CompanyId.Value) &&
                                x.ServiceValueType == ServiceValueType.ForCompany &&
                                x.ServiceId.HasValue
                            )
                            .GroupBy(x => x.CompanyId.Value)
                            .Select(x => new CompanyWithServiceCountDto
                            {
                                CompanyId = x.Key,
                                Count = x.Select(x => x.ServiceId.Value).Distinct().Count()
                            })
                            .ToListAsync();

        }
        public async Task<List<CompanyBranchWithServiceCountDto>> GetCountOfServicesForCompanyBranches(List<int> companyBranchesIds)
        {
            return await _serviceValueRepository
                            .GetAll()
                            .AsNoTrackingWithIdentityResolution()
                            .Where(x =>
                                x.CompanyBranchId.HasValue &&
                                companyBranchesIds.Contains(x.CompanyBranchId.Value) &&
                                x.ServiceValueType == ServiceValueType.ForCompanyBranch &&
                                x.ServiceId.HasValue
                            )
                            .GroupBy(x => x.CompanyBranchId.Value)
                            .Select(x => new CompanyBranchWithServiceCountDto
                            {
                                CompanyBranchId = x.Key,
                                Count = x.Select(x => x.ServiceId.Value).Distinct().Count()
                            })
                            .ToListAsync();

        }
    }
}
