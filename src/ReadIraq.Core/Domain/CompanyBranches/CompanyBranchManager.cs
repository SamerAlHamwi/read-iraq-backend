using Abp.Collections.Extensions;
using Abp.Domain.Entities;
using Abp.Domain.Repositories;
using Abp.Domain.Services;
using Abp.EntityFrameworkCore.Repositories;
using Abp.UI;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using ReadIraq.Domain.Attachments;
using ReadIraq.Domain.AttributeChoices;
using ReadIraq.Domain.Cities;
using ReadIraq.Domain.Companies;
using ReadIraq.Domain.Companies.Dto;
using ReadIraq.Domain.CompanyBranches.Dto;
using ReadIraq.Domain.RequestForQuotations;
using ReadIraq.Domain.Reviews;
using ReadIraq.Domain.Reviews.Dto;
using ReadIraq.Domain.SourceTypes;
using ReadIraq.Domain.TimeWorks;
using ReadIraq.Domain.TimeWorks.Dtos;
using ReadIraq.Localization.SourceFiles;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading.Tasks;

namespace ReadIraq.Domain.CompanyBranches
{
    public class CompanyBranchManager : DomainService, ICompanyBranchManager
    {
        private readonly IRepository<Company> _companyRepository;
        private readonly ICityManager _cityManager;
        private readonly IRepository<CompanyBranch> _companyBranchesRepository;
        private readonly IRepository<CompanyContact> _companyContactRepository;
        private readonly IMapper _mapper;
        private readonly IRepository<Review> _reviewRepository;
        private readonly IRepository<CompanyTranslation> _companyTranslationsRepository;
        private readonly IAttachmentManager _attachmentManager;
        private readonly ICompanyManager _companyManager;
        private readonly IRequestForQuotationManager _requestForQuotationManager;
        private readonly ISourceTypeManager _sourceTypeManager;
        private readonly IRepository<TimeWork> _timeWorkRepository;
        private readonly IAttributeChoiceManger _attributeChoiceManger;
        public CompanyBranchManager(IRepository<CompanyBranch> companyBranchesRepository,
            IRepository<CompanyTranslation> companyTranslationsRepository,
            IAttachmentManager attachmentManager,
            ICompanyManager companyManager

            , IRepository<CompanyContact> companyContactRepository,
            IMapper mapper,
            IRepository<Review> reviewRepository,
            IRepository<Company> companyRepository,
            ICityManager cityManager,
            IRequestForQuotationManager requestForQuotationManager,
            ISourceTypeManager sourceTypeManager,
            IRepository<TimeWork> timeWorkRepository,
            IAttributeChoiceManger attributeChoiceManger)
        {
            _companyBranchesRepository = companyBranchesRepository;
            _companyContactRepository = companyContactRepository;
            _mapper = mapper;
            _companyRepository = companyRepository;
            _cityManager = cityManager;
            _companyTranslationsRepository = companyTranslationsRepository;
            _attachmentManager = attachmentManager;
            _companyManager = companyManager;
            _requestForQuotationManager = requestForQuotationManager;
            _reviewRepository = reviewRepository;
            _sourceTypeManager = sourceTypeManager;
            _timeWorkRepository = timeWorkRepository;
            _attributeChoiceManger = attributeChoiceManger;
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

            if (!await _companyBranchesRepository.GetAll().AnyAsync(x => x.Id == companyBranchId))
            {
                throw new UserFriendlyException(404, Exceptions.ObjectWasNotFound, Tokens.CompanyBranch + " " + companyBranchId.ToString());

            }
            return true;
        }

        public async Task<List<CompanyBranch>> GetCompanyBranchesByCompanyId(int companyId)
        {
            return await _companyBranchesRepository.GetAll().Where(x => x.CompanyId == companyId).ToListAsync();
        }

