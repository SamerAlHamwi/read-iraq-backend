using Abp.AutoMapper;
using System.ComponentModel.DataAnnotations;

namespace ReadIraq.Domain.Regions.Dto
{
    [AutoMap(typeof(RegionTranslation))]
    public class RegionTranslationDto
    {
        /// <summary>
        /// Name
        /// </summary>
        [Required]
        public string Name { get; set; }
        /// <summary>
        /// Language
        /// </summary>
        [Required]
        public string Language { get; set; }
    }
}
