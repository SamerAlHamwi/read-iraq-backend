using Abp.Dependency;
using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;
using static ReadIraq.Enums.Enum;

namespace ReadIraq.FileUploadService
{
    public interface IFileUploadService : ITransientDependency
    {
        /// <summary>
        /// Save file to Attachments folder (wwwroot\Attachments\).
        /// </summary>
        /// <param name="file">uploaded file</param>
        /// <returns>MediaType and storage key relative to wwwroot folder</returns>
        Task<UploadedFileInfo> SaveAttachmentAsync(IFormFile file);
        Task<UploadedApk> SaveApk(IFormFile file);

        /// <summary>
        /// Save file to Images folder (wwwroot\Images\).
        /// </summary>
        /// <param name="file">uploaded file</param>
        /// <returns>Image Type and Path of saved file relative to wwwroot folder</returns>
        Task<UploadedImageInfo> SaveImageAsync(IFormFile file);

        /// <summary>
        /// Delete Attachment
        /// </summary>
        /// <param name="fileRelativePath"></param>
        void DeleteAttachment(string fileRelativePath);

        /// <summary>
        /// Check File Size Async
        /// </summary>
        /// <param name="file"></param>
        /// <returns></returns>
        Task CheckFileSizeAsync(IFormFile file);
        Task CheckApkSizeAsync(IFormFile file);
        /// <summary>
        ///  Get And Check File Type
        /// </summary>
        /// <param name="file"></param>
        /// <returns></returns>
        MediaType GetAndCheckFileType(IFormFile file);
        /// <summary>
        /// Generate Path To Save
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        UploadedImageInfo GeneratePathToSave(string patientInfo, string folderName);

        UploadedImageInfo GeneratePathToSave(string propertyInfo);
    }

    /// <summary>
    /// Uploaded File Info
    /// </summary>
    public class UploadedFileInfo
    {
        /// <summary>
        /// MediaType
        /// </summary>
        public MediaType Type { get; set; }
        /// <summary>
        /// StorageKey
        /// </summary>
        public string RelativePath { get; set; } // kept name for backward compatibility
        public string LowResolutionPhotoRelativePath { get; set; }
    }
    public class UploadedApk
    {
        public MediaType Type { get; set; }
        public string RelativePath { get; set; }
    }
    /// <summary>
    /// Uploaded Image Info
    /// </summary>
    public class UploadedImageInfo
    {
        /// <summary>
        /// ImageType
        /// </summary>
        public ImageType Type { get; set; }
        /// <summary>
        /// RelativePath
        /// </summary>
        public string RelativePath { get; set; }
        /// <summary>
        /// PathToSave
        /// </summary>
        public string PathToSave { get; set; }
    }
}
