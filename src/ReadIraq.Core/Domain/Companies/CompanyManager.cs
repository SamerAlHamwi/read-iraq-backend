using Abp.Collections.Extensions;
using Abp.Domain.Entities;
using Abp.Domain.Repositories;
using Abp.Domain.Services;
using Abp.Domain.Uow;
using Abp.EntityFrameworkCore.Repositories;
using Abp.UI;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using ReadIraq.Authorization.Users;
using ReadIraq.Domain.Attachments;
using ReadIraq.Domain.AttributeChoices;
using ReadIraq.Domain.Cities;
using ReadIraq.Domain.Companies.Dto;
using ReadIraq.Domain.CompanyBranches;
using ReadIraq.Domain.RequestForQuotations;
using ReadIraq.Domain.Reviews;
using ReadIraq.Domain.Reviews.Dto;
using ReadIraq.Domain.ServiceValues.Dto;
using ReadIraq.Domain.SourceTypes;
using ReadIraq.Domain.TimeWorks;
using ReadIraq.Domain.TimeWorks.Dtos;
using ReadIraq.Localization.SourceFiles;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static ReadIraq.Enums.Enum;

namespace ReadIraq.Domain.Companies
{
    public class CompanyManager : DomainService, ICompanyManager
    {
        private readonly IRepository<Company> _companyRepository;
        private readonly IMapper _mapper;
        private readonly IRepository<Review> _reviewRepository;
        private readonly IRepository<CompanyBranch> _companyBranchesRepository;
        private readonly IRepository<CompanyContact> _companyContactRepository;
        private readonly IRepository<CompanyTranslation> _companyTranslationsRepository;
        private readonly IAttachmentManager _attachmentManager;
        private readonly ICityManager _cityManager;
        private readonly IRequestForQuotationManager _requestForQuotationManager;
        private readonly ISourceTypeManager _sourceTypeManager;
        private readonly IRepository<TimeWork> _timeWorkRepository;
        private readonly IAttributeChoiceManger _attributeChoiceManger;
        private readonly UserManager _userManager;
        public CompanyManager(IRepository<CompanyBranch> companyBranchesRepository,
            IRepository<CompanyTranslation> companyTranslationsRepository,
            IAttachmentManager attachmentManager,
            ICityManager cityManager

            , IRepository<CompanyContact> companyContactRepository,
            IRepository<Company> companyRepository,
            IMapper mapper,
            IRepository<Review> reviewRepository,
            IRequestForQuotationManager requestForQuotationManager,
            ISourceTypeManager sourceTypeManager,
            IRepository<TimeWork> timeWorkRepository,
            IAttributeChoiceManger attributeChoiceManger,
            UserManager userManager)
        {
            _companyBranchesRepository = companyBranchesRepository;
            _companyContactRepository = companyContactRepository;
            _companyRepository = companyRepository;
            _mapper = mapper;
            _reviewRepository = reviewRepository;
            _companyTranslationsRepository = companyTranslationsRepository;
            _attachmentManager = attachmentManager;
            _cityManager = cityManager;
            _requestForQuotationManager = requestForQuotationManager;
            _sourceTypeManager = sourceTypeManager;
            _timeWorkRepository = timeWorkRepository;
            _attributeChoiceManger = attributeChoiceManger;
            _userManager = userManager;
        }
        public async Task<Company> GetSuperLiteEntityByIdAsync(int id)
        {
            return await _companyRepository.GetAsync(id);
        }
        public async Task<Company> GetLiteEntityByIdAsync(int id)
        {
            return await _companyRepository
                .GetAllIncluding(x => x.Translations)
                .Include(x => x.AvailableCities)
                .ThenInclude(x => x.Translations)
                .Include(x => x.AvailableCities)
                .ThenInclude(x => x.Country)
                .ThenInclude(x => x.Translations)
                .Include(x => x.CompanyContact)
                .AsNoTracking().Where(x => x.Id == id).FirstOrDefaultAsync();
        }
        public async Task<Company> GetEntityByIdAsync(int id)
        {
            return await _companyRepository.GetAll()
                .AsNoTrackingWithIdentityResolution().
                Include(x => x.AvailableCities)
                .ThenInclude(x => x.Translations)
                .Include(x => x.User)
                .Include(x => x.services)
                .Include(x => x.Translations)
                .Include(x => x.CompanyContact)
                .Where(x => x.Id == id).FirstOrDefaultAsync();
        }
        public async Task<Company> GetLiteCompanyByIdAsync(int id)
        {
            return await _companyRepository
                .GetAllIncluding(x => x.Translations)

                .AsNoTracking().Where(x => x.Id == id).FirstOrDefaultAsync();
        }

