using Abp.Application.Services.Dto;

namespace ReadIraq.Search.Dto
{
    public class SearchInput : PagedResultRequestDto
    {
        public string Q { get; set; }
        public string Type { get; set; } // subject|session|teacher
    }
}
