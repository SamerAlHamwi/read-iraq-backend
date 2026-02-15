using Abp.AutoMapper;
using System.ComponentModel.DataAnnotations;

namespace ReadIraq.Domain.Toolss.Dto
{
    [AutoMap(typeof(ToolTranslation))]
    public class ToolsTranslationDto
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
