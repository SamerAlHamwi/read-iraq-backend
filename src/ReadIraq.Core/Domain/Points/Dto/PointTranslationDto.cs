using Abp.AutoMapper;
using ReadIraq.Domain.Points;
using System.ComponentModel.DataAnnotations;

namespace ReadIraq.Points.Dto

{
    /// <summary>
    /// Post Category Translation Dto
    /// </summary>
    [AutoMap(typeof(PointTranslation))]
    public class PointTranslationDto
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
