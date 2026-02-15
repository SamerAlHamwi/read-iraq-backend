using Abp.Application.Services.Dto;

namespace ReadIraq.Domain.MoneyTransfers.Dtos
{
    public class UpdateMoneyTransferDto : CreateMoneyTransferDto, IEntityDto
    {
        public int Id { get; set; }
    }
}
