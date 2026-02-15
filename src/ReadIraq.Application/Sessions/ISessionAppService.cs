using Abp.Application.Services;
using ReadIraq.Sessions.Dto;
using System.Threading.Tasks;

namespace ReadIraq.Sessions
{
    public interface ISessionAppService : IApplicationService
    {
        Task<GetCurrentLoginInformationsOutput> GetCurrentLoginInformations();
    }
}