        public async Task<List<CompanyBranchDetailsDto>> GetCompanyBranchesDtoByCompanyId(int companyId)
        {
            var compnayBranches = await _companyBranchesRepository.GetAll()
                .AsNoTrackingWithIdentityResolution()
                .Include(x => x.Translations)
                .Include(x => x.CompanyContact)
                .Include(x => x.Region).ThenInclude(x => x.Translations)
                .Include(x => x.User)
                .Where(x => x.CompanyId == companyId)
                .ToListAsync();
            return ObjectMapper.Map(compnayBranches, new List<CompanyBranchDetailsDto>());
        }
        public async Task<CompanyBranchDetailsDto> GetCompanyBranchDtoById(int companyBranchId)
        {
            var compnayBranche = await _companyBranchesRepository.GetAll()
                .AsNoTrackingWithIdentityResolution()
                .Include(x => x.AvailableCities).ThenInclude(x => x.Translations).Include(x => x.AvailableCities).ThenInclude(x => x.Country).ThenInclude(x => x.Translations)
                .Include(x => x.Translations)
                .Include(x => x.CompanyContact)
                .Include(x => x.Region).ThenInclude(x => x.Translations)
                .Include(x => x.User)
                .Include(x => x.Company).ThenInclude(x => x.Translations)
                .Where(x => x.Id == companyBranchId)
                .FirstOrDefaultAsync();
            if (compnayBranche is null)
            {
                throw new UserFriendlyException(404, Exceptions.ObjectWasNotFound, Tokens.CompanyBranch);
            }
            return ObjectMapper.Map(compnayBranche, new CompanyBranchDetailsDto());
        }
        public async Task<CompanyBranch> GetSuperLiteEntityByIdAsync(int companyBranchId)
        {
            return await _companyBranchesRepository.GetAsync(companyBranchId);
        }
        public async Task<CompanyBranch> GetLiteEntityByIdAsync(int companyBranchId)
        {
            return await _companyBranchesRepository.GetAllIncluding(x => x.Translations).Where(x => x.Id == companyBranchId).FirstOrDefaultAsync();
        }
        public async Task<CompanyBranch> GetEntityByIdAsync(int companyBranchId)
        {
            return await _companyBranchesRepository.GetAll()
                .Include(x => x.Translations)
                .Include(x => x.AvailableCities)
                .Include(x => x.services)
                .Include(x => x.CompanyContact)
                .Include(x => x.Region)
                .Include(x => x.Company)
                .ThenInclude(c => c.Translations).Where(x => x.Id == companyBranchId).FirstOrDefaultAsync();
        }

        public async Task<int> GetCompnayBranchIdByUserId(long userId)
        {
            return _companyBranchesRepository.GetAll().Where(x => x.UserId == userId).Select(x => x.Id).FirstOrDefault();
        }

        public async Task<List<int>> GetCompanyBranchIdsThatContainsSameCitiesInRequest(long requestId)
        {
            var cityIdsForRequest = await _requestForQuotationManager.GetCityIdsForRequest(requestId);
            var companyBranchesIds = await _companyBranchesRepository.GetAll()
                .AsNoTracking()
                .Include(x => x.AvailableCities)
                .Where(x => x.AvailableCities.Any(x => cityIdsForRequest.Contains(x.Id))).Select(x => x.Id).ToListAsync();
            return companyBranchesIds;
        }

        public async Task<bool> UpdateCitiesForCompanyBranchAsync(List<int> citiesIds, CompanyBranch companyBranch)
        {
            try
            {

                if (!citiesIds.IsNullOrEmpty())
                {
                    var oldCities = companyBranch.AvailableCities.ToList();
                    var newCities = await _cityManager.CheckAndGetCitiesById(citiesIds);

                    var cityToDelete = oldCities.Except(newCities).ToList();
                    foreach (var city in cityToDelete)
                    {
                        companyBranch.AvailableCities.Remove(city);
                    }
                    foreach (var city in newCities)
                    {
                        if (!companyBranch.AvailableCities.Contains(city))
                            companyBranch.AvailableCities.Add(city);
                    }
                }
                if (citiesIds.IsNullOrEmpty() || citiesIds.Count() == 0)
                    companyBranch.AvailableCities.Clear();

                await UnitOfWorkManager.Current.SaveChangesAsync();
                return true;
            }
            catch (Exception ex) { throw; }
        }

