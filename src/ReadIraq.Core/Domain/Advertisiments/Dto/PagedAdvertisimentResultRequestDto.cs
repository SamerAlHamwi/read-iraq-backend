using Abp.Application.Services.Dto;
using static ReadIraq.Enums.Enum;

namespace ReadIraq.Advertisiments.Dto
{
    /// <summary>
    /// Paged Post Category Result Request Dto
    /// </summary>
    public class PagedAdvertisimentResultRequestDto : PagedResultRequestDto
    {
        /// <summary>
        /// Keyword
        /// </summary>
        public string Keyword { get; set; }
        public bool? IsActive { get; set; }
        /// <summary>
        /// Screen Value: Home=1, Properties=2, Vehicles=3,Wanted=4
        /// </summary>
        public Screen? Screen { get; set; }
        /// <summary>
        /// Position For Advertisiment Value:  Top=1,InBetween=2
        /// </summary>
        public PositionForAdvertisiment? Position { get; set; }

    }
}
