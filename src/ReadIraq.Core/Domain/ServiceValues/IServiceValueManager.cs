using Abp.Domain.Services;
using ReadIraq.Domain.Companies.Dto;
using ReadIraq.Domain.RequestForQuotations.Dto;
using ReadIraq.Domain.services.Dto;
using ReadIraq.Domain.ServiceValues.Dto;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ReadIraq.Domain.ServiceValues
{
    public interface IServiceValueManager : IDomainService
    {
        Task<bool> InsertServiceValuesForUser(List<ServiceValuesDto> serviceValues, long requestForQuotationId);
        Task<bool> InsertServiceValuesForCompany(List<ServiceValuesDto> serviceValues, int companyId);
        Task<List<ServiceDetailsDto>> GetFullServicesByCompanyOrRequestIdAsync(long id, bool forCompany = false, bool forCompanyBranch = false);
        Task<List<ServiceIdsAndRequestIdsDto>> GetServicesIdsAndRequestIdsAsync(List<long> requestIds);
        Task<List<ServiceIdsAndCompanyIdsDto>> GetServicesIdsAndCompanyIdsAsync(List<int> companyIds);
        Task<List<ServiceIdsAndCompanyBranchIdsDto>> GetServicesIdsAndCompanyBranchIdsAsync(List<int> companyBranchIds);

        Task<bool> InsertServiceValuesForCompanyBranch(List<ServiceValuesDto> serviceValues, int companyId, int companyBracnhId);
        Task HardDeleteServiceValues(List<ServiceValue> serviceValues);
        Task DeleteServiceValuesForCompanyBranch(int branchId);
        Task<CompanyAndCompanyBranchIdsDto> FilterCompatibleCompaniesWithRequestByServices(long requestId, bool forCompany = false, bool getCompanyonlyAcceptPossibleRequest = false);
        Task<List<int>> GetServiceIdsForCompnayOrRequestAsync(long id, bool forCompany = false);
        Task<CompanyAndCompanyBranchIdsDto> GetCompanyOrCompanyBracnhIdsCompatibleWithRequest(List<int> serviceIds, List<int> cityIds, bool forCompany = true);
        //Task<CompanyAndCompanyBranchIdsDto> GetCompanyBranchIdsCompatibleWithRequest(List<int> serviceIds);
        Task<bool> CheckIfServiceBelongsToRequest(int servicetId);
        Task<bool> CheckIfServiceBelongsToCompanyOrCompanyBranch(int servicetId);

        Task<bool> CheckIfSubServiceBelongsToRequest(int subServicetId);
        Task<bool> CheckIfSubServiceBelongsToCompanyOrCompanyBranch(int subServicetId);

        Task<bool> CheckIfToolBelongsToRequest(int toolId);
        Task<bool> CheckIfToolBelongsToCompanyOrCompanyBranch(int toolId);
        Task<List<ServiceStatisticsForRequestsDto>> GetServicesStatisticsForRequestsDto();
        Task<List<ServiceValuesDto>> GetAllServiceValuesForCompany(int companyId);
        Task<int> GetCountOfServicesForRequest(long requestId);
        Task<List<CompanyWithServiceCountDto>> GetCountOfServicesForCompanies(List<int> companyIds);
        Task<List<CompanyBranchWithServiceCountDto>> GetCountOfServicesForCompanyBranches(List<int> companyBranchIds);
    }
}
