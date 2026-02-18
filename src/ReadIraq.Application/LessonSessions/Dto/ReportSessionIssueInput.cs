using System;

namespace ReadIraq.LessonSessions.Dto
{
    public class ReportSessionIssueInput
    {
        public Guid SessionId { get; set; }
        public string Description { get; set; }
    }
}
