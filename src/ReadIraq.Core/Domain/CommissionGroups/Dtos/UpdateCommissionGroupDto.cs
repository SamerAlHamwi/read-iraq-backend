using Abp.Application.Services.Dto;

namespace ReadIraq.Domain.CommissionGroups.Dtos
{
    public class UpdateCommissionGroupDto : CreateCommissionGroupDto, IEntityDto
    {
        public int Id { get; set; }
    }
}
