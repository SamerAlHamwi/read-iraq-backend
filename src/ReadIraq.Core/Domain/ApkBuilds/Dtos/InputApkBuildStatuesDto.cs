using static ReadIraq.Enums.Enum;

namespace ReadIraq.Domain.ApkBuilds.Dtos
{
    public class InputApkBuildStatuesDto
    {
        public AppType AppType { get; set; }
        public SystemType SystemType { get; set; }
        public int VersionCode { get; set; }

    }
    public class OutputApkBuildStatuesDto
    {
        public UpdateOptions UpdateOptions { get; set; }
        public bool ApkIsNotFound { get; set; }
    }
    public class InputApkNuildStatuesDto
    {
        public int Id { get; set; }

        public UpdateOptions UpdateOptions { get; set; }
    }
}
