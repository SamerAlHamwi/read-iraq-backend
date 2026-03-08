using System;
using System.Collections.Generic;

namespace ReadIraq.Teachers.Dto
{
    public class PreferredTeachersSectionDto
    {
        public Guid? SubjectId { get; set; }
        public string Title { get; set; }
        public List<LiteTeacherProfileDto> Teachers { get; set; }
    }
}
