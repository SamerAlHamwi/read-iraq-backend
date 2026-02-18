using Abp.Application.Services;
using ReadIraq.Dashboard.Dto;
using System.Threading.Tasks;

namespace ReadIraq.Dashboard
{
    public interface IDashboardAppService : IApplicationService
    {
        Task<DashboardSummaryOutput> GetSummaryAsync();
    }
}
