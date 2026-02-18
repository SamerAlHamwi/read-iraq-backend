using Abp.AutoMapper;
using ReadIraq.Authorization.Users;
using System.ComponentModel.DataAnnotations;

namespace ReadIraq.Users.Dto
{
    [AutoMapTo(typeof(User))]
    public class UpdateMeDto
    {
        [Required]
        public string Name { get; set; }
        [Required]
        public string Surname { get; set; }
        public string Avatar { get; set; }
        public int? GovernorateId { get; set; }
        public int? GradeId { get; set; }
    }
}
