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
using ReadIraq.Domain.Offers.Dto;
using ReadIraq.Domain.RejectReasons.Dto;
using ReadIraq.Domain.RequestForQuotations;
using ReadIraq.Domain.RequestForQuotations.Dto;
using ReadIraq.Domain.services;
using ReadIraq.Localization.SourceFiles;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static ReadIraq.Enums.Enum;

namespace ReadIraq.Domain.Offers
{
    public class OfferManager : DomainService, IOfferManager
    {
        private readonly IRepository<Offer, Guid> _offerRepository;
        private readonly CompanyBranchManager _companyBranchManager;
        private readonly ServiceManager _serviceManager;
        private readonly IMapper _mapper;
        private readonly IRequestForQuotationManager _requestForQuotationManager;
        private readonly CompanyManager _companyManager;
        private readonly UserManager _userManager;
        public OfferManager(IRepository<Offer, Guid> offerRepository,
            CompanyBranchManager companyBranchManager,
            ServiceManager serviceManager,
            IMapper mapper,
            IRequestForQuotationManager requestForQuotationManager,
            CompanyManager companyManager,
            UserManager userManager
      )
        {
            _offerRepository = offerRepository;
            _companyBranchManager = companyBranchManager;
            _serviceManager = serviceManager;
            _mapper = mapper;
            _requestForQuotationManager = requestForQuotationManager;
            _companyManager = companyManager;
            _userManager = userManager;
        }

        public async Task CheckIfEntityExict(Guid id)
        {
            if (!await _offerRepository.GetAll().AsNoTracking().AnyAsync(x => x.Id == id))
                throw new UserFriendlyException(404, Exceptions.ObjectWasNotFound, Tokens.Offer);
        }

        public async Task<Offer> GetFullEntityByIdAsync(Guid id)
        {
            var entity = await _offerRepository.GetAll()
                .Include(x => x.ServiceValueForOffers)
                .Include(x => x.SelectedCompanies)
                .ThenInclude(x => x.RequestForQuotation)
                .ThenInclude(x => x.User)
                .Where(x => x.Id == id).FirstOrDefaultAsync();
            if (entity == null)
                throw new UserFriendlyException(404, Exceptions.ObjectWasNotFound, Tokens.Offer);
            return entity;
        }
        public async Task<List<Offer>> GetFullEntitiesByIdAsync(List<Guid> ids)
        {
            return await _offerRepository.GetAll()
                            .Include(x => x.SelectedCompanies)
                            .ThenInclude(x => x.RequestForQuotation)
                            .ThenInclude(x => x.User)
                            .Where(x => ids.Contains(x.Id)).ToListAsync();
        }
        public async Task<Offer> GetEntityByIdAsync(Guid id)
        {
            var entity = await _offerRepository.GetAll()
                .Include(x => x.SelectedCompanies)
                .ThenInclude(x => x.RequestForQuotation)
                .Where(x => x.Id == id).FirstOrDefaultAsync();
            if (entity == null)
                throw new UserFriendlyException(404, Exceptions.ObjectWasNotFound, Tokens.Offer);
            return entity;
        }

