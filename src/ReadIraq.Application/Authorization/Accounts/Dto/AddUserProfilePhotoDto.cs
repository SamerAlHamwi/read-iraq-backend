using System.ComponentModel.DataAnnotations;

namespace ReadIraq.Authorization.Accounts.Dto
{
    public class AddUserProfilePhotoDto
    {
        [Required]
        public long PhotoId { get; set; }
    }
}

