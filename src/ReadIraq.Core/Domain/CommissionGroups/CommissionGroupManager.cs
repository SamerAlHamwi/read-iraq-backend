using Abp.Domain.Repositories;
using Abp.Domain.Services;
using Abp.UI;
using Microsoft.EntityFrameworkCore;
using ReadIraq.Domain.CommissionGroups.Dtos;
using ReadIraq.Domain.Companies;
using ReadIraq.Localization.SourceFiles;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ReadIraq.Domain.CommissionGroups
{
    public class CommissionGroupManager : DomainService, ICommissionGroupManager
    {
        private readonly IRepository<CommissionGroup> _commissionGroupRepository;
        private readonly ICompanyManager _companyManager;
        public CommissionGroupManager(IRepository<CommissionGroup> commissionGroupRepository,
            ICompanyManager companyManager)
        {
            _commissionGroupRepository = commissionGroupRepository;
            _companyManager = companyManager;
        }
        public async Task AddNewCompanyToDefaultGroup(int companyId)
        {
            var defaultGroup = await _commissionGroupRepository.FirstOrDefaultAsync(x => x.IsDefault == true);
            defaultGroup.Companies.Add(await _companyManager.GetSuperLiteEntityByIdAsync(companyId));
            await _commissionGroupRepository.UpdateAsync(defaultGroup);
            await UnitOfWorkManager.Current.SaveChangesAsync();
        }
        public async Task<bool> CheckIfGroupContainCompanyAsync(int groupId, int companyId)
        {
            if (
                !await _commissionGroupRepository.GetAll()
                .AsNoTracking().Include(x => x.Companies)
                .AnyAsync(x => x.Id == groupId && x.Companies.Select(x => x.Id).Contains(companyId))
                )
                throw new UserFriendlyException(404, Exceptions.IncompatibleValue, Tokens.CommissionGroupWithCompany);
            return true;
        }

        public async Task<bool> CheckIfGroupExistAsync(int groupId)
        {
            if (!await _commissionGroupRepository.GetAll().AsNoTracking().AnyAsync(x => x.Id == groupId))
                throw new UserFriendlyException(404, Exceptions.ObjectWasNotFound, Tokens.CommissionGroup);
            return true;

        }

        public async Task CheckIfNameWasExisted(double name)
        {
            if (await _commissionGroupRepository.GetAll().AnyAsync(x => x.Name == name))
                throw new UserFriendlyException(403, Exceptions.ObjectIsAlreadyExist, Tokens.CommissionGroup);
        }

        public async Task<CommissionGroupDto> GetCommissionByCompanyIdAsync(int companyId)
        {
            return await _commissionGroupRepository.GetAll().Include(x => x.Companies)
                            .Where(x => x.Companies.Any(x => x.Id == companyId))
                            .Select(x => new CommissionGroupDto { Commission = x.Name })
                            .FirstOrDefaultAsync();
        }

        public async Task<CommissionGroup> GetCommissionGroupAsync(int groupId)
        {
            var group = await _commissionGroupRepository.GetAll()
                .Include(c => c.Companies)
                .Where(x => x.Id == groupId)
                .FirstOrDefaultAsync();
            if (group is null)
                throw new UserFriendlyException(404, Exceptions.ObjectWasNotFound, Tokens.CommissionGroup);
            return group;
        }

        public async Task<List<CommissionGroupWithCompanyIdsDto>> GetCommissionGroupByCompanyIds(List<int> companyIds)
        {
            return ObjectMapper.Map(await _commissionGroupRepository.GetAll()
                .AsNoTrackingWithIdentityResolution()
                .Include(x => x.Companies)
                .Where(x => x.Companies.Any(x => companyIds.Contains(x.Id)))
                .ToListAsync(), new List<CommissionGroupWithCompanyIdsDto>());

        }

        public async Task<int> GetCurrentCommissionGroupIdByCompanyId(int companyId)
        {
            return await _commissionGroupRepository.GetAll()
                  .AsNoTrackingWithIdentityResolution()
                  .Include(x => x.Companies)
                  .Where(x => x.Companies.Any(x => x.Id == companyId))
                  .Select(x => x.Id)
                  .FirstOrDefaultAsync();
        }

        public async Task ReAddCompanyInToOwnsGroup(Company company, int groupId)
        {
            var group = await GetCommissionGroupAsync(groupId);
            group.Companies.Add(company);
            await _commissionGroupRepository.UpdateAsync(group);
            await UnitOfWorkManager.Current.SaveChangesAsync();
        }

        public async Task RemoveCompanyFromCommission(Company company)
        {
            var group = await _commissionGroupRepository.GetAll()
                       .Include(x => x.Companies)
                       .Where(x => x.Companies.Any(x => x.Id == company.Id))
                       .FirstOrDefaultAsync();
            group.Companies.Remove(company);
            await _commissionGroupRepository.UpdateAsync(group);
            await UnitOfWorkManager.Current.SaveChangesAsync();
        }
    }
}
