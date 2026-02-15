using Abp.Domain.Repositories;
using Abp.Domain.Services;
using Abp.UI;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using ReadIraq.Domain.Attachments;
using ReadIraq.Domain.services.Dto;
using ReadIraq.Domain.Services.Dto;
using ReadIraq.Domain.ServiceValueForOffers;
using ReadIraq.Domain.ServiceValues.Dto;
using ReadIraq.Domain.SubServices;
using ReadIraq.Domain.Toolss;
using ReadIraq.Localization.SourceFiles;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static ReadIraq.Enums.Enum;

namespace ReadIraq.Domain.services
{
    public class ServiceManager : DomainService, IServiceManager
    {
        private readonly IRepository<Service> _serviceRepository;
        private readonly IRepository<Tool> _toolRepository;
        private readonly IRepository<SubService> _subServiceRepository;
        private readonly IMapper _mapper;
        private readonly IRepository<ServiceTranslation> _serviceTranslationRepository;
        private readonly IAttachmentManager _attachmentManager;
        public ServiceManager(IRepository<Service> serviceRepository, IMapper mapper, IRepository<ServiceTranslation> serviceTranslationRepository, IRepository<Tool> toolRepository, IRepository<SubService> subServiceRepository, IAttachmentManager attachmentManager)
        {
            _serviceRepository = serviceRepository;
            _mapper = mapper;
            _serviceTranslationRepository = serviceTranslationRepository;
            _toolRepository = toolRepository;
            _subServiceRepository = subServiceRepository;
            _attachmentManager = attachmentManager;
        }

        public async Task<bool> CheckServiceIfExict(List<ServiceTranslationDto> translations)
        {
            var serviceTranslations = await _serviceTranslationRepository.GetAllListAsync();

            foreach (var translation in translations)
            {
                foreach (var serviceTranslation in serviceTranslations)
                    if (serviceTranslation.Name == translation.Name && serviceTranslation.Language == translation.Language)
                        return true;
            }
            return false;
        }

        public async Task<bool> CheckServicesIsCorrect(List<ServiceValuesDto> serviceValues)
        {
            var services = await _serviceRepository.GetAll().Include(x => x.SubServices).ThenInclude(x => x.Tools).AsNoTrackingWithIdentityResolution().ToListAsync();
            var subServices = services.SelectMany(x => x.SubServices).ToList();
            var toolsForSubService = services.SelectMany(x => x.SubServices).SelectMany(x => x.Tools).ToList();
            foreach (var serviceValue in serviceValues)
            {

                if (!services.Any(x => x.Id == serviceValue.ServiceId))
                    throw new UserFriendlyException(404, Exceptions.ObjectWasNotFound, Tokens.Service + " " + serviceValue.ServiceId.ToString());

                if (!subServices.Any(x => x.Id == serviceValue.SubServiceId))
                    throw new UserFriendlyException(404, Exceptions.ObjectWasNotFound, Tokens.SubService + " " + serviceValue.SubServiceId.ToString());
                var servicesWithSubServices = subServices.Where(x => x.ServiceId == serviceValue.ServiceId).Select(x => x.Id).ToList();
                if (!servicesWithSubServices.Any(x => x == serviceValue.SubServiceId))
                    throw new UserFriendlyException(Exceptions.IncompatibleValue, Tokens.SubService + " " + serviceValue.SubServiceId.ToString());
                if (serviceValue.ToolId is not null)
                    if (!toolsForSubService.Any(x => x.Id == serviceValue.ToolId.Value))
                        throw new UserFriendlyException(404, Exceptions.IncompatibleValue, Tokens.Tool + " ToolId With ServiceId");
            }
            return true;
        }

        public async Task<ServiceDetailsDto> GetEntityByIdAsync(int id)
        {
            var service = await GetFullEntityByIdAsync(id);
            return ObjectMapper.Map<ServiceDetailsDto>(service);
        }
        public async Task<Service> GetFullEntityByIdAsync(int id)
        {
            var service = await _serviceRepository.GetAll()
                .Include(x => x.Translations)
                .Include(x => x.SubServices).ThenInclude(x => x.Tools).ThenInclude(x => x.Translations)
                .Where(x => x.Id == id).FirstOrDefaultAsync();
            if (service is null)
                throw new UserFriendlyException(404, Exceptions.ObjectWasNotFound, Tokens.Service);
            return service;
        }

        public async Task HardDeleteForEntityTranslation(List<ServiceTranslation> translations)
        {
            foreach (var serviceTranslation in translations)
            {
                await _serviceTranslationRepository.HardDeleteAsync(serviceTranslation);
            }
        }
        public async Task<Service> GetLiteEntityAsyn(int id)
        {
            var service = await _serviceRepository.GetAll().Include(x => x.Translations).Where(x => x.Id == id).FirstOrDefaultAsync();
            if (service is null)
                throw new UserFriendlyException(404, Exceptions.ObjectWasNotFound, Tokens.Service);
            return service;
        }

        public async Task<List<ServiceDetailsDto>> GetFullServicesForUserOrCompanyOrOffer(List<int> servicesIds, List<int> subservicesIds, List<int> toolIdsForSubServices)
        {

            var services = await _serviceRepository.GetAll().AsNoTracking().Include(x => x.Translations)
                    .Where(x => servicesIds.Contains(x.Id)).ToListAsync();
            var subServices = await _subServiceRepository.GetAll().AsNoTracking().Include(x => x.Translations)
                    .Where(x => subservicesIds.Contains(x.Id)).ToListAsync();
            var toolForSubServices = await _toolRepository.GetAll().AsNoTracking().Include(x => x.Translations)
               .Where(x => toolIdsForSubServices.Contains(x.Id)).ToListAsync();
            foreach (var service in services)
            {
                service.SubServices = subServices.Where(x => x.ServiceId == service.Id).ToList();
            }
            foreach (var subService in services.SelectMany(x => x.SubServices))
            {
                subService.Tools = toolForSubServices.Where(x => x.SubServiceId == subService.Id).ToList();
            }

            var fullservices = ObjectMapper.Map(services, new List<ServiceDetailsDto>());
            var attchmentForServices = await _attachmentManager.GetByRefTypeAsync(AttachmentRefType.Service);
            var attchmentForSubServices = await _attachmentManager.GetByRefTypeAsync(AttachmentRefType.SubService);
            var attchmentForTools = await _attachmentManager.GetByRefTypeAsync(AttachmentRefType.Tool);
            foreach (var service in fullservices)
            {
                var attachment = attchmentForServices.Where(x => x.RefId == service.Id).FirstOrDefault();
                if (attachment is not null)
                    service.Attachment = new LiteAttachmentDto
                    {
                        Id = attachment.Id,
                        Url = _attachmentManager.GetUrl(attachment),
                        LowResolutionPhotoUrl = _attachmentManager.GetLowResolutionPhotoUrl(attachment)
                    };
            }
            foreach (var subService in fullservices.SelectMany(x => x.SubServices))
            {
                var attachment = attchmentForSubServices.Where(x => x.RefId == subService.Id).FirstOrDefault();
                if (attachment is not null)
                    subService.Attachment = new LiteAttachmentDto
                    {
                        Id = attachment.Id,
                        Url = _attachmentManager.GetUrl(attachment),
                        LowResolutionPhotoUrl = _attachmentManager.GetLowResolutionPhotoUrl(attachment)
                    };
            }

            foreach (var tool in fullservices.SelectMany(x => x.SubServices.SelectMany(x => x.Tools)))
            {
                var attachment = attchmentForTools.Where(x => x.RefId == tool.Id).FirstOrDefault();
                if (attachment is not null)
                    tool.Attachment = new LiteAttachmentDto
                    {
                        Id = attachment.Id,
                        Url = _attachmentManager.GetUrl(attachment),
                        LowResolutionPhotoUrl = _attachmentManager.GetLowResolutionPhotoUrl(attachment)
                    };
            }
            return fullservices;

        }

        public async Task<List<ServiceDto>> GetAllServicesDtosAsync()
        {
            return ObjectMapper.Map(await _serviceRepository.GetAllIncluding(x => x.Translations).AsNoTracking().ToListAsync(), new List<ServiceDto>());
        }


        public async Task<List<ServiceDetailsDto>> GetFullServicesForOffer(List<ServiceValueForOffer> servicesValues)
        {
            var servicesIds = servicesValues.Select(x => x.ServiceId).Distinct().ToList();
            var subservicesIds = servicesValues.Select(x => x.SubServiceId).Distinct().ToList();
            var toolIdsForSubServices = servicesValues.Where(x => x.ToolId.HasValue).Select(x => x.ToolId.Value).Distinct().ToList();

            var fullservices = await GetFullServicesForUserOrCompanyOrOffer(servicesIds, subservicesIds, toolIdsForSubServices);
            foreach (var tool in fullservices.SelectMany(x => x.SubServices.SelectMany(x => x.Tools)))
            {

                tool.Amount = servicesValues.Where(x => x.ToolId.HasValue && x.ToolId.Value == tool.Id)
               .Select(x => x.Amount).FirstOrDefault();
            }
            return fullservices;
        }

        //public async Task<ServiceDetailsDto> GetServiceByRequestForQuotationIdAsync(long requestId)
        //{
        //    var service = await _serviceRepository.GetAll().Where(x => x.RequestForQuotationId == requestId).FirstOrDefaultAsync();
        //    var serviceDto = new ServiceDetailsDto();
        //    serviceDto = _mapper.Map<ServiceDetailsDto>(service);
        //    return serviceDto;
        //}
    }
}
