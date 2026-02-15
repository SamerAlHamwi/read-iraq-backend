using Abp.Domain.Services;
using ReadIraq.Domain.Toolss.Dto;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ReadIraq.Domain.Toolss
{
    public interface IToolManger : IDomainService
    {
        Task<bool> CheckIfToolsIsExist(List<ToolTranslation> Translations);
        //Task<Tool> GetEntityByIdAsync(int id);
        Task<Tool> GetLiteEntityByIdAsync(int id);

        Task<List<Tool>> GetToolssByIdsAsync(List<int> Ids);

        Task<List<ToolTranslation>> GetToolsTranslationByToolsId(int ToolsId);
        Task<ToolDetailsDto> GetEntityByIdAsync(int id);
        Task HardDeleteTranslationAsync(List<ToolTranslation> translations);
        Task<bool> CheckIfToolsIsCorrect(List<int> toolIds);
        Task<bool> DeleteToolsForSubServices(List<int> subServicesIds);


    }

}