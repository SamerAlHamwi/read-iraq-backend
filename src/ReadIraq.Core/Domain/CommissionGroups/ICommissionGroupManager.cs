using Abp.Domain.Services;
using ReadIraq.Domain.CommissionGroups.Dtos;
using ReadIraq.Domain.Companies;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ReadIraq.Domain.CommissionGroups
{
    public interface ICommissionGroupManager : IDomainService
    {
        Task<bool> CheckIfGroupExistAsync(int groupId);
        Task<CommissionGroup> GetCommissionGroupAsync(int groupId);
        Task<bool> CheckIfGroupContainCompanyAsync(int groupId, int companyId);
        Task CheckIfNameWasExisted(double name);
        Task AddNewCompanyToDefaultGroup(int companyId);
        Task<CommissionGroupDto> GetCommissionByCompanyIdAsync(int companyId);
        Task<List<CommissionGroupWithCompanyIdsDto>> GetCommissionGroupByCompanyIds(List<int> companyIds);
        Task<int> GetCurrentCommissionGroupIdByCompanyId(int companyId);
        Task ReAddCompanyInToOwnsGroup(Company company, int groupId);
        Task RemoveCompanyFromCommission(Company company);
    }
}