        public async Task<OfferDetailsDto> GetOfferDetailsDtoById(Guid id, long userId)
        {
            try
            {
                var offer = await _offerRepository.GetAll().AsNoTracking()
                    .Include(x => x.SelectedCompanies)
                    .Include(x => x.ServiceValueForOffers)
                    .Include(x => x.RejectReason)
                    .ThenInclude(x => x.Translations)
                    .Where(x => x.Id == id).FirstOrDefaultAsync();
                if (offer == null)
                    throw new UserFriendlyException(404, Exceptions.ObjectWasNotFound, Tokens.Offer);
                var user = await _userManager.GetUserByIdAsync(userId);
                var serviceDetails = await _serviceManager.GetFullServicesForOffer(offer.ServiceValueForOffers.ToList());
                offer.ServiceValueForOffers = null;
                var offerDetails = _mapper.Map<OfferDetailsDto>(offer);
                offerDetails.ServiceValueForOffers = serviceDetails;
                if (offer.SelectedCompanies.CompanyId.HasValue)
                {
                    var selectedCompany = await _companyManager.GetLiteEntityByIdAsync(offer.SelectedCompanies.CompanyId.Value);
                    if (selectedCompany is not null)
                    {
                        offerDetails.SelectedCompanies.Company = _mapper.Map<CompanyDto>(selectedCompany);
                        offerDetails.SelectedCompanies.Company.GeneralRating = await _companyManager.GetGeneralRatingDtoForComapny(offer.SelectedCompanies.CompanyId.Value);
                        offerDetails.SelectedCompanies.Company.Reviews = await _companyManager.GetReviewsForCompany(offer.SelectedCompanies.CompanyId.Value);
                    }
                }
                if (offer.SelectedCompanies.CompanyBranchId.HasValue)
                {
                    var companybranch = await _companyBranchManager.GetEntityByIdAsync(offer.SelectedCompanies.CompanyBranchId.Value);
                    if (companybranch is not null)
                    {
                        offerDetails.SelectedCompanies.CompanyBranch = _mapper.Map<CompanyBranchDto>(companybranch);
                        offerDetails.SelectedCompanies.CompanyBranch.GeneralRating = await _companyBranchManager.GetGeneralRatingDtoForComapnyBranch(offer.SelectedCompanies.CompanyBranchId.Value);
                        offerDetails.SelectedCompanies.CompanyBranch.Reviews = await _companyBranchManager.GetReviewsForCompanyBranch(offer.SelectedCompanies.CompanyBranchId.Value);
                    }
                }
                offerDetails.SelectedCompanies.RequestForQuotation = await _requestForQuotationManager.GetRequestForQuotationDtoById(offer.SelectedCompanies.RequestForQuotationId);
                bool showContact = false;
                if (offerDetails.SelectedCompanies.RequestForQuotation.Statues is (RequestForQuotationStatues.InProcess) && userId == offerDetails.SelectedCompanies.RequestForQuotation.User.Id)
                {
                    if (DateTime.UtcNow.AddHours(48) >= offerDetails.SelectedCompanies.RequestForQuotation.MoveAtUtc)
                        showContact = true;
                }
                if (!showContact && offerDetails.SelectedCompanies.RequestForQuotation.Statues is not
                    (RequestForQuotationStatues.FinishByCompany
                    or RequestForQuotationStatues.Finished
                    or RequestForQuotationStatues.NotFinishByUser)
                    && user.Type is (UserType.BasicUser or UserType.MediatorUser)
                    )
                {
                    if (offerDetails.SelectedCompanies.CompanyBranch is not null)
                    {
                        offerDetails.SelectedCompanies.CompanyBranch.CompanyContact = null;
                    }
                    if (offerDetails.SelectedCompanies.Company is not null)
                    {
                        offerDetails.SelectedCompanies.Company.CompanyContact = null;
                    }
                }
                if (offer.RejectReasonId.HasValue)
                {
                    offerDetails.RejectReasonAndDescription = new RejectReasonAndDescriptionForOffer()
                    {
                        RejectReason = _mapper.Map<RejectReasonDetailsDto>(offer.RejectReason),
                        RejectReasonDescription = offer.RejectReasonDescription
                    };
                }

                return offerDetails;
            }
            catch (Exception e)
            {

                throw;
            }


        }

        public async Task<long> GetUserIdForRequestIntoOfferByOfferId(Guid id)
        {
            return await _offerRepository.GetAll().AsNoTracking()
                              .Include(x => x.SelectedCompanies)
                              .ThenInclude(x => x.RequestForQuotation)
                              .Where(x => x.Id == id)
                              .Select(x => x.SelectedCompanies.RequestForQuotation.UserId)
                              .FirstOrDefaultAsync();
        }

        public async Task<List<Offer>> GetOffersWhichSentToUser(List<Guid> OffersIds, long userId)
        {
            var offers = await _offerRepository.GetAll()
                .Include(x => x.SelectedCompanies)
               .ThenInclude(x => x.RequestForQuotation)
               .Where(x => OffersIds.Contains(x.Id) && x.Statues == OfferStatues.Approved && x.SelectedCompanies.RequestForQuotation.UserId == userId).ToListAsync();
            return offers;
        }



        public async Task MakeOtherOffersRejectedWhenUserTakeOffer(Guid offerId, long requestId)
        {
            var offers = await _offerRepository
                .GetAll()
                .Include(x => x.SelectedCompanies)
                .Where(x => x.SelectedCompanies.RequestForQuotationId == requestId && x.Id != offerId)
                .ToListAsync();
            offers.ForEach(x => x.Statues = OfferStatues.RejectedByUser);
            await UnitOfWorkManager.Current.SaveChangesAsync();
        }

        public async Task<bool> CheckIfCompanyOfferHasApprovedWithThisRequest(int companyId, long requestId)
        {
            return await _offerRepository
                 .GetAll().AsNoTrackingWithIdentityResolution()
                 .Include(x => x.SelectedCompanies)
                 .AnyAsync(x => x.Statues == OfferStatues.SelectedByUser && x.SelectedCompanies.CompanyId.Value == companyId && x.SelectedCompanies.RequestForQuotationId == requestId);
        }
        public async Task<bool> CheckIfCompanyBranchOfferHasApprovedWithThisRequest(int companyBranchId, long requestId)
        {
            return await _offerRepository
                 .GetAll().AsNoTrackingWithIdentityResolution()
                 .Include(x => x.SelectedCompanies)
                 .AnyAsync(x => x.Statues == OfferStatues.SelectedByUser && x.SelectedCompanies.CompanyBranchId.Value == companyBranchId && x.SelectedCompanies.RequestForQuotationId == requestId);
        }

