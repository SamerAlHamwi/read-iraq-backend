using Abp.Application.Services;
using Abp.Application.Services.Dto;
using ReadIraq.ActivationCodes.Dto;
using System;
using System.Threading.Tasks;

namespace ReadIraq.ActivationCodes
{
    public interface IActivationCodeAppService : IApplicationService
    {
        Task GenerateCodes(CreateActivationCodeInput input);
        Task<ActivationCodeDto> GetCode(Guid id);
        Task<PagedResultDto<ActivationCodeDto>> GetAll(GetActivationCodesInput input);
        Task<ActivationCodeStatisticsDto> GetStatistics();
        Task UseCode(string code, Guid? subjectId);
    }
}
