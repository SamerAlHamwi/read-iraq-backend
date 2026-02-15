using Abp.Domain.Services;
using ReadIraq.Domain.Mediator.Dto;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ReadIraq.Domain.Mediators.Mangers
{
    public interface IMediatorManager : IDomainService
    {
        Task<List<Mediator>> GetAllMediatorsCodes();
        Task<bool> CheckIfMediatorExist(string dialCode, string phoneNumber);
        Task<Mediator> GetEntityByIdAsync(int id);
        Task<bool> CheckIfMediatorCodeExist(string mediatorCode);
        Task<Mediator> GetEntityByPhoneNumberAsync(string dialCode, string phoneNumber);
        Task<List<StatisticsRegisteredUsersViaBrokers>> StatisticsRegisteredUsersViaBrokers();
        Task<int> GetBrokersCount();
        Task UpdateCommissionForMediator(double paidMoneyForRequest, double commissionFromCompany, string mediatorCode);
        //Task GiveMediatorCommissionForFinishRequest(string code, TinySelectedCompanyDto selectedCompany,Guid offerId);



    }
}
