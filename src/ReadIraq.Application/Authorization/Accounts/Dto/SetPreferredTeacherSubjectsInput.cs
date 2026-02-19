using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ReadIraq.Authorization.Accounts.Dto
{
    public class SetPreferredTeacherSubjectsInput
    {
        [Required]
        public long UserId { get; set; }

        [Required]
        public List<PreferredTeacherSubjectDto> TeacherSubjects { get; set; }
    }

    public class PreferredTeacherSubjectDto
    {
        [Required]
        public Guid SubjectId { get; set; }

        [Required]
        public List<Guid> TeachersId { get; set; }
    }
}
