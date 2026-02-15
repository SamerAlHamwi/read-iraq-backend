using Abp.Domain.Repositories;
using Abp.Domain.Services;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace ReadIraq.Domain.MoneyTransfers
{
    public class MoneyTransferManager : DomainService, IMoneyTransferManager
    {
        private readonly IRepository<MoneyTransfer> _moneyTransferRepository;
        public MoneyTransferManager(IRepository<MoneyTransfer> moneyTransferRepository)
        {
            _moneyTransferRepository = moneyTransferRepository;
        }

        public async Task InsertNewMoneyTransfer(MoneyTransfer newMoneyTransfer)
        {
            await _moneyTransferRepository.InsertAsync(newMoneyTransfer);
            await UnitOfWorkManager.Current.SaveChangesAsync();
        }

        public async Task<double> ReturnAmountByOfferId(Guid offerId)
        {
            return await _moneyTransferRepository.GetAll()
                .AsNoTracking()
                .Where(x => x.OfferId == offerId.ToString())
                .Select(x => x.Amount)
                .FirstOrDefaultAsync();

        }
    }
}