        public async Task<List<CompanyBranch>> CheckAndGetCompanyBranch(List<int> companyBranchIds)
        {
            List<CompanyBranch> companyBranches = new List<CompanyBranch>();
            foreach (var companyBranchId in companyBranchIds)
            {
                if (!await CheckIfCompanyBrachExict(companyBranchId))
                    throw new UserFriendlyException(404, Exceptions.ObjectWasNotFound, Tokens.CompanyBranch);
                companyBranches.Add(await _companyBranchesRepository.GetAsync(companyBranchId));
            }
            return companyBranches;
        }

        public async Task<bool> CheckIfCompanyBrachExict(int companyBranchId)
        {
            return await _companyBranchesRepository.GetAll().AnyAsync(x => x.Id == companyBranchId);
        }



        public async Task<CompanyContactDetailsDto> GetCompanyContactDtoByCompanyId(int companyId)
        {
            var company = await _companyRepository.GetAll().AsNoTrackingWithIdentityResolution().Include(x => x.CompanyContact).Where(x => x.Id == companyId).FirstOrDefaultAsync();
            var companyContact = company.CompanyContact;
            return ObjectMapper.Map<CompanyContactDetailsDto>(companyContact);
        }


        public async Task<string> CreateCodeForCompany()
        {
            string firstPrefix = "CO";
            Random random = new Random();
            string generatedCode;
            do
            {
                int randomNumber = random.Next(1, 1000);
                generatedCode = $"{firstPrefix}{randomNumber:D3}";
            }
            while (await _companyRepository.GetAll().AnyAsync(x => x.Code == generatedCode));
            return generatedCode;
        }



        public async Task<List<Company>> GetListOfCompany(List<int> companyIds)
        {
            return await _companyRepository.GetAll().Where(x => companyIds.Contains(x.Id)).ToListAsync();
        }

        public async Task<bool> CheckIfUserHasCompany(long userId)
        {
            return await _companyRepository.GetAll().AsNoTrackingWithIdentityResolution().AnyAsync(x => x.UserId == userId);
        }

        public async Task<bool> CheckIfCompanyExict(int companyId)
        {
            if (!await _companyRepository.GetAll().AnyAsync(x => x.Id == companyId))
            {
                throw new UserFriendlyException(404, Exceptions.ObjectWasNotFound, Tokens.Company + " " + companyId.ToString());

            }
            return true;
        }
        public async Task UpdateAttachmentTypeListAsync(List<long> newAttachmentIds, AttachmentRefType attachmentType, long companyId)
        {
            var existingAttachments = await _attachmentManager.GetByRefAsync(companyId, attachmentType);
            var imagesattachmentsToDelete = existingAttachments.Where(x => !newAttachmentIds.Contains((x.Id)));
            var imagesattachmentIdsToAdd = newAttachmentIds.Except(existingAttachments.Select(x => x.Id).ToList());
            foreach (var existingAttachment in imagesattachmentsToDelete)
            {
                await _attachmentManager.DeleteRefIdAsync(existingAttachment);
            }

            foreach (var newAttachmentId in imagesattachmentIdsToAdd)
            {
                await _attachmentManager.CheckAndUpdateRefIdAsync(
                    newAttachmentId, attachmentType, companyId);
            }
        }

        //public async Task UpdateContcatCompany(CompanyContact companyContact, int companyId)
        //{
        //    companyContact.CompanyId = companyId;
        //    await _companyContactRepository.UpdateAsync(companyContact);
        //    await UnitOfWorkManager.Current.SaveChangesAsync();
        //}
        public async Task HardDeleteCompanyTranslation(List<CompanyTranslation> translations)
        {
            try
            {
                foreach (var translation in translations)
                {

                    await _companyTranslationsRepository.HardDeleteAsync(translation);
                }
            }
            catch (Exception ex) { throw; }
        }

