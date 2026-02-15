using System.ComponentModel.DataAnnotations;

namespace ReadIraq.Users.Dto
{
    public class ChangeUserLanguageDto
    {
        [Required]
        public string LanguageName { get; set; }
    }
}