using Abp.Domain.Services;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ReadIraq.Domain.Partners
{
    public interface IPartnerManager : IDomainService
    {

        Task<bool> CheckIfPartnerExist(string phoneNumber);
        Task<bool> CheckIfPartnerExistForUpdate(string phoneNumber, int partnerId);
        Task<bool> CheckIfPartnerByIdExist(int id);
        Task<Partner> GetEntityByIdAsync(int id);
        Task<Partner> GetFullEntityByIdAsync(int id);
        Task<bool> UpdateCitiesForPartnerAsync(List<int> CitiesIds, Partner partner);
        Task<int> GetPartnersCount();
    }
}
