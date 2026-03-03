using System;
using System.Collections.Generic;

namespace ReadIraq.LessonSessions.Dto
{
    public class GetLessonsByIdsInput
    {
        public List<Guid> Ids { get; set; }
    }
}
