using Abp.AutoMapper;
using ReadIraq.Domain.ContactUses;
using System.ComponentModel.DataAnnotations;


namespace ReadIraq.ContactUsService.Dto
{
    /// <summary>
    /// Post Category Translation Dto
    /// </summary>
    [AutoMap(typeof(ContactUsTranslation))]
    public class ContactUsTranslationDto
    {
        /// <summary>
        /// Name
        /// </summary>
        [Required]
        public string Name { get; set; }
        [Required]
        public string Description { get; set; }
        [Required]
        public string Address { get; set; }
        /// <summary>
        /// Language
        /// </summary>
        [Required]
        public string Language { get; set; }
    }
}