        public async Task<CompanyAndBranchDto> GetSelectedCompanyAndBranch(CompanyAndCompanyBranchIdsDto input)
        {
            var companies = new List<Company>();
            var companyBranches = new List<CompanyBranch>();
            if (input.CompanyIds is not null)
            {
                companies = await _companyRepository.GetAll().AsNoTracking().Where(x => input.CompanyIds.Contains(x.Id)).ToListAsync();
            }
            if (input.CompanyBranchIds is not null)
            {
                companyBranches = await _companyBranchesRepository.GetAll().AsNoTracking().Where(x => input.CompanyBranchIds.Contains(x.Id)).ToListAsync();
            }
            return new CompanyAndBranchDto { Companies = companies, CompanyBranches = companyBranches };
        }

        public async Task<int> GetCompnayIdByUserId(long userId)
        {
            return await _companyRepository.GetAll().AsNoTrackingWithIdentityResolution().Where(x => x.UserId == userId).Select(x => x.Id).FirstOrDefaultAsync();
        }

        public async Task<bool> UpdateCitiesByCompanyIdAsync(List<int> citiesIds, Company company)
        {
            try
            {

                if (!citiesIds.IsNullOrEmpty())
                {
                    var oldCities = company.AvailableCities.ToList();
                    var newCities = await _cityManager.CheckAndGetCitiesById(citiesIds);

                    var cityToDelete = oldCities.Except(newCities).ToList();
                    foreach (var city in cityToDelete)
                    {
                        company.AvailableCities.Remove(city);
                    }
                    foreach (var city in newCities)
                    {
                        if (!company.AvailableCities.Contains(city))
                            company.AvailableCities.Add(city);
                    }
                }
                if (citiesIds.IsNullOrEmpty() || citiesIds.Count() == 0)
                    company.AvailableCities.Clear();

                await UnitOfWorkManager.Current.SaveChangesAsync();
                return true;
            }
            catch (Exception ex) { throw; }
        }

        public async Task<List<int>> GetCompanyIdsThatContainsSameCitiesInRequest(long requestIds)
        {
            try
            {
                var cityIdsForRequest = await _requestForQuotationManager.GetCityIdsForRequest(requestIds);
                var companyIds = await _companyRepository.GetAll()
                    .AsNoTracking()
                    .Include(x => x.AvailableCities)
                    .Where(x => x.AvailableCities.Any(x => cityIdsForRequest.Contains(x.Id))).Select(x => x.Id).ToListAsync();
                return companyIds;
            }
            catch (Exception ex) { throw new UserFriendlyException(ex.Message); }
        }

        public async Task<GeneralRatingDto> GetGeneralRatingDtoForComapny(int companyId)
        {
            //var companyId = _offerManager.GetEntityByIdAsync()
            var result = await _reviewRepository
                .GetAll().Include(x => x.Offer).ThenInclude(x => x.SelectedCompanies)
                .AsNoTracking()
                .Where(x => x.Offer.SelectedCompanies.CompanyId.Value == companyId)
                .GroupBy(x => 1) // Group all reviews into a single group
                .Select(group => new GeneralRatingDto
                {
                    Quality = group.Average(x => x.Quality),
                    OverallRating = group.Average(x => x.OverallRating),
                    ValueOfServiceForMoney = group.Average(x => x.ValueOfServiceForMoney),
                    CustomerService = group.Average(x => x.CustomerService)
                })
               .FirstOrDefaultAsync();
            if (result is null) return new GeneralRatingDto();
            return result;
        }

