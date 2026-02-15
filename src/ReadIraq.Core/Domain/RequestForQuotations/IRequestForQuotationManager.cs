using Abp.Domain.Services;
using ReadIraq.Domain.AttributeChoices;
using ReadIraq.Domain.RequestForQuotationContacts.Dto;
using ReadIraq.Domain.RequestForQuotations.Dto;
using ReadIraq.Domain.SearchedPlacesByUsers.Dtos;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ReadIraq.Domain.RequestForQuotations
{
    public interface IRequestForQuotationManager : IDomainService
    {
        //Task InsertSelectedCompaniesByUser(RequestForQuotation requestForQuotation, List<Company> companies);
        Task<List<LiteAttachmentDto>> GetAttachmentThatOnlyForRequest(long requestId, List<long> attachmentIds);
        Task<List<LiteAttachmentDto>> GetFinishedAttachmentForRequestByCompany(long requestId);
        Task<RequestForQuotationDto> GetRequestForQuotationDtoById(long id);
        Task<List<int>> GetCityIdsForRequest(long requestId);
        Task<bool> CheckIfThereIsRequestBelongsToSourceType(int SourceTypeId);
        Task<bool> CheckIfThereIsRequestBelongsToAttribute(int AttributeId);
        Task<bool> CheckIfThereIsRequestBelongsToAttributeChoice(AttributeChoice attributeChoice);
        Task<RequestForQuotation> GetEntityById(long id);
        Task UpdateAttacmentForRequestForQuotation(long requestId, List<AttributeChoiceAndAttachmentDto> AttributeChoiceAndAttachments);
        Task UpdateContcatRequestForQuotation(List<CreateRequestForQuotationContactDto> requestForQuotationContacts, RequestForQuotation request);
        Task MakeRequestAsPossible(RequestForQuotation requestForQuotation);
        Task MakeRequestCancelledByUserAfterRekectedAllOffers(RequestForQuotation requestForQuotation);
        Task MakeRequestHasOffers(RequestForQuotation requestForQuotation);
        Task MakeRequestInProcessAfterUserTakeOffer(RequestForQuotation request);
        Task CheckIfEntityExict(long id);
        Task<RequestsStatisticalNumbersDto> GetCountNumberAboutRequestsForQuotation(RequestStatisticalInputDto input);
        Task<List<CitiesStatisticsForRequestsDto>> GetCitiesStatisticsForRequestsDto(InputCitiesStatisticsForRequestsDto input);
        Task<List<long>> GetRequestIdsWhichSubmitededByRigesteredUserViaBroker(long userId);
        Task<bool> InserNewSearchedPlaceForUser(SearchedPlacesByUserDto input, long userId);
        Task<List<SearchedPlacesByUserDto>> GetAllSearchedPlacesByUser(long userId);
        Task<List<RequestForQuotation>> GetAllRequestHadOffersAndUserDidnotMakeAnyChangeForAutoRejectItAsync();
        Task<List<long>> GetRequestIdsForUsersByBrokerId(int mediatorId);
        Task CheckIfPlaceIdExistForThisUserAndDeleteIt(long userId, string placeId);
        Task<int> GetCountOfFinishedRequestUsersIds(List<long> usersIds);
        Task<List<RequestForQuotation>> GetAllPossibleRequestWithOutDate();
        Task MakeAllPossibleRequestsAfterCustomTimeOutOfPossible();
        Task DeleteAllRequestForUserIfFound(long userId);
        Task<RequestForQuotation> GetRequestWithAttributeValues(long requestId);


    }
}
