using Abp.Domain.Services;
using ReadIraq.Authorization.Users;
using ReadIraq.Domain.Companies.Dto;
using ReadIraq.Domain.CompanyBranches.Dto;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ReadIraq.Domain.SelectedCompaniesByUsers
{
    public interface ISelectedCompaniesBySystemForRequestManager : IDomainService
    {
        Task CheckIfEntityExict(Guid id);
        Task CheckIfEntityExictByRequestId(long requestId);
        Task<Guid> GetEntityIdByRequestId(long requestId, User user);
        Task<List<long>> GetRequestIdsByCompanyId(int companyId);
        Task<List<long>> GetRequestIdsByCompanyBranchId(int companyBranchId);
        //Task<List<long>> GetRequestIdsWhichConnectedWithCompanyAndBeenRejectedFromUserForCompany(int companyId);
        //Task<List<long>> GetRequestIdsWhichConnectedWithCompanyAndBeenRejectedFromUserForCompanyBranch(int companyBranchId);
        Task<List<RequestsForCompanyCountDto>> GetRequestsForCompanyCountDto();
        Task<List<RequestsForCompanyBranchCountDto>> GetRequestsForCompanyBranchCountDto();
        Task<List<int>> GetCompanyIdsWithThisRequestAsync(long requestId);
        Task<List<int>> GetCompanyBranchIdsWithThisRequestAsync(long requestId);
    }
}
