using Abp.Domain.Entities;
using Abp.Domain.Entities.Auditing;
using ReadIraq.Domain.SubServices;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace ReadIraq.Domain.Toolss
{
    public class Tool : FullAuditedEntity, IActiveEntity, IMultiLingualEntity<ToolTranslation>
    {
        public Tool()
        {
            Translations = new HashSet<ToolTranslation>();
        }
        public bool IsActive { get; set; }

        public ICollection<ToolTranslation> Translations { get; set; }
        public int? SubServiceId { get; set; }
        [ForeignKey(nameof(SubServiceId))]
        public virtual SubService SubService { get; set; }
    }
}
