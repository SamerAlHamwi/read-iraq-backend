using Abp.Domain.Services;
using ReadIraq.Domain.Offers.Dto;
using ReadIraq.Domain.RequestForQuotations.Dto;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using static ReadIraq.Enums.Enum;

namespace ReadIraq.Domain.Offers
{
    public interface IOfferManager : IDomainService
    {
        Task CheckIfEntityExict(Guid id);
        Task<Offer> GetFullEntityByIdAsync(Guid id);
        Task<List<Offer>> GetFullEntitiesByIdAsync(List<Guid> ids);
        Task<long> GetUserIdForRequestIntoOfferByOfferId(Guid id);
        Task<OfferDetailsDto> GetOfferDetailsDtoById(Guid id, long userId);
        Task<Offer> GetEntityByIdAsync(Guid id);
        Task<List<Offer>> GetOffersWhichSentToUser(List<Guid> OffersIds, long userId);
        Task MakeOtherOffersRejectedWhenUserTakeOffer(Guid offerId, long requestId);
        Task<bool> CheckIfCompanyOfferHasApprovedWithThisRequest(int companyId, long requestId);
        Task<bool> CheckIfCompanyBranchOfferHasApprovedWithThisRequest(int companyBranchId, long requestId);
        Task<OfferIdAndStatusDto> GetOfferIdByRequestId(long requestId);
        Task<List<RequestCompanyCompanyBranchIdsDto>> GetAllSelectedCompaniesWithThisRequest(List<long> requestIds);
        Task<List<Offer>> GetAllOffersApprovedWithThisRequest(long requestIds);
        Task<Guid> MakeOfferFinishByRequestId(long requestId);
        Task<OfferIdAndStatusDto> GetOfferIdByUserIdAndRequstIdAsync(long requestId, long userId, UserType userType);
        Task<List<int>> GetCompanyIdsThatSentOffer(long requestId);
        Task<List<int>> GetCompanyBranchIdsThatSentOffer(long requestId);
        Task<TinySelectedCompanyDto> GetSelectedCompanyByOfferId(Guid offerId);
        Task MakeOffersRejectByUserAsyn(List<Offer> offers);
        Task<List<long>> GetRequestIdsWhichConnectedWithCompanyAndBeenRejectedFromUserForCompany(int companyId);
        Task<List<long>> GetRequestIdsWhichConnectedWithCompanyAndBeenRejectedFromUserForCompanyBranch(int companyBranchId);
        Task CheckIfRequestConnectWithMediatorThenGiveCommissionForHim(long userId, TinySelectedCompanyDto tinySelected, Guid offerId);
        Task<OfferStatues?> CheckIfCompanyHasAnyOfferNeedToProcess(int companyId);
        Task<OfferStatues?> CheckIfCompanyBranchHasAnyOfferNeedToProcess(int companyBranchId);
        Task DeleteAllUnNeededOffers(int companyId, bool forCompany = true);
    }
}
