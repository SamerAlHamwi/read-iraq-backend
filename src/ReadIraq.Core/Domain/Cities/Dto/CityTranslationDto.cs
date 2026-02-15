using Abp.AutoMapper;
using ReadIraq.Domain.Cities;
using System.ComponentModel.DataAnnotations;

namespace ReadIraq.Cities.Dto

{
    /// <summary>
    /// Post Category Translation Dto
    /// </summary>
    [AutoMap(typeof(CityTranslation))]
    public class CityTranslationDto
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
