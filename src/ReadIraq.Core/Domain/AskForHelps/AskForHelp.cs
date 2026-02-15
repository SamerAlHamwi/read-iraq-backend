using Abp.Domain.Entities.Auditing;
using ReadIraq.Authorization.Users;
using System.ComponentModel.DataAnnotations.Schema;
using static ReadIraq.Enums.Enum;

namespace ReadIraq.Domain.AskForHelps
{
    public class AskForHelp : FullAuditedEntity
    {
        public long UserId { get; set; }
        [ForeignKey(nameof(UserId))]
        public virtual User User { get; set; }
        public string Message { get; set; }
        public AskForHelpStatues Statues { get; set; }
    }
}