        public async Task<CompanyStatuesDto> GetCompanyStatuesByCompanyBranchId(int companyBranchId)
        {
            var company = await _companyRepository.GetAll().AsNoTracking()
                 .Include(x => x.CompanyBranches)
                 .Where(x => x.CompanyBranches.Any(x => x.Id == companyBranchId))
                 .Select(x => new { x.statues, x.ReasonRefuse }).FirstOrDefaultAsync();
            return new CompanyStatuesDto
            {
                Statues = company.statues,
                ReasonRefuse = company.ReasonRefuse
            };
        }
        public async Task<List<ReviewDetailsDto>> GetReviewsForCompany(int companyId)
        {
            using (UnitOfWorkManager.Current.DisableFilter(AbpDataFilters.MayHaveTenant, AbpDataFilters.MustHaveTenant))
            {
                var reviews = await _reviewRepository
              .GetAll().Include(x => x.Offer).ThenInclude(x => x.SelectedCompanies).Include(x => x.User)
              .AsNoTracking()
              .Where(x => x.Offer.SelectedCompanies.CompanyId.Value == companyId)
              .Select(x => _mapper.Map<ReviewDetailsDto>(x)).ToListAsync();
                return reviews;
            }
        }


        //public async Task GiftPointToCompany(Company company)
        //{
        //    var pointsToGift = await SettingManager.GetSettingValueAsync<int>(AppSettingNames.NumberOfPointsToGitft);
        //    company.NumberOfGiftedPoints = +pointsToGift;
        //    await _companyRepository.UpdateAsync(company);
        //}

        public async Task<Company> GetCompanyEntityById(int companyId)
        {
            var company = await _companyRepository.GetAsync(companyId);
            if (company is null)
                throw new UserFriendlyException(Exceptions.ObjectWasNotFound, Tokens.Company);
            return company;
        }

        public async Task GetPointFromCompanyForGettingContactRequest(int companyId, int pointsToGetRequest)
        {
            var company = await GetCompanyEntityById(companyId);
            if (pointsToGetRequest > (company.NumberOfGiftedPoints + company.NumberOfPaidPoints))
                throw new UserFriendlyException(515, Exceptions.YouDonotHAveMuchPoints);
            if (pointsToGetRequest <= company.NumberOfGiftedPoints)
            {
                // Deduct points from giftedPoints
                company.NumberOfGiftedPoints -= pointsToGetRequest;
            }
            else
            {
                // Deduct all available giftedPoints
                pointsToGetRequest -= company.NumberOfGiftedPoints;
                company.NumberOfGiftedPoints = 0;

                // If there are still points to deduct, deduct from paidPoints
                if (pointsToGetRequest <= company.NumberOfPaidPoints)
                {
                    // Deduct points from paidPoints
                    company.NumberOfPaidPoints -= pointsToGetRequest;
                }
            }
            await _companyRepository.UpdateAsync(company);
            await UnitOfWorkManager.Current.SaveChangesAsync();
        }

        public async Task AddPaidPointsToCompany(int numberOfPoint, int companyId)
        {
            var company = await _companyRepository
                .GetAll().Where(x => x.Id == companyId).FirstOrDefaultAsync();
            if (company == null)
                throw new EntityNotFoundException(typeof(Company), companyId);

            company.NumberOfPaidPoints = company.NumberOfPaidPoints + numberOfPoint;
            await _companyRepository.UpdateAsync(company);
        }

        public async Task<int> GetCompaniesCount()
        {
            return await _companyRepository.GetAll().AsNoTracking().Where(x => x.IsDeleted == false).CountAsync();
        }

        public async Task GiftPointToCompany(int companyId, int sourceTypeId, List<int> choiceIds)
        {
            var company = await GetCompanyEntityById(companyId);
            int pointsForGiftToCompany = await _sourceTypeManager.GetPointsToGiftCompanyBySourceTypeIdAsync(sourceTypeId);
            if (pointsForGiftToCompany == 111111111)
                pointsForGiftToCompany = await _attributeChoiceManger.GetPointsToGiftToCompanyByAttributeChoices(choiceIds);
            company.NumberOfGiftedPoints += pointsForGiftToCompany;
            company.NumberOfTransfers += 1;
            await _companyRepository.UpdateAsync(company);
        }

        public async Task<List<int>> FilterCompanyIdsThatOnlyAcceptPossibleRequest(List<int> companyIds)
        {
            return await _companyRepository.GetAll()
                 .AsNoTracking()
                 .Where(x => x.AcceptPossibleRequests == true && companyIds.Contains(x.Id))
                 .Select(x => x.Id)
                 .ToListAsync();
        }

