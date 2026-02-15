namespace ReadIraq.Configuration.Dto
{
    public class FileSizeSettingDto
    {
        public double FileSize { get; set; }
    }
    public class ImageSizeSettingDto
    {
        public int ImageSize { get; set; }
    }
    public class HoursDto
    {
        public int HoursToWaitUser { get; set; }
        public int HoursToConvertRequestToOutOfPossible { get; set; }
    }
}
