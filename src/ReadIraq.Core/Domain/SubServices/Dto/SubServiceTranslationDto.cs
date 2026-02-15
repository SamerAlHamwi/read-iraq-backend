using Abp.AutoMapper;
using System.ComponentModel.DataAnnotations;

namespace ReadIraq.Domain.SubServices.Dto
{
    [AutoMap(typeof(SubServiceTranslation))]
    public class SubServiceTranslationDto
    {
        [Required]
        public string Name { get; set; }
        /// <summary>
        /// Language
        /// </summary>
        [Required]
        public string Language { get; set; }
    }
}
