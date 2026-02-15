using Abp.Domain.Services;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ReadIraq.Domain.PaidRequestPossibles
{
    public interface IPaidRequestPossibleManager : IDomainService
    {
        Task InsertNewEntity(PaidRequestPossible entity);
        Task<bool> CheckIfCompanyPaidForRequestContact(int companyBranchId, long requsetId);
        Task<bool> CheckIfCompanyBranchPaidForRequestContact(int companyId, long requsetId);
        Task<List<int>> GetCompanyBranchesIdsWhichBoughtInfoUser(long requsetId);
        Task<List<int>> GetCompaniesIdsWhichBoughtInfoUser(long requsetId);
        Task<List<long>> GetAllPaidRequestIdsWithThisCompany(int id, bool forCompany = true);
    }
}
