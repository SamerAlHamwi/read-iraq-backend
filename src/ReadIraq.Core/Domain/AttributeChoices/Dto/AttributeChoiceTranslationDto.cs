using Abp.AutoMapper;
using System.ComponentModel.DataAnnotations;

namespace ReadIraq.Domain.AttributeChoices.Dto
{
    [AutoMap(typeof(AttributeChoiceTranslation))]
    public class AttributeChoiceTranslationDto
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
