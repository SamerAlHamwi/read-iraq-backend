using System;

namespace ReadIraq.Notifications.Dto
{
    public class ScheduleNotificationInput : SendNotificationInput
    {
        public DateTime ScheduledTime { get; set; }
        public string Recurrence { get; set; } // e.g. "None", "Daily", "Weekly"
    }
}