        public async Task<GeneralRatingDto> GetGeneralRatingDtoForComapnyBranch(int companyBranchId)
        {

            var result = await _reviewRepository
                 .GetAll().Include(x => x.Offer).ThenInclude(x => x.SelectedCompanies)
                 .AsNoTracking()
                 .Where(x => x.Offer.SelectedCompanies.CompanyBranchId.Value == companyBranchId)
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

        public async Task<List<ReviewDetailsDto>> GetReviewsForCompanyBranch(int companyBranchId)
        {

            var result = await _reviewRepository.GetAll().Include(x => x.Offer).ThenInclude(x => x.SelectedCompanies).Include(x => x.User)
                .AsNoTracking()
                .Where(x => x.Offer.SelectedCompanies.CompanyBranchId.Value == companyBranchId)
                .Select(x => _mapper.Map<ReviewDetailsDto>(x)).ToListAsync();

            return result;
        }


        //public async Task GiftPointToCompanyBranch(CompanyBranch companyBranch)
        //{
        //    var pointsToGift = await SettingManager.GetSettingValueAsync<int>(AppSettingNames.NumberOfPointsToGitft);
        //    companyBranch.NumberOfGiftedPoints = +pointsToGift;
        //    await _companyBranchesRepository.UpdateAsync(companyBranch);
        //}

        public async Task<CompanyBranch> GetCompanyBranchEntityById(int companyBranchId)
        {
            var companyBranch = await _companyBranchesRepository.GetAsync(companyBranchId);
            if (companyBranch is null)
                throw new UserFriendlyException(Exceptions.ObjectWasNotFound, Tokens.CompanyBranch);
            return companyBranch;
        }

        public async Task GetPointFromCompanyBranchForGettingContactRequest(int companyBranchId, int pointsToGetRequest)
        {
            var companyBranch = await GetCompanyBranchEntityById(companyBranchId);
            if (pointsToGetRequest > (companyBranch.NumberOfGiftedPoints + companyBranch.NumberOfPaidPoints))
                throw new UserFriendlyException(515, Exceptions.YouDonotHAveMuchPoints);
            if (pointsToGetRequest <= companyBranch.NumberOfGiftedPoints)
            {
                // Deduct points from giftedPoints
                companyBranch.NumberOfGiftedPoints -= pointsToGetRequest;
            }
            else
            {
                // Deduct all available giftedPoints
                pointsToGetRequest -= companyBranch.NumberOfGiftedPoints;
                companyBranch.NumberOfGiftedPoints = 0;

                // If there are still points to deduct, deduct from paidPoints
                if (pointsToGetRequest <= companyBranch.NumberOfPaidPoints)
                {
                    // Deduct points from paidPoints
                    companyBranch.NumberOfPaidPoints -= pointsToGetRequest;
                }
            }
            await _companyBranchesRepository.UpdateAsync(companyBranch);
            await UnitOfWorkManager.Current.SaveChangesAsync();
        }
        public async Task AddPaidPointsToCompanyBrnach(int numberOfPoint, int companyBranchId)
        {
            var companyBranch = await _companyBranchesRepository
                .GetAll().Where(x => x.Id == companyBranchId).FirstOrDefaultAsync();
            if (companyBranch == null)
                throw new EntityNotFoundException(typeof(CompanyBranch), companyBranch);

            companyBranch.NumberOfPaidPoints = companyBranch.NumberOfPaidPoints + numberOfPoint;
            await _companyBranchesRepository.UpdateAsync(companyBranch);
        }

        public async Task<int> GetCompanyBranchesCount()
        {
            return await _companyBranchesRepository.GetAll().AsNoTracking().Where(x => x.IsDeleted == false).CountAsync();
        }

        public async Task GiftPointToCompanyBranch(int companyBranchId, int sourceTypeId, List<int> choiceIds)
        {
            var companyBranch = await GetCompanyBranchEntityById(companyBranchId);
            int pointsForGiftToCompany = await _sourceTypeManager.GetPointsToGiftCompanyBySourceTypeIdAsync(sourceTypeId);
            if (pointsForGiftToCompany == 111111111)
                pointsForGiftToCompany = await _attributeChoiceManger.GetPointsToGiftToCompanyByAttributeChoices(choiceIds);
            companyBranch.NumberOfGiftedPoints += pointsForGiftToCompany;
            companyBranch.NumberOfTransfers += 1;
            await _companyBranchesRepository.UpdateAsync(companyBranch);
        }

        public async Task<List<int>> FilterCompanyBranchIdsThatOnlyAcceptPossibleRequest(List<int> companyBranchIds)
        {
            return await _companyBranchesRepository.GetAll()
                 .AsNoTracking()
                 .Where(x => x.AcceptPossibleRequests == true && companyBranchIds.Contains(x.Id))
                 .Select(x => x.Id)
                 .ToListAsync();
        }

        public async Task CheckIfCompanyBranchIsFeature(int companyBranchId)
        {
            if (await _companyBranchesRepository.GetAll().AnyAsync(x => x.Id == companyBranchId && x.IsFeature))
                throw new UserFriendlyException(Exceptions.ObjectIsAlreadyExist, Tokens.Company);
        }

        public async Task MakeCompanyBranchAsFeature(int numberInMonths, int companyBranchId)
        {
            var companyBranch = await _companyBranchesRepository.GetAsync(companyBranchId);
            companyBranch.StartFeatureSubscribtionDate = DateTime.UtcNow;
            companyBranch.EndFeatureSubscribtionDate = DateTime.UtcNow.AddMonths(numberInMonths);
            companyBranch.IsFeature = true;
            await _companyBranchesRepository.UpdateAsync(companyBranch);
            await UnitOfWorkManager.Current.SaveChangesAsync();
        }

        public async Task<int> GetcompanyIdByCompanyBranchIdAsync(int companyBranchId)
        {
            return await _companyBranchesRepository.GetAll().AsNoTracking()
                  .Where(x => x.Id == companyBranchId)
                  .Select(x => x.CompanyId.Value)
                  .FirstOrDefaultAsync();
        }

        public async Task MakeAllCompanyBranchesNotFeatureIfTimeEndedAsync()
        {
            await _companyBranchesRepository.GetAll()
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
            //    await _companyBranchesRepository.UpdateAsync(item);
            //}
            await UnitOfWorkManager.Current.SaveChangesAsync();
        }

        public async Task CheckIfCompanyBranchHasTimeWorkThenDeleteIt(int companyBranchId)
        {
            await _timeWorkRepository.GetAll().Where(x => x.CompanyBranchId == companyBranchId).ExecuteDeleteAsync();
        }

        public async Task InsertNewTimeWorksForCompanyBranch(List<TimeWork> timeWorks)
        {
            await _timeWorkRepository.InsertRangeAsync(timeWorks);
        }

        public async Task<List<TimeOfWorkDto>> GetTimeWorksDtoForCompanyBranch(int companyBranchId)
        {
            return ObjectMapper.Map(await _timeWorkRepository.GetAll().AsNoTracking().Where(x => x.CompanyBranchId.HasValue && x.CompanyBranchId.Value == companyBranchId).ToListAsync(), new List<TimeOfWorkDto>());
        }

        public async Task<CompanyBranch> GetCompanyBranchByUserIdAsync(long userId)
        {
            return await _companyBranchesRepository.GetAll()
                 .Where(x => x.UserId.Value == userId)
                 .FirstOrDefaultAsync();
        }

        public async Task UpdateBranchAsync(CompanyBranch companyBranch)
        {
            await _companyBranchesRepository.UpdateAsync(companyBranch);
            await UnitOfWorkManager.Current.SaveChangesAsync();
        }

        public async Task<bool> CheckIfCompanyHasBranches(int companyId)
        {
            return await _companyBranchesRepository.GetAll()
                  .AsNoTrackingWithIdentityResolution()
                  .AnyAsync(x => x.CompanyId == companyId);
        }
    }
}
