using Abp.Application.Services.Dto;

namespace ReadIraq.ActivationCodes.Dto
{
    public class GetActivationCodesInput : PagedAndSortedResultRequestDto
    {
        public string Filter { get; set; }
        public bool? IsUsed { get; set; }
    }
}
