using Abp.Domain.Repositories;
using Abp.Domain.Services;
using Abp.UI;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using ReadIraq.Authorization.Users;
using ReadIraq.Domain.Companies;
using ReadIraq.Domain.Companies.Dto;
using ReadIraq.Domain.CompanyBranches;
using ReadIraq.Domain.CompanyBranches.Dto;
using ReadIraq.Localization.SourceFiles;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static ReadIraq.Enums.Enum;

namespace ReadIraq.Domain.SelectedCompaniesByUsers
{
    public class SelectedCompaniesBySystemForRequestManager : DomainService, ISelectedCompaniesBySystemForRequestManager
    {
        private readonly IRepository<SelectedCompaniesBySystemForRequest, Guid> _selectedCompaniesBySystemRepository;
        private readonly ICompanyManager _companyManager;
        private readonly IMapper _mapper;
        private readonly ICompanyBranchManager _companyBranchManager;
        public SelectedCompaniesBySystemForRequestManager(IRepository<SelectedCompaniesBySystemForRequest, Guid> selectedCompaniesBySystemRepository,
            ICompanyManager companyManager,
            IMapper mapper,
            ICompanyBranchManager companyBranchManager)
        {
            _selectedCompaniesBySystemRepository = selectedCompaniesBySystemRepository;
            _companyManager = companyManager;
            _mapper = mapper;
            _companyBranchManager = companyBranchManager;
        }
        public async Task CheckIfEntityExict(Guid id)
        {
            if (!await _selectedCompaniesBySystemRepository.GetAll().AsNoTrackingWithIdentityResolution().AnyAsync(x => x.Id == id))
                throw new UserFriendlyException(Exceptions.ObjectWasNotFound, Tokens.Entity);
        }

        public async Task<List<long>> GetRequestIdsByCompanyId(int companyId)
        {
            return await _selectedCompaniesBySystemRepository
                .GetAll()
                .AsNoTracking()
                .Where(x => x.CompanyId.Value == companyId)
                .Select(x => x.RequestForQuotationId).ToListAsync();
        }
        public async Task<List<long>> GetRequestIdsByCompanyBranchId(int companyBranchId)
        {
            return await _selectedCompaniesBySystemRepository
                .GetAll()
                .AsNoTracking()
                .Where(x => x.CompanyBranchId.Value == companyBranchId)
                .Select(x => x.RequestForQuotationId).ToListAsync();
        }

        public async Task CheckIfEntityExictByRequestId(long requestId)
        {
            if (!await _selectedCompaniesBySystemRepository.GetAllIncluding(x => x.RequestForQuotation).AsNoTrackingWithIdentityResolution().AnyAsync(x => x.RequestForQuotationId == requestId && x.RequestForQuotation.Statues == Enums.Enum.RequestForQuotationStatues.Approved))
                throw new UserFriendlyException(Exceptions.ObjectWasNotFound, Tokens.Entity);
        }

        public async Task<Guid> GetEntityIdByRequestId(long requestId, User user)
        {
            if (user.Type == UserType.CompanyUser)
                return await _selectedCompaniesBySystemRepository
                     .GetAll()
                     .AsNoTrackingWithIdentityResolution()
                     .Where(x => x.RequestForQuotationId == requestId && x.CompanyId == (_companyManager.GetCompnayIdByUserId(user.Id).GetAwaiter().GetResult()))
                     .Select(X => X.Id).FirstOrDefaultAsync();
            else
                return await _selectedCompaniesBySystemRepository
                        .GetAll()
                        .AsNoTrackingWithIdentityResolution()
                        .Where(x => x.RequestForQuotationId == requestId && x.CompanyBranchId == (_companyBranchManager.GetCompnayBranchIdByUserId(user.Id).GetAwaiter().GetResult()))
                        .Select(X => X.Id).FirstOrDefaultAsync();
        }


        //public async Task<List<long>> GetRequestIdsWhichConnectedWithCompanyAndBeenRejectedFromUserForCompany(int companyId)
        //{
        //    return await _selectedCompaniesBySystemRepository
        //                .GetAll()
        //                .AsNoTracking()
        //                .Include(x => x.RequestForQuotation)
        //                .Where(x => x.CompanyId.Value == companyId && x.RequestForQuotation.Statues == RequestForQuotationStatues.Possible)
        //                .Select(x => x.RequestForQuotationId).ToListAsync();
        //}

        //public async Task<List<long>> GetRequestIdsWhichConnectedWithCompanyAndBeenRejectedFromUserForCompanyBranch(int companyBranchId)
        //{
        //    return await _selectedCompaniesBySystemRepository
        //                .GetAll()
        //                .AsNoTracking()
        //                .Include(x => x.RequestForQuotation)
        //                .Where(x => x.CompanyBranchId.Value == companyBranchId && x.RequestForQuotation.Statues == RequestForQuotationStatues.Possible)
        //                .Select(x => x.RequestForQuotationId).ToListAsync();
        //}



        public async Task<List<RequestsForCompanyCountDto>> GetRequestsForCompanyCountDto()
        {



            var result = await _selectedCompaniesBySystemRepository.GetAll().AsNoTracking().Where(x => x.CompanyId.HasValue)
                                .GroupBy(item => item.CompanyId)
                                .Select(group => new RequestsForCompanyCountDto
                                {
                                    CompanyId = group.Key.Value,
                                    RequestForQuotationCount = group.Select(item => item.RequestForQuotationId).Distinct().Count()
                                })
                                .OrderByDescending(result => result.RequestForQuotationCount).ToListAsync();
            var resultToReturn = new List<RequestsForCompanyCountDto>();
            foreach (var item in result)
            {
                item.Company = _mapper.Map<LiteCompanyDto>(await _companyManager.GetLiteCompanyByIdAsync(item.CompanyId));
                if (item.Company is not null)
                    resultToReturn.Add(item);
            }
            if (resultToReturn is null)
                return new List<RequestsForCompanyCountDto>();
            return resultToReturn;
        }
        public async Task<List<RequestsForCompanyBranchCountDto>> GetRequestsForCompanyBranchCountDto()
        {



            var result = await _selectedCompaniesBySystemRepository.GetAll().AsNoTracking().Where(x => x.CompanyBranchId.HasValue)
                                .GroupBy(item => item.CompanyBranchId)
                                .Select(group => new RequestsForCompanyBranchCountDto
                                {
                                    CompanyBranchId = group.Key.Value,
                                    RequestForQuotationCount = group.Select(item => item.RequestForQuotationId).Distinct().Count()
                                })
                                .OrderByDescending(result => result.RequestForQuotationCount).ToListAsync();
            var resultToReturn = new List<RequestsForCompanyBranchCountDto>();
            foreach (var item in result)
            {
                item.CompanyBranch = _mapper.Map<LiteCompanyBranchDto>(await _companyBranchManager.GetLiteEntityByIdAsync(item.CompanyBranchId));
                if (item.CompanyBranch is not null)
                    resultToReturn.Add(item);
            }
            if (resultToReturn is null)
                return new List<RequestsForCompanyBranchCountDto>();
            return resultToReturn;
        }

        public async Task<List<int>> GetCompanyIdsWithThisRequestAsync(long requestId)
        {
            return await _selectedCompaniesBySystemRepository.GetAll()
                .AsNoTrackingWithIdentityResolution().Where(c => c.CompanyId.HasValue && c.RequestForQuotationId == requestId)
                .Select(x => x.CompanyId.Value)
                .ToListAsync();
        }

        public async Task<List<int>> GetCompanyBranchIdsWithThisRequestAsync(long requestId)
        {
            return await _selectedCompaniesBySystemRepository.GetAll()
                          .AsNoTrackingWithIdentityResolution().Where(c => c.CompanyBranchId.HasValue && c.RequestForQuotationId == requestId)
                          .Select(x => x.CompanyBranchId.Value)
                          .ToListAsync();
        }
    }
}
