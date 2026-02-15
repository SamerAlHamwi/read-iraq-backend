using Abp.AutoMapper;
using ReadIraq.Domain.Countries;
using System.ComponentModel.DataAnnotations;


namespace ReadIraq.Countries.Dto
{
    /// <summary>
    /// Post Category Translation Dto
    /// </summary>
    [AutoMap(typeof(CountryTranslation))]
    public class CountryTranslationDto
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
