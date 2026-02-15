using AutoMapper;
using ReadIraq.Domain.PrivacyPolicies;
using ReadIraq.PrivacyPolicyService.Dto;

namespace ReadIraq.PrivacyPolicyService.Mapper
{
    public class PrivacyPolicyMapProfile : Profile
    {
        public PrivacyPolicyMapProfile()
        {
            CreateMap<CreatePrivacyPolicyDto, PrivacyPolicy>();
        }
    }
}
