using Abp.Configuration;
using Abp.Domain.Repositories;
using Abp.Domain.Services;
using Abp.EntityFrameworkCore.Repositories;
using Abp.UI;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using ReadIraq.Authorization.Users;
using ReadIraq.Configuration;
using ReadIraq.Domain.Attachments;
using ReadIraq.Domain.AttributeAndAttachments;
using ReadIraq.Domain.AttributeChoices;
using ReadIraq.Domain.AttributeForSourcTypeValues;
using ReadIraq.Domain.Cities;
using ReadIraq.Domain.Mediators.Mangers;
using ReadIraq.Domain.RequestForQuotationContacts;
using ReadIraq.Domain.RequestForQuotationContacts.Dto;
using ReadIraq.Domain.RequestForQuotations.Dto;
using ReadIraq.Domain.SearchedPlacesByUsers;
using ReadIraq.Domain.SearchedPlacesByUsers.Dtos;
using ReadIraq.Localization.SourceFiles;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static ReadIraq.Enums.Enum;

namespace ReadIraq.Domain.RequestForQuotations
{
    public class RequestForQuotationManager : DomainService, IRequestForQuotationManager
    {
        private readonly IRepository<RequestForQuotation, long> _repository;
        private readonly IRepository<AttributeForSourceTypeValue, int> _attributeForSourceTypeValueRepository;
        private readonly IAttachmentManager _attachmentManager;
        private readonly IRepository<AttributeChoiceAndAttachment> _attributeChoiceAndAttachmentsRepository;
        private readonly IRepository<RequestForQuotationContact, long> _requestForQuotationContactRepository;
        private readonly IMapper _mapper;
        private readonly UserManager _userManager;
        private readonly CityManager _cityManager;
        private readonly IMediatorManager _mediatorManager;
        private readonly IRepository<SearchedPlacesByUser> _searchedPlaceByUserRepository;

        public RequestForQuotationManager(IRepository<RequestForQuotation, long> repository,
            IRepository<AttributeForSourceTypeValue, int> attributeForSourceTypeValueRepository,
            IRepository<AttributeChoiceAndAttachment> attributeChoiceAndAttachmentsRepository,
            IRepository<RequestForQuotationContact, long> requestForQuotationContactRepository,
            IMapper mapper,
            UserManager userManager,
            CityManager cityManager,
            IMediatorManager mediatorManager,
            IAttachmentManager attachmentManager,
            IRepository<SearchedPlacesByUser> searchedPlaceByUserRepository)
        {
            _repository = repository;
            _attributeForSourceTypeValueRepository = attributeForSourceTypeValueRepository;
            _attachmentManager = attachmentManager;
            _attributeChoiceAndAttachmentsRepository = attributeChoiceAndAttachmentsRepository;
            _requestForQuotationContactRepository = requestForQuotationContactRepository;
            _mapper = mapper;
            _userManager = userManager;
            _cityManager = cityManager;
            _mediatorManager = mediatorManager;
            _searchedPlaceByUserRepository = searchedPlaceByUserRepository;
        }

        public async Task<List<LiteAttachmentDto>> GetAttachmentThatOnlyForRequest(long requestId, List<long> attachmentIds)
        {

            var allAttachmentForRequest = await _attachmentManager.GetByRefAsync(requestId, AttachmentRefType.RequestForQuotation);
            var attachmentJustForRequest = allAttachmentForRequest.Where(x => !attachmentIds.Contains(x.Id)).ToList();
            var result = new List<LiteAttachmentDto>();
            foreach (var attachment in attachmentJustForRequest)
            {
                result.Add(new LiteAttachmentDto
                {
                    Url = _attachmentManager.GetUrl(attachment),
                    Id = attachment.Id,
                    LowResolutionPhotoUrl = _attachmentManager.GetLowResolutionPhotoUrl(attachment)
                });
            }
            return result;
        }
        public async Task<List<LiteAttachmentDto>> GetFinishedAttachmentForRequestByCompany(long requestId)
        {
            var attachmentsForFinishedRequest = await _attachmentManager.GetByRefAsync(requestId, AttachmentRefType.FinishedRequestByCompany);
            var result = new List<LiteAttachmentDto>();
            foreach (var attachment in attachmentsForFinishedRequest)
            {
                result.Add(new LiteAttachmentDto
                {
                    Url = _attachmentManager.GetUrl(attachment),
                    Id = attachment.Id,
                    LowResolutionPhotoUrl = _attachmentManager.GetLowResolutionPhotoUrl(attachment)
                });
            }
            return result;
        }


