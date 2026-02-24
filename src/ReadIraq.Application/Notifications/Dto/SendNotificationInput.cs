using System;
using System.Collections.Generic;
using static ReadIraq.Enums.Enum;

namespace ReadIraq.Notifications.Dto
{
    public class SendNotificationInput
    {
        public string Title { get; set; }
        public string Body { get; set; }
        public NotificationType Type { get; set; }
        public NotificationTargetFilter Filter { get; set; }
    }

    public class NotificationTargetFilter
    {
        public bool All { get; set; }
        public Guid? GradeId { get; set; }
        public Guid? SubjectId { get; set; }
        public bool SubscribedOnly { get; set; }
        public List<long> UserIds { get; set; }
        public bool TeacherStudentsOnly { get; set; }
    }
}
