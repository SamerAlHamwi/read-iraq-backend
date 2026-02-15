using static ReadIraq.Enums.Enum;

namespace ReadIraq.Domain.ApkBuilds.Dtos
{
    public class CreateApkBuildDto
    {
        public AppType AppType { get; set; }
        public SystemType SystemType { get; set; }
        public int VersionCode { get; set; }
        public string VersionNumber { get; set; }
        public string Description { get; set; }
    }
}
