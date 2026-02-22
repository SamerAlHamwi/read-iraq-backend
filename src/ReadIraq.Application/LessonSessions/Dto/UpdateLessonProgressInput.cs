using System;

namespace ReadIraq.LessonSessions.Dto
{
    public class UpdateLessonProgressInput
    {
        public Guid SessionId { get; set; }
        public int WatchedSeconds { get; set; }
        public bool IsCompleted { get; set; }
    }
}
