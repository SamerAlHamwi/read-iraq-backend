using Abp.Application.Services.Dto;

namespace ReadIraq.Domain.Regions.Dto
{
    public class PagedRegionResultRequestDto : PagedResultRequestDto
    {
        public string Keyword { get; set; }
        public int? CityId { get; set; }
        public bool? IsActive { get; set; }
    }
}