        public async Task<OfferIdAndStatusDto> GetOfferIdByRequestId(long requestId)
        {
            return await _offerRepository.GetAll()
                .AsNoTracking()
                .Include(x => x.SelectedCompanies)
                .Where(x => ((x.Statues == OfferStatues.SelectedByUser) || (x.Statues == OfferStatues.Finished)) && x.SelectedCompanies.RequestForQuotationId == requestId)
                .Select(x => new OfferIdAndStatusDto { SelectedOfferId = x.Id, OfferStatues = x.Statues }).FirstOrDefaultAsync();

        }

        public async Task<List<RequestCompanyCompanyBranchIdsDto>> GetAllSelectedCompaniesWithThisRequest(List<long> requestIds)
        {
            return await _offerRepository.GetAll()
                          .AsNoTracking()
                          .Include(x => x.SelectedCompanies)
                          .Where(x => requestIds.Contains(x.SelectedCompanies.RequestForQuotationId))
                          .Select(x => new RequestCompanyCompanyBranchIdsDto { CompanyId = x.SelectedCompanies.CompanyId, CompanyBranchId = x.SelectedCompanies.CompanyBranchId, RequestId = x.SelectedCompanies.RequestForQuotationId, OfferStatues = x.Statues })
                          .ToListAsync();
        }

        public async Task<Guid> MakeOfferFinishByRequestId(long requestId)
        {
            var offerId = (await GetOfferIdByRequestId(requestId)).SelectedOfferId;
            var offer = await _offerRepository.GetAsync(offerId.Value);
            offer.Statues = OfferStatues.Finished;
            await _offerRepository.UpdateAsync(offer);
            await UnitOfWorkManager.Current.SaveChangesAsync();
            return offer.Id;
        }

        public async Task<OfferIdAndStatusDto> GetOfferIdByUserIdAndRequstIdAsync(long requestId, long userId, UserType userType)
        {
            if (userType is UserType.CompanyUser)
                return await _offerRepository.GetAll().Include(x => x.SelectedCompanies)
                    .Where(x => x.SelectedCompanies.CompanyId == (_companyManager.GetCompnayIdByUserId(userId).GetAwaiter().GetResult()) && x.SelectedCompanies.RequestForQuotationId == requestId)
                   .Select(x => new OfferIdAndStatusDto { SelectedOfferId = x.Id, OfferStatues = x.Statues }).FirstOrDefaultAsync();
            else
                return await _offerRepository.GetAll().Include(x => x.SelectedCompanies)
                     .Where(x => x.SelectedCompanies.CompanyBranchId == (_companyBranchManager.GetCompnayBranchIdByUserId(userId).GetAwaiter().GetResult()) && x.SelectedCompanies.RequestForQuotationId == requestId)
                   .Select(x => new OfferIdAndStatusDto { SelectedOfferId = x.Id, OfferStatues = x.Statues }).FirstOrDefaultAsync();


        }

        public async Task<List<int>> GetCompanyIdsThatSentOffer(long requestId)
        {
            return await _offerRepository
                    .GetAll()
                    .AsNoTrackingWithIdentityResolution()
                    .Include(c => c.SelectedCompanies)
                    .Where(p => p.Provider == OfferProvider.Company && p.SelectedCompanies.RequestForQuotationId == requestId)
                    .Select(x => x.SelectedCompanies.CompanyId.Value).ToListAsync();
        }

        public async Task<List<int>> GetCompanyBranchIdsThatSentOffer(long requestId)
        {
            return await _offerRepository
                              .GetAll()
                              .AsNoTrackingWithIdentityResolution()
                              .Include(c => c.SelectedCompanies)
                              .Where(p => p.Provider == OfferProvider.BranchCompany && p.SelectedCompanies.RequestForQuotationId == requestId)
                              .Select(x => x.SelectedCompanies.CompanyBranchId.Value).ToListAsync();
        }

        public async Task<TinySelectedCompanyDto> GetSelectedCompanyByOfferId(Guid offerId)
        {
            var offer = await _offerRepository.GetAll()
                            .AsNoTracking()
                            .Include(c => c.SelectedCompanies)
                            .Where(x => x.Id == offerId)
                            .FirstOrDefaultAsync();
            if (offer.Provider == OfferProvider.Company)
                return new TinySelectedCompanyDto { Id = offer.SelectedCompanies.CompanyId.Value, Provider = Provider.Company };
            else
                return new TinySelectedCompanyDto { Id = offer.SelectedCompanies.CompanyBranchId.Value, Provider = Provider.CompanyBranch };
        }

