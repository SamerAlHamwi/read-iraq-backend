using Abp.Application.Services.Dto;

namespace ReadIraq.Cities.Dto
{
    public class PagedCityResultRequestDto : PagedResultRequestDto
    {
        public string Keyword { get; set; }
        public int? CountryId { get; set; }
        public bool? IsActive { get; set; }
    }
}