        public async Task<List<int>> GetCityIdsForRequest(long requestId)
        {
            var cityIds = await _repository.GetAll()
                .AsNoTracking()
                .Where(x => x.Id == requestId && x.SourceCityId.HasValue && x.DestinationCityId.HasValue).Select(x => new { x.SourceCityId, x.DestinationCityId }).FirstOrDefaultAsync();
            return new List<int>() { cityIds.SourceCityId.Value, cityIds.DestinationCityId.Value };
        }

        public async Task<RequestForQuotationDto> GetRequestForQuotationDtoById(long id)
        {
            var request = await _repository.GetAll().Include(x => x.SourceCity).ThenInclude(x => x.Translations)
                .Include(x => x.DestinationCity).ThenInclude(x => x.Translations).Include(x => x.User).Where(x => x.Id == id).FirstOrDefaultAsync();
            return ObjectMapper.Map<RequestForQuotationDto>(request);
        }

        public async Task<RequestForQuotation> GetEntityById(long id)
        {
            return await _repository.GetAll()
                .Include(x => x.AttributeForSourceTypeValues)
                .Include(x => x.Services)
                .Include(x => x.RequestForQuotationContacts)
                .Where(x => x.Id == id).FirstOrDefaultAsync();
        }


        public async Task UpdateAttacmentForRequestForQuotation(long requestId, List<AttributeChoiceAndAttachmentDto> AttributeChoiceAndAttachments)
        {
            var attrbuiteChoiceAndattachmentIds = AttributeChoiceAndAttachments.Select(x => x.AttributeChoiceId).ToList();
            var oldAttributeChoiceAndAttachments = _attributeChoiceAndAttachmentsRepository.GetAll().Where(x => x.RequestForQuotationId == requestId).ToList();
            var attrbuiteChoiceAndattachmentToDelete = oldAttributeChoiceAndAttachments.Where(x => !attrbuiteChoiceAndattachmentIds.Contains(x.AttributeChoiceId)).ToList();
            var allNewAttachmentsIds = AttributeChoiceAndAttachments.Select(x => x.AttachmentIds).SelectMany(list => list).ToList();
            var currentAttachments = await _attachmentManager.GetByRefAsync(requestId, AttachmentRefType.RequestForQuotation);
            var attachmentsToDelete = currentAttachments.Where(x => !allNewAttachmentsIds.Contains((x.Id))).ToList();

            if (attrbuiteChoiceAndattachmentToDelete.Count > 0 || attachmentsToDelete.Count > 0)
                await DeleteAttachmentsAndAttributeChoices(attrbuiteChoiceAndattachmentToDelete, attachmentsToDelete);

            var attributesAndAttachmentsToAdd = new List<AttributeChoiceAndAttachment>();
            foreach (var item in AttributeChoiceAndAttachments)
            {
                List<Attachment> attachments = new List<Attachment>();
                var imagesattachmentIdsToAdd = item.AttachmentIds.Except(currentAttachments.Select(x => x.Id).ToList());
                foreach (var newAttachmentId in item.AttachmentIds)
                {
                    var attachment = imagesattachmentIdsToAdd.Contains(newAttachmentId)
                    ? await _attachmentManager.CheckAndUpdateRefIdAsync(newAttachmentId, AttachmentRefType.RequestForQuotation, requestId, true, true)
                    : await _attachmentManager.GetByIdAsync(newAttachmentId);
                    attachments.Add(attachment);
                }
                if (item.AttributeChoiceId != null)
                {
                    if (!(oldAttributeChoiceAndAttachments.Select(x => x.AttributeChoiceId).ToList()).Contains(item.AttributeChoiceId.Value))
                    {
                        attributesAndAttachmentsToAdd.Add(new AttributeChoiceAndAttachment()
                        {
                            AttributeChoiceId = item.AttributeChoiceId.Value,
                            RequestForQuotationId = requestId,
                            Attachments = attachments
                        });
                    }
                    else
                    {
                        var attributeChoiceAndAttachmentToUpdate = oldAttributeChoiceAndAttachments.Where(x => x.AttributeChoiceId == item.AttributeChoiceId).FirstOrDefault();
                        attributeChoiceAndAttachmentToUpdate.Attachments = attachments;
                        await _attributeChoiceAndAttachmentsRepository.UpdateAsync(attributeChoiceAndAttachmentToUpdate);
                    }
                }
                else
                    attachments.ForEach(x => x.AttributeChoiceAndAttachmentId = null);
                continue;
            }
            if (attributesAndAttachmentsToAdd is not null && attributesAndAttachmentsToAdd.Count > 0)
                await _attributeChoiceAndAttachmentsRepository.InsertRangeAsync(attributesAndAttachmentsToAdd);

            await UnitOfWorkManager.Current.SaveChangesAsync();
        }

