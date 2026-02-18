using System;
using System.Collections.Generic;

namespace ReadIraq.Teachers.Dto
{
    public class AssignSubjectsInput
    {
        public Guid TeacherProfileId { get; set; }
        public List<Guid> SubjectIds { get; set; }
    }
}