        public async Task CheckIfCompanyIsFeature(int companyId)
        {
            if (await _companyRepository.GetAll().AnyAsync(x => x.Id == companyId && x.IsFeature))
                throw new UserFriendlyException(Exceptions.ObjectIsAlreadyExist, Tokens.Company);
        }

        public async Task MakeCompanyAsFeature(int numberInMonths, int companyId)
        {
            var company = await _companyRepository.GetAsync(companyId);
            company.StartFeatureSubscribtionDate = DateTime.UtcNow;
            company.EndFeatureSubscribtionDate = DateTime.UtcNow.AddMonths(numberInMonths);
            company.IsFeature = true;
            await _companyRepository.UpdateAsync(company);
            await UnitOfWorkManager.Current.SaveChangesAsync();
        }

        public async Task MakeAllCompaniesNotFeatureIfTimeEndedAsync()
        {
            await _companyRepository.GetAll()
                               .AsTracking()
                               .Where(x => x.IsFeature && x.EndFeatureSubscribtionDate.HasValue && x.EndFeatureSubscribtionDate.Value.Date <= DateTime.UtcNow)
                            .ExecuteUpdateAsync(se => se
                            .SetProperty(x => x.IsFeature, false)
                            .SetProperty(x => x.StartFeatureSubscribtionDate, (DateTime?)null)
                            .SetProperty(x => x.EndFeatureSubscribtionDate, (DateTime?)null));
            //foreach (var item in companies)
            //{
            //    item.IsFeature = false;
            //    item.StartFeatureSubscribtionDate = null;
            //    item.EndFeatureSubscribtionDate = null;
            //    await _companyRepository.UpdateAsync(item);
            //}
            await UnitOfWorkManager.Current.SaveChangesAsync();

        }

        public async Task CheckIfCompanyHasTimeWorkThenDeleteIt(int companyId)
        {
            await _timeWorkRepository.GetAll().Where(x => x.CompanyId == companyId).ExecuteDeleteAsync();
        }

        public async Task InsertNewTimeWorksForCompany(List<TimeWork> timeWorksToInsert)
        {
            await _timeWorkRepository.InsertRangeAsync(timeWorksToInsert);
        }

        public async Task<List<TimeOfWorkDto>> GetTimeWorksDtoForCompany(int companyId)
        {
            return ObjectMapper.Map(await _timeWorkRepository.GetAll().AsNoTracking().Where(x => x.CompanyId.HasValue && x.CompanyId.Value == companyId).ToListAsync(), new List<TimeOfWorkDto>());
        }

        public async Task<long> GetUserIdByCompanyIdAsync(int companyId)
        {
            return await _companyRepository.GetAll()
                            .AsNoTrackingWithIdentityResolution()
                            .Where(x => x.Id == companyId)
                            .Select(x => x.UserId.Value)
                            .FirstOrDefaultAsync();

        }

        public async Task DeleteUpdatedCompanyInstance(int companyId)
        {
            await _companyRepository.GetAll().Where(x => x.Id == companyId).ExecuteUpdateAsync(s => s.SetProperty(x => x.IsDeleted, true));
        }

        public async Task<CompanyStatuesDto> GetChildCompanyStatuesIfFound(int companyId)
        {
            return await _companyRepository.GetAll().AsNoTrackingWithIdentityResolution()
                  .Where(x => x.ParentCompanyId.HasValue && x.ParentCompanyId == companyId)
                  .Select(x => new CompanyStatuesDto { CompanyId = x.Id, Statues = x.statues, ReasonRefuse = x.ReasonRefuse })
                  .FirstOrDefaultAsync();
        }

        public async Task DeleteUserForCompanyOrBranchAsync(long userId)
        {
            using (UnitOfWorkManager.Current.DisableFilter(AbpDataFilters.MayHaveTenant, AbpDataFilters.MustHaveTenant))
            {
                await _userManager.Users.Where(x => x.Id == userId).ExecuteUpdateAsync(s => s.SetProperty(x => x.IsDeleted, true));
                await UnitOfWorkManager.Current.SaveChangesAsync();
            }
        }
    }
}