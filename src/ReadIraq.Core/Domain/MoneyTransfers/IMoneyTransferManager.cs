using Abp.Domain.Services;
using System;
using System.Threading.Tasks;

namespace ReadIraq.Domain.MoneyTransfers
{
    public interface IMoneyTransferManager : IDomainService
    {
        Task<double> ReturnAmountByOfferId(Guid offerId);
        Task InsertNewMoneyTransfer(MoneyTransfer newMoneyTransfer);
    }
}
