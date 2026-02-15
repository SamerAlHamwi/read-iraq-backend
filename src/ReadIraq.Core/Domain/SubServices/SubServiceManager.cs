using Abp.Domain.Entities;
using Abp.Domain.Repositories;
using Abp.Domain.Services;
using Abp.UI;
using Microsoft.EntityFrameworkCore;
using ReadIraq.Domain.SubServices.Dto;
using ReadIraq.Domain.Toolss;
using ReadIraq.Localization.SourceFiles;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading.Tasks;

namespace ReadIraq.Domain.SubServices
{
    public class SubServiceManager : DomainService, ISubServiceManager
    {
        private readonly IRepository<SubService> _SubServiceRepository;
        private readonly ToolManger _toolManger;
        private readonly IRepository<SubServiceTranslation> _SubServiceTranslationRepository;
        public SubServiceManager(IRepository<SubService> SubServiceRepository,

            ToolManger toolManger,
            IRepository<SubServiceTranslation> SubServiceTranslationRepository)
        {
            _SubServiceRepository = SubServiceRepository;
            _toolManger = toolManger;
            _SubServiceTranslationRepository = SubServiceTranslationRepository;
        }

        public async Task<bool> CheckIfSubServiceIsExist(List<SubServiceTranslationDto> Translations)
        {
            var SubServiceTranslations = await _SubServiceTranslationRepository.GetAll().AsNoTrackingWithIdentityResolution().ToListAsync();
            foreach (var Translation in Translations)
            {
                foreach (var SubService in SubServiceTranslations)
                    if (SubService.Name == Translation.Name && SubService.Language == Translation.Language)
                        return true;
            }
            return false;
        }

        public async Task<List<SubService>> GetSubServicesByIdsAsync(List<int> Ids)
        {
            return await _SubServiceRepository.GetAll().Where(e => Ids.Contains(e.Id)).ToListAsync();
        }

        public async Task<SubServiceDetailsDto> GetEntityByIdAsync(int id)
        {
            var entity = await _SubServiceRepository.GetAll()
             .Include(c => c.Translations)
              .FirstOrDefaultAsync(x => x.Id == id);
            if (entity is null)
                throw new UserFriendlyException(404, Exceptions.ObjectWasNotFound, Tokens.SubService);
            return ObjectMapper.Map<SubServiceDetailsDto>(entity);
        }
        public async Task<SubService> GetFullEntityByIdAsync(int id)
        {
            var entity = await _SubServiceRepository.GetAll()
             .Include(c => c.Translations)
             .FirstOrDefaultAsync(x => x.Id == id);
            if (entity is null)
                throw new UserFriendlyException(404, Exceptions.ObjectWasNotFound, Tokens.SubService);
            return entity;
        }

        public async Task<SubService> GetLiteEntityByIdAsync(int id)
        {
            var entity = await _SubServiceRepository.GetAll().Where(x => x.Id == id).Include(x => x.Translations).FirstOrDefaultAsync();
            if (entity == null)
                throw new EntityNotFoundException(typeof(SubService), id);
            return entity;
        }

        public async Task<List<SubServiceTranslation>> GetSubServiceTranslationBySubServiceId(int SubServiceId)
        {
            return await _SubServiceTranslationRepository.GetAll().AsNoTracking().Where(x => x.CoreId == SubServiceId && x.IsDeleted == false).ToListAsync();
        }

        public async Task HardDeleteForEntityTranslation(List<SubServiceTranslation> translations)
        {
            foreach (var translation in translations)
            {
                await _SubServiceTranslationRepository.HardDeleteAsync(translation);
            }
        }

        public async Task<bool> DeleteSubServiceForServiceBySerivceId(int serivceId)
        {
            var subServices = await _SubServiceRepository.GetAll().Where(x => x.ServiceId == serivceId).Include(x => x.Translations).ToListAsync();

            await _toolManger.DeleteToolsForSubServices(subServices.Select(x => x.Id).ToList());

            foreach (var item in subServices)
            {
                await HardDeleteForEntityTranslation(item.Translations.ToList());
                await _SubServiceRepository.DeleteAsync(item);
                await UnitOfWorkManager.Current.SaveChangesAsync();
            }
            return true;
        }
    }
}
