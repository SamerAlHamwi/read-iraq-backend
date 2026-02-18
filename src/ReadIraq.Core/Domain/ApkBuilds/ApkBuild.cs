using Abp.Domain.Entities.Auditing;
using static ReadIraq.Enums.Enum;

namespace ReadIraq.Domain.ApkBuilds
{
    public class ApkBuild : FullAuditedEntity
    {
        public SystemType SystemType { get; set; }
        public int VersionCode { get; set; }
        public string VersionNumber { get; set; }
        public string Description { get; set; }
        public UpdateOptions UpdateOptions { get; set; }

    }
}
