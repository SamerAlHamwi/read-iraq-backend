using Abp.Domain.Repositories;
using Abp.Domain.Services;
using Abp.UI;
using Microsoft.EntityFrameworkCore;
using ReadIraq.Domain.Toolss.Dto;
using ReadIraq.Localization.SourceFiles;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading.Tasks;

namespace ReadIraq.Domain.Toolss
{
    public class ToolManger : DomainService, IToolManger
    {
        private readonly IRepository<Tool> _ToolsRepository;
        private readonly IRepository<ToolTranslation> _ToolsTranslationRepository;
        public ToolManger(IRepository<Tool> ToolsRepository,

            IRepository<ToolTranslation> ToolsTranslationRepository)
        {
            _ToolsRepository = ToolsRepository;
            _ToolsTranslationRepository = ToolsTranslationRepository;
        }

        public async Task<bool> CheckIfToolsIsExist(List<ToolTranslation> Translations)
        {
            var ToolsTranslations = await _ToolsTranslationRepository.GetAll().ToListAsync();
            foreach (var Translation in Translations)
            {
                foreach (var Tools in ToolsTranslations)
                    if (Tools.Name == Translation.Name && Tools.Language == Translation.Language)
                        return true;
            }
            return false;
        }

        public async Task<List<Tool>> GetToolssByIdsAsync(List<int> Ids)
        {
            return await _ToolsRepository.GetAll().Where(e => Ids.Contains(e.Id)).ToListAsync();
        }

        //public async Task<Tool> GetEntityByIdAsync(int id)
        //{
        //    var entity = await _ToolsRepository.GetAll()
        //     .Include(c => c.Translations).
        //      Include(c=>c.Attribute).ThenInclude(x=>x.Translations)
        //      .FirstOrDefaultAsync(x => x.Id == id);
        //    if (entity == null)
        //        throw new EntityNotFoundException(typeof(Tool), id);
        //    return entity;
        //}

        public async Task<Tool> GetLiteEntityByIdAsync(int id)
        {
            var entity = await _ToolsRepository.GetAll().Where(x => x.Id == id).Include(c => c.Translations).FirstOrDefaultAsync();
            if (entity is null)
                throw new UserFriendlyException(404, Exceptions.ObjectWasNotFound, Tokens.Tool);
            return entity;
        }

        public async Task<List<ToolTranslation>> GetToolsTranslationByToolsId(int ToolsId)
        {
            return await _ToolsTranslationRepository.GetAll().AsNoTracking().Where(x => x.CoreId == ToolsId && x.IsDeleted == false).ToListAsync();
        }

        public async Task<ToolDetailsDto> GetEntityByIdAsync(int id)
        {

            var entity = await _ToolsRepository.GetAll().AsNoTrackingWithIdentityResolution()
             .Include(c => c.Translations)
             .Include(x => x.SubService).ThenInclude(x => x.Translations)
             .Where(x => x.Id == id)
              .FirstOrDefaultAsync();
            if (entity is null)
                throw new UserFriendlyException(404, Exceptions.ObjectWasNotFound, Tokens.Tool);
            return ObjectMapper.Map<ToolDetailsDto>(entity);

        }
        public async Task<Tool> GetFullEntityByIdAsync(int id)
        {

            var entity = await _ToolsRepository.GetAll().AsNoTrackingWithIdentityResolution()
             .Include(c => c.Translations)
             .Include(x => x.SubService).ThenInclude(x => x.Translations)
             .Where(x => x.Id == id)
              .FirstOrDefaultAsync();
            if (entity is null)
                throw new UserFriendlyException(404, Exceptions.ObjectWasNotFound, Tokens.Tool);
            return entity;

        }

        public async Task HardDeleteTranslationAsync(List<ToolTranslation> translations)
        {
            foreach (var item in translations)
            {
                await _ToolsTranslationRepository.HardDeleteAsync(item);
            }
        }

        public async Task<bool> CheckIfToolsIsCorrect(List<int> toolIds)
        {
            var toolsIds = await _ToolsRepository.GetAll().AsNoTrackingWithIdentityResolution().Select(x => x.Id).ToListAsync();
            if (toolsIds.Any(x => toolIds.Contains(x)))
                throw new UserFriendlyException(404, Exceptions.ObjectWasNotFound, Tokens.Tool);
            return true;
        }

        public async Task<bool> DeleteToolsForSubServices(List<int> subServicesIds)
        {
            var tools = await _ToolsRepository.GetAll().Where(x => subServicesIds.Contains(x.SubServiceId.Value)).Include(x => x.Translations).ToListAsync();
            foreach (var tool in tools)
            {
                await HardDeleteTranslationAsync(tool.Translations.ToList());
                await _ToolsRepository.DeleteAsync(tool);
                await UnitOfWorkManager.Current.SaveChangesAsync();
            }
            return true;
        }
    }
}
