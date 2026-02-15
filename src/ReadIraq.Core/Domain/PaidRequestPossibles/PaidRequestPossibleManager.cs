using Abp.Domain.Repositories;
using Abp.Domain.Services;
using Microsoft.EntityFrameworkCore;
using ReadIraq.Domain.Companies;
using ReadIraq.Domain.CompanyBranches;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading.Tasks;

namespace ReadIraq.Domain.PaidRequestPossibles
{
    public class PaidRequestPossibleManager : DomainService, IPaidRequestPossibleManager
    {
        private readonly IRepository<PaidRequestPossible> _paidRequestPossibleRepository;
        private readonly ICompanyManager _companyManager;
        private readonly ICompanyBranchManager _companyBranchManager;
        public PaidRequestPossibleManager(IRepository<PaidRequestPossible> paidRequestPossibleRepository,
            ICompanyManager companyManager,
            ICompanyBranchManager companyBranchManager)
        {
            _paidRequestPossibleRepository = paidRequestPossibleRepository;
            _companyManager = companyManager;
            _companyBranchManager = companyBranchManager;
        }



        public async Task<bool> CheckIfCompanyBranchPaidForRequestContact(int companyBranchId, long requsetId)
        {
            return await _paidRequestPossibleRepository.GetAll().AnyAsync(x => x.CompanyBranchId == companyBranchId && x.RequestId == requsetId);
        }

        public async Task<bool> CheckIfCompanyPaidForRequestContact(int companyId, long requsetId)
        {
            return await _paidRequestPossibleRepository.GetAll().AnyAsync(x => x.CompanyId == companyId && x.RequestId == requsetId);
        }

        public async Task InsertNewEntity(PaidRequestPossible entity)
        {
            await _paidRequestPossibleRepository.InsertAsync(entity);
            await UnitOfWorkManager.Current.SaveChangesAsync();
        }
        public async Task<List<int>> GetCompaniesIdsWhichBoughtInfoUser(long requsetId)
        {
            return await _paidRequestPossibleRepository.GetAll().Where(x => x.RequestId == requsetId && x.CompanyId.HasValue && x.IsDeleted == false).Select(x => x.CompanyId.Value).ToListAsync();
        }
        public async Task<List<int>> GetCompanyBranchesIdsWhichBoughtInfoUser(long requsetId)
        {
            return await _paidRequestPossibleRepository.GetAll().Where(x => x.RequestId == requsetId && x.CompanyBranchId.HasValue && x.IsDeleted == false).Select(x => x.CompanyBranchId.Value).ToListAsync();
        }

        public async Task<List<long>> GetAllPaidRequestIdsWithThisCompany(int id, bool forCompany = true)
        {
            if (forCompany)
                return await _paidRequestPossibleRepository
                    .GetAll()
                    .AsNoTracking()
                    .Where(x => x.CompanyId.HasValue && x.CompanyId == id)
                    .Select(x => x.RequestId)
                    .ToListAsync();
            return await _paidRequestPossibleRepository
                    .GetAll()
                    .AsNoTracking()
                    .Where(x => x.CompanyBranchId.HasValue && x.CompanyBranchId == id)
                    .Select(x => x.RequestId)
                    .ToListAsync();

        }
    }
}
