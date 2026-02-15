using Abp.Application.Services.Dto;

namespace ReadIraq.PrivacyPolicyService.Dto
{
    public class UpdatePrivacyPolicyDto : CreatePrivacyPolicyDto, IEntityDto
    {
        public int Id { get; set; }
    }
}