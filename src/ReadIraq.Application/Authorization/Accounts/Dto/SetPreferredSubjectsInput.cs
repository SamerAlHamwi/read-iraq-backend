using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ReadIraq.Authorization.Accounts.Dto
{
    public class SetPreferredSubjectsInput
    {
        [Required]
        public long UserId { get; set; }

        [Required]
        public List<Guid> SubjectIds { get; set; }
    }
}