        public async Task UpdateContcatRequestForQuotation(List<CreateRequestForQuotationContactDto> requestForQuotationContacts, RequestForQuotation request)
        {
            var oldRequestContact = request.RequestForQuotationContacts.ToList();
            var newRequestContact = _mapper.Map<List<CreateRequestForQuotationContactDto>, List<RequestForQuotationContact>>(requestForQuotationContacts);
            foreach (var item in newRequestContact)
            {
                if (item.RequestForQuotationContactType == RequestForQuotationContactType.Source)
                    item.Id = oldRequestContact.Where(x => x.RequestForQuotationContactType == RequestForQuotationContactType.Source).Select(x => x.Id).FirstOrDefault();
                else if (item.RequestForQuotationContactType == RequestForQuotationContactType.Destination)
                    item.Id = oldRequestContact.Where(x => x.RequestForQuotationContactType == RequestForQuotationContactType.Destination).Select(x => x.Id).FirstOrDefault();
                item.RequestForQuotationId = request.Id;
                await _requestForQuotationContactRepository.UpdateAsync(item);

            }
            await UnitOfWorkManager.Current.SaveChangesAsync();
        }

        public async Task<bool> CheckIfThereIsRequestBelongsToSourceType(int sourceTypeId)
        {
            var Request = await _repository.GetAll().AnyAsync(x => x.SourceTypeId == sourceTypeId);
            if (Request == true)
                throw new UserFriendlyException(string.Format(Exceptions.ObjectCantBeDelete, Tokens.SourceType));
            return Request;
        }
        public async Task<bool> CheckIfThereIsRequestBelongsToAttribute(int attributeForSourcTypeId)
        {
            var RequestIds = await _attributeForSourceTypeValueRepository.GetAll().AsNoTracking()
                .Where(x => x.IsDeleted == false && x.AttributeForSourcTypeId == attributeForSourcTypeId).Select(x => x.RequestForQuotationId).ToListAsync();

            bool check = await _repository.GetAll().AsNoTracking()
                 .Where(x => x.IsDeleted == false && RequestIds.Contains(x.Id)).AnyAsync();
            if (check == true)
                throw new UserFriendlyException(string.Format(Exceptions.ObjectCantBeDelete, Tokens.AttributeForSourceType));
            return check;
        }
        private async Task DeleteAttachmentsAndAttributeChoices(List<AttributeChoiceAndAttachment> attrbuiteChoiceAndattachmentToDelete, List<Attachment> attachmentsToDelete)
        {
            foreach (var item in attrbuiteChoiceAndattachmentToDelete)
            {
                await _attributeChoiceAndAttachmentsRepository.DeleteAsync(item);
            }
            foreach (var existingAttachment in attachmentsToDelete)
            {
                await _attachmentManager.DeleteRefIdAsync(existingAttachment);
            }
        }

        public async Task<bool> CheckIfThereIsRequestBelongsToAttributeChoice(AttributeChoice attributeChoice)
        {
            var check = await _repository.GetAll().AsNoTracking()
                .Include(x => x.AttributeChoiceAndAttachments)
                .AnyAsync(x => x.AttributeChoiceAndAttachments.Any(x => x.AttributeChoiceId == attributeChoice.Id));
            if (check == true)
                throw new UserFriendlyException(string.Format(Exceptions.ObjectCantBeDelete, Tokens.AttributeChoice));
            return check;
        }

        public async Task MakeRequestAsPossible(RequestForQuotation requestForQuotation)
        {
            requestForQuotation.Statues = RequestForQuotationStatues.Possible;
            requestForQuotation.HadOffersDate = null;
            requestForQuotation.PossibledDate = DateTime.UtcNow;
            await _repository.UpdateAsync(requestForQuotation);
            await UnitOfWorkManager.Current.SaveChangesAsync();
        }
        public async Task MakeRequestCancelledByUserAfterRekectedAllOffers(RequestForQuotation requestForQuotation)
        {
            requestForQuotation.Statues = RequestForQuotationStatues.CanceledAfterRejectOffers;
            requestForQuotation.HadOffersDate = null;
            await _repository.UpdateAsync(requestForQuotation);
            await UnitOfWorkManager.Current.SaveChangesAsync();
        }


        public async Task MakeRequestHasOffers(RequestForQuotation requestForQuotation)
        {
            requestForQuotation.Statues = RequestForQuotationStatues.HasOffers;
            requestForQuotation.HadOffersDate = DateTime.UtcNow;
            await _repository.UpdateAsync(requestForQuotation);
        }

