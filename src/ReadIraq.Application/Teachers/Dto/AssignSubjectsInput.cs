using System;
using System.Collections.Generic;

namespace ReadIraq.Teachers.Dto
{
    public class AssignSubjectsInput
    {
        public Guid TeacherProfileId { get; set; }
        public List<TeacherSubjectDto> Subjects { get; set; }
    }

    public class TeacherSubjectDto
    {
        public Guid SubjectId { get; set; }
        public int GradeId { get; set; }
    }
}
