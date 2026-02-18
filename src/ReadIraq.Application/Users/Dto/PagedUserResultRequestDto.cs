using Abp.Application.Services.Dto;
using static ReadIraq.Enums.Enum;

namespace ReadIraq.Users.Dto
{
    //custom PagedResultRequestDto
    public class PagedUserResultRequestDto : PagedResultRequestDto
    {
        public string Keyword { get; set; }
        public bool? IsActive { get; set; }
        /// <summary>
        /// SuperAdmin = 1,
        /// Student = 2,
        /// Teacher = 3
        /// </summary>
        public UserType? UserType { get; set; }
        public string MediatorCode { get; set; }

        public int? GradeId { get; set; }
        public bool? Subscribed { get; set; }
    }
}