        public async Task MakeRequestInProcessAfterUserTakeOffer(RequestForQuotation request)
        {
            request.Statues = RequestForQuotationStatues.InProcess;
            request.HadOffersDate = null;
            await _repository.UpdateAsync(request);
        }

        public async Task CheckIfEntityExict(long id)
        {
            if (!await _repository.GetAll().AsNoTracking().AnyAsync(x => x.Id == id))
                throw new UserFriendlyException(404, Exceptions.ObjectWasNotFound, Tokens.RequestForQuotation);
        }


        public async Task<RequestsStatisticalNumbersDto> GetCountNumberAboutRequestsForQuotation(RequestStatisticalInputDto input)
        {
            var result = _repository.GetAll()
               .Where(p =>
                (!input.DateFrom.HasValue || p.CreationTime.Date >= input.DateFrom.Value.Date) &&
                (!input.DateTo.HasValue || p.CreationTime.Date <= input.DateTo.Value.Date))
               .GroupBy(p => 1)
               .Select(r => new RequestsStatisticalNumbersDto
               {
                   TotalNumber = r.Sum(p => p.IsDeleted == false ? 1 : 0),
                   Checking = r.Sum(p => p.Statues == RequestForQuotationStatues.Checking ? 1 : 0),
                   Approved = r.Sum(p => p.Statues == RequestForQuotationStatues.Approved ? 1 : 0),
                   Rejected = r.Sum(p => p.Statues == RequestForQuotationStatues.Rejected ? 1 : 0),
                   HasOffers = r.Sum(p => p.Statues == RequestForQuotationStatues.HasOffers ? 1 : 0),
                   InProcess = r.Sum(p => p.Statues == RequestForQuotationStatues.InProcess ? 1 : 0),
                   Possible = r.Sum(p => p.Statues == RequestForQuotationStatues.Possible ? 1 : 0),
               })
               .FirstOrDefault();
            if (result is null) return new RequestsStatisticalNumbersDto();
            return result;

        }
        public async Task<List<CitiesStatisticsForRequestsDto>> GetCitiesStatisticsForRequestsDto(InputCitiesStatisticsForRequestsDto input)
        {

            var result = input.ForDestination
                ? await _repository.GetAll()
                    .GroupBy(item => item.DestinationCityId)
                    .Select(group => new CitiesStatisticsForRequestsDto
                    {
                        CityId = group.Key.Value,
                        RequestForQuotationCount = group.Select(item => item.Id).Count()
                    }).OrderByDescending(r => r.RequestForQuotationCount).ToListAsync()
                : await _repository.GetAll()
                    .GroupBy(item => item.SourceCityId)
                    .Select(group => new CitiesStatisticsForRequestsDto
                    {
                        CityId = group.Key.Value,
                        RequestForQuotationCount = group.Select(item => item.Id).Count()
                    }).OrderByDescending(r => r.RequestForQuotationCount).ToListAsync();
            foreach (var item in result)
            {
                item.cityDto = await _cityManager.GetEntityDtoByIdAsync(item.CityId);
                if (item.cityDto is null) result.Remove(item);

            }

            if (result is null) return new List<CitiesStatisticsForRequestsDto>();
            return result;

        }

        public async Task<List<long>> GetRequestIdsWhichSubmitededByRigesteredUserViaBroker(long userId)
        {
            var userBroker = await _userManager.GetUserByIdAsync(userId);
            if (userBroker.Type != UserType.MediatorUser)
                throw new UserFriendlyException(Exceptions.YouCannotDoThisAction + "You Are Not Broker");
            var broker = await _mediatorManager.GetEntityByPhoneNumberAsync(userBroker.DialCode, userBroker.PhoneNumber);
            if (broker is null)
                throw new UserFriendlyException(Exceptions.ObjectWasNotFound, Tokens.Mediator);
            return await GetRequestIdsForUsersByBrokerCode(broker.MediatorCode);
        }
        public async Task<List<long>> GetRequestIdsForUsersByBrokerId(int mediatorId)
        {
            var mediator = await _mediatorManager.GetEntityByIdAsync(mediatorId);
            return await GetRequestIdsForUsersByBrokerCode(mediator.MediatorCode);
        }
        private async Task<List<long>> GetRequestIdsForUsersByBrokerCode(string mediatorCode)
        {
            var usersIds = await _userManager.Users.Where(x => x.MediatorCode == mediatorCode).Select(x => x.Id).ToListAsync();
            return await _repository.GetAll().AsNoTracking().Where(x => usersIds.Contains(x.UserId)).Select(x => x.Id).ToListAsync();
        }

        public async Task<bool> InserNewSearchedPlaceForUser(SearchedPlacesByUserDto input, long userId)
        {
            if (await _searchedPlaceByUserRepository.GetAll().AnyAsync(x => x.UserId == userId && x.PlaceId == input.PlaceId))
                return true;
            var searchedPlace = ObjectMapper.Map<SearchedPlacesByUser>(input);
            searchedPlace.UserId = userId;
            await _searchedPlaceByUserRepository.InsertAsync(searchedPlace);
            await UnitOfWorkManager.Current.SaveChangesAsync();
            return true;
        }

        public async Task<List<SearchedPlacesByUserDto>> GetAllSearchedPlacesByUser(long userId)
        {
            return ObjectMapper.Map(
                await _searchedPlaceByUserRepository.GetAll().AsNoTracking().Where(x => x.UserId == userId).ToListAsync(),
                new List<SearchedPlacesByUserDto>()
                );
        }

        public async Task<List<RequestForQuotation>> GetAllRequestHadOffersAndUserDidnotMakeAnyChangeForAutoRejectItAsync()
        {
            var hoursToWaitUser = await SettingManager.GetSettingValueAsync<int>(AppSettingNames.HoursToWaitUserToAcceptOfferOrRejectThem);

            return await _repository
                .GetAll()
                .Where(x => x.HadOffersDate.HasValue && DateTime.UtcNow >= x.HadOffersDate.Value.AddHours(hoursToWaitUser) && x.Statues == RequestForQuotationStatues.HasOffers)
                .ToListAsync();
        }

        public async Task CheckIfPlaceIdExistForThisUserAndDeleteIt(long userId, string placeId)
        {
            var searchedPlace = await _searchedPlaceByUserRepository.GetAll().Where(x => x.UserId == userId && x.PlaceId == placeId).FirstOrDefaultAsync();
            if (searchedPlace is null)
                throw new UserFriendlyException(404, Exceptions.ObjectWasNotFound, Tokens.Entity);
            await _searchedPlaceByUserRepository.DeleteAsync(searchedPlace);
        }

        public async Task<int> GetCountOfFinishedRequestUsersIds(List<long> usersIds)
        {
            return await _repository.GetAll()
                                .AsNoTracking()
                                .Where(x => usersIds.Contains(x.UserId) && x.Statues == RequestForQuotationStatues.Finished)
                                .CountAsync();
        }

        public async Task<List<RequestForQuotation>> GetAllPossibleRequestWithOutDate()
        {
            var hoursToConvertRequestForOutOfPossible = await SettingManager.GetSettingValueAsync<int>(AppSettingNames.HoursToMakeRequestOutOfPossible);

            return await _repository
                .GetAll()
                .Where(x => x.PossibledDate.HasValue && DateTime.UtcNow >= x.PossibledDate.Value.AddHours(hoursToConvertRequestForOutOfPossible) && x.Statues == RequestForQuotationStatues.Possible)
                .ToListAsync();
        }

        public async Task MakeAllPossibleRequestsAfterCustomTimeOutOfPossible()
        {
            var requests = await GetAllPossibleRequestWithOutDate();
            foreach (var request in requests)
            {
                request.Statues = RequestForQuotationStatues.OutOfPossible;
                request.PossibledDate = null;
            }
            await UnitOfWorkManager.Current.SaveChangesAsync();
        }

        public async Task DeleteAllRequestForUserIfFound(long userId)
        {
            await _repository.GetAll().Where(x => x.UserId == userId)
                  .ExecuteUpdateAsync(se => se.SetProperty(x => x.IsDeleted, true));
            await UnitOfWorkManager.Current.SaveChangesAsync();
        }

        public async Task<RequestForQuotation> GetRequestWithAttributeValues(long requestId)
        {
            var request = await _repository.GetAll().Include(c => c.AttributeForSourceTypeValues)
                 .Where(x => x.Id == requestId)
                 .FirstOrDefaultAsync();
            return request is null ? throw new UserFriendlyException(Exceptions.ObjectWasNotFound, Tokens.RequestForQuotation) : request;
        }

        //public async Task InsertSelectedCompaniesByUser(RequestForQuotation requestForQuotation, List<Company> companies)
        //{
        //    var selectedCompanies = new SelectedCompaniesBySystemForRequest()
        //    {
        //        RequestForQuotation = requestForQuotation,
        //        Companies = companies
        //    };
        //    await _selectedCompaniesByUserRepository.InsertAsync(selectedCompanies);
        //    await UnitOfWorkManager.Current.SaveChangesAsync();
        //}
    }
}
