using Abp.Domain.Entities;
using Abp.Domain.Repositories;
using Abp.Domain.Services;
using Abp.UI;
using Microsoft.EntityFrameworkCore;
using ReadIraq.Authorization.Users;
using ReadIraq.Domain.Mediator.Dto;
using ReadIraq.Localization.SourceFiles;
using ReadIraq.Mediators.Dto;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading.Tasks;

namespace ReadIraq.Domain.Mediators.Mangers
{
    public class MediatorManager : DomainService, IMediatorManager
    {
        private readonly IRepository<Mediator> _mediatorRepository;
        private readonly UserManager _userManager;


        public MediatorManager(IRepository<Mediator> mediatorRepository, UserManager userManager)
        {
            _mediatorRepository = mediatorRepository;
            _userManager = userManager;

        }

        public async Task<List<Mediator>> GetAllMediatorsCodes()
        {

            return await _mediatorRepository.GetAll().ToListAsync();
        }
        public async Task<bool> CheckIfMediatorExist(string dialCode, string phoneNumber)
        {
            return await _mediatorRepository.GetAll().AnyAsync(x => x.MediatorPhoneNumber == phoneNumber && x.DialCode == dialCode);

        }
        public async Task<bool> CheckIfMediatorCodeExist(string mediatorCode)
        {
            return await _mediatorRepository.GetAll().Where(x => x.MediatorCode == mediatorCode).AnyAsync();

        }
        public async Task<Mediator> GetEntityByIdAsync(int id)
        {
            var entity = await _mediatorRepository.GetAll()
                .Where(m => m.Id == id)
                .FirstOrDefaultAsync();
            if (entity == null)
                throw new EntityNotFoundException(typeof(Mediator), id);
            return entity;
        }

        public async Task<Mediator> GetEntityByPhoneNumberAsync(string dialCode, string phoneNumber)
        {
            var entity = await _mediatorRepository.GetAll()
                .FirstOrDefaultAsync(x => x.DialCode == dialCode && x.MediatorPhoneNumber == phoneNumber);
            if (entity is null)
                throw new UserFriendlyException(string.Format(Exceptions.ObjectWasNotFound, Tokens.Mediator));
            return entity;
        }
        public async Task<List<StatisticsRegisteredUsersViaBrokers>> StatisticsRegisteredUsersViaBrokers()
        {
            var result = await _userManager.Users.Where(x => !string.IsNullOrWhiteSpace(x.MediatorCode)).GroupBy(x => x.MediatorCode).Select(r => new StatisticsRegisteredUsersViaBrokers
            {
                Code = r.Key,
                UserCount = r.Select(item => item.Id).Count()

            }).ToListAsync();
            var resultToReturn = new List<StatisticsRegisteredUsersViaBrokers>();
            foreach (var item in result)
            {
                var broker = await _mediatorRepository.GetAll().AsNoTracking().Where(x => x.MediatorCode == item.Code).FirstOrDefaultAsync();
                item.Broker = ObjectMapper.Map<MediatorDetailsDto>(broker);
                if (item.Broker is not null)
                    resultToReturn.Add(item);
            }
            if (resultToReturn is null) return new List<StatisticsRegisteredUsersViaBrokers>();
            return resultToReturn;
        }

        public async Task<int> GetBrokersCount()
        {
            return await _mediatorRepository.GetAll().AsNoTracking().Where(x => x.IsDeleted == false).CountAsync();
        }

        public async Task UpdateCommissionForMediator(double paidMoneyForRequest, double commissionFromCompany, string mediatorCode)
        {
            var mediator = await _mediatorRepository.GetAll().Where(x => x.MediatorCode == mediatorCode).FirstOrDefaultAsync();
            mediator.MoneyOwed = mediator.MoneyOwed + ((paidMoneyForRequest * commissionFromCompany) / 100 * mediator.CommissionPercentage / 100);
            await _mediatorRepository.UpdateAsync(mediator);
            await UnitOfWorkManager.Current.SaveChangesAsync();

        }

        //public async Task GiveMediatorCommissionForFinishRequest(string code, TinySelectedCompanyDto selectedCompany, Guid offerId)
        //{
        //    double commission = 0;
        //    //if (selectedCompany.Provider == Provider.Company)
        //    //    commission = (await _commissionGroupManager.GetCommissionByCompanyIdAsync(selectedCompany.Id)).Commission;
        //    //else
        //    //{
        //    //    var branch = await _companyBranchManager.GetSuperLiteEntityByIdAsync(selectedCompany.Id);
        //    //    if (branch.CompanyId.HasValue)
        //    //        commission = (await _commissionGroupManager.GetCommissionByCompanyIdAsync(selectedCompany.Id)).Commission;
        //    //    else
        //    //        commission = await SettingManager.GetSettingValueAsync<double>(AppSettingNames.CommissionForBranchesWithoutCompany);

        //    //}
        //    var paidMoneyForThisRequest = await _moneyTransferManager.ReturnAmountByOfferId(offerId);
        //    Mediator mediator = await _mediatorRepository.GetAll()
        //            .Where(x => x.MediatorCode == code)
        //            .FirstOrDefaultAsync();
        //    mediator.MoneyOwed = (paidMoneyForThisRequest * commission) / 100 * mediator.CommissionPercentage / 100;
        //    await _mediatorRepository.UpdateAsync(mediator);
        //}
    }
}