        public async Task<List<Offer>> GetAllOffersApprovedWithThisRequest(long requestIds)
        {
            return await _offerRepository.GetAll()
                   .Include(x => x.SelectedCompanies)
                   .Where(x => x.Statues == OfferStatues.Approved && x.SelectedCompanies.RequestForQuotationId == requestIds)
                   .ToListAsync();
        }

        public async Task MakeOffersRejectByUserAsyn(List<Offer> offers)
        {
            foreach (var offer in offers)
            {
                offer.Statues = OfferStatues.RejectedByUser;
            }
            await UnitOfWorkManager.Current.SaveChangesAsync();
        }

        public async Task<List<long>> GetRequestIdsWhichConnectedWithCompanyAndBeenRejectedFromUserForCompany(int companyId)
        {
            return await _offerRepository.GetAll()
                  .AsNoTracking()
                  .Include(x => x.SelectedCompanies)
                  .ThenInclude(x => x.RequestForQuotation)
                  .Where(x => x.SelectedCompanies.CompanyId == companyId && x.Statues == OfferStatues.RejectedByUser && x.SelectedCompanies.RequestForQuotation.Statues == RequestForQuotationStatues.Possible)
                  .Select(x => x.SelectedCompanies.RequestForQuotationId)
                  .ToListAsync();
        }

        public async Task<List<long>> GetRequestIdsWhichConnectedWithCompanyAndBeenRejectedFromUserForCompanyBranch(int companyBranchId)
        {
            return await _offerRepository.GetAll()
                          .AsNoTracking()
                          .Include(x => x.SelectedCompanies)
                          .ThenInclude(x => x.RequestForQuotation)
                          .Where(x => x.SelectedCompanies.CompanyBranchId == companyBranchId && x.Statues == OfferStatues.RejectedByUser && x.SelectedCompanies.RequestForQuotation.Statues == RequestForQuotationStatues.Possible)
                          .Select(x => x.SelectedCompanies.RequestForQuotationId)
                          .ToListAsync();
        }

        public async Task CheckIfRequestConnectWithMediatorThenGiveCommissionForHim(long userId, TinySelectedCompanyDto tinySelected, Guid offerId)
        {
            var code = (await _userManager.GetUserByIdAsync(userId)).MediatorCode;
            //if (!string.IsNullOrWhiteSpace(code))
            //    await _mediatorManager.GiveMediatorCommissionForFinishRequest(code, tinySelected, offerId);
        }

        public async Task<OfferStatues?> CheckIfCompanyHasAnyOfferNeedToProcess(int companyId)
        {
            return await _offerRepository.GetAll()
                    .AsNoTrackingWithIdentityResolution()
                    .Include(x => x.SelectedCompanies)
                    .Where(x => x.SelectedCompanies.CompanyId == companyId && (x.Statues == OfferStatues.Approved || x.Statues == OfferStatues.SelectedByUser))
                    .Select(x => x.Statues)
                    .FirstOrDefaultAsync();
        }
        public async Task<OfferStatues?> CheckIfCompanyBranchHasAnyOfferNeedToProcess(int companyBranchId)
        {
            return await _offerRepository.GetAll()
                    .AsNoTrackingWithIdentityResolution()
                    .Include(x => x.SelectedCompanies)
                    .Where(x => x.SelectedCompanies.CompanyBranchId == companyBranchId && (x.Statues == OfferStatues.Approved || x.Statues == OfferStatues.SelectedByUser))
                    .Select(x => x.Statues)
                    .FirstOrDefaultAsync();
        }

        public async Task DeleteAllUnNeededOffers(int companyId, bool forCompany = true)
        {
            if (forCompany)
                await _offerRepository.GetAll()
                       .Include(x => x.SelectedCompanies)
                       .Where(x => x.SelectedCompanies.CompanyId == companyId && (x.Statues == OfferStatues.Checking || x.Statues == OfferStatues.RejectedNeedToEdit || x.Statues == OfferStatues.Rejected || x.Statues == OfferStatues.RejectedByUser))
                       .ExecuteUpdateAsync(s => s.SetProperty(x => x.IsDeleted, true));
            else
                await _offerRepository.GetAll()
                  .Include(x => x.SelectedCompanies)
                  .Where(x => x.SelectedCompanies.CompanyBranchId == companyId && (x.Statues == OfferStatues.Checking || x.Statues == OfferStatues.RejectedNeedToEdit || x.Statues == OfferStatues.Rejected || x.Statues == OfferStatues.RejectedByUser))
                  .ExecuteUpdateAsync(s => s.SetProperty(x => x.IsDeleted, true));
            await UnitOfWorkManager.Current.SaveChangesAsync();

        }
    }
}
