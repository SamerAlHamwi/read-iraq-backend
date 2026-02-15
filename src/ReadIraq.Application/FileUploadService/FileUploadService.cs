using Abp.Authorization;
using Abp.Configuration;
using Abp.Localization;
using Abp.Localization.Sources;
using Abp.Timing;
using Abp.UI;
using Castle.Core.Logging;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using ReadIraq.Configuration;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Processing;
using System;
using System.IO;
using System.Threading.Tasks;
using static ReadIraq.Enums.Enum;

namespace ReadIraq.FileUploadService
{
    /// <summary>
    /// File Upload Service
    /// </summary>
    [AbpAuthorize]
    public class FileUploadService : IFileUploadService
    {
        //Can get those constants from configuration
        private static readonly string AttachmentsFolder = Path.Combine(AppConsts.UploadsFolderName, AppConsts.AttachmentsFolderName);
        private static readonly string LowResolutionPhotosFolder = Path.Combine(AppConsts.UploadsFolderName, AppConsts.LowResolutionPhotosFolderName);
        private static readonly string ImagesFolder = Path.Combine(AppConsts.UploadsFolderName, AppConsts.ImagesFolderName);
        private static readonly string ApksFolder = Path.Combine(AppConsts.UploadsFolderName, AppConsts.ApkFolderName);

        private readonly ISettingManager _settingManager;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly ILocalizationSource _localizationSource;
        /// <summary>
        /// Logger
        /// </summary>
        public ILogger Logger { get; set; }
        /// <summary>
        ///  File Upload Service
        /// </summary>
        /// <param name="localizationManager"></param>
        /// <param name="settingManager"></param>
        /// <param name="webHostEnvironment"></param>
        public FileUploadService(ILocalizationManager localizationManager,
            ISettingManager settingManager,
            IWebHostEnvironment webHostEnvironment)
        {
            _settingManager = settingManager;
            _webHostEnvironment = webHostEnvironment;
            _localizationSource = localizationManager.GetSource(ReadIraqConsts.LocalizationSourceName);
            Logger = NullLogger.Instance;
        }
        /// <summary>
        /// Save Attachment Async
        /// </summary>
        /// <param name="file"></param>
        /// <returns></returns>
        [AbpAllowAnonymous]
        public async Task<UploadedFileInfo> SaveAttachmentAsync(IFormFile file)
        {
            var fileInfo = new UploadedFileInfo { Type = GetAndCheckFileType(file) };

            var fileName = GenerateUniqueFileName(file);
            var pathToSaveAttacment = GetPathToSaveAttachment(fileName, AttachmentsFolder);

            fileInfo.RelativePath = GetAttachmentRelativePath(fileName, AttachmentsFolder);
            using (var stream = new FileStream(pathToSaveAttacment, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }
            if (fileInfo.Type == AttachmentType.PNG || fileInfo.Type == AttachmentType.JPG || fileInfo.Type == AttachmentType.JPEG)
            {
                var pathToSaveLowResolutionPhotos = GetPathToSaveAttachment(fileName, LowResolutionPhotosFolder);
                // Load the original image from the saved file
                using (var originalImage = Image.Load(pathToSaveAttacment))
                {
                    var ImageSize = await _settingManager.GetSettingValueAsync<int>(AppSettingNames.ImageSize);
                    // Create and save a version with resolution 200x200
                    originalImage.Mutate(x => x.Resize(new ResizeOptions
                    {
                        Size = new Size(ImageSize),
                        Mode = ResizeMode.Max
                    }));
                    var pathToSaveLowResolutionPhotos200 = GetPathToSaveAttachment(fileName, LowResolutionPhotosFolder);
                    originalImage.Save(pathToSaveLowResolutionPhotos200);
                }
                fileInfo.LowResolutionPhotoRelativePath = GetAttachmentRelativePath(fileName, LowResolutionPhotosFolder);
            }
            //if (fileInfo.Type == AttachmentType.PNG || fileInfo.Type == AttachmentType.JPG || fileInfo.Type == AttachmentType.JPEG)
            //{
            //    var pathToSaveLowResolutionPhotos = GetPathToSaveAttachment(fileName, LowResolutionPhotosFolder);
            //    // Load the original image from the saved file
            //    using (var originalImage = Image.FromFile(pathToSaveAttacment))
            //    {
            //        // Create and save a version with resolution 200x200

            //        using (var newImage200 = ResizeImage(originalImage, await _settingManager.GetSettingValueAsync<int>(AppSettingNames.ImageSize)))
            //        {
            //            //var fileName200 = $"{Path.GetFileNameWithoutExtension(fileName)}_200x200{Path.GetExtension(fileName)}";
            //            var pathToSaveLowResolutionPhotos200 = GetPathToSaveAttachment(fileName, LowResolutionPhotosFolder);
            //            newImage200.Save(pathToSaveLowResolutionPhotos200);
            //        }


            //    }
            //    fileInfo.LowResolutionPhotoRelativePath = GetAttachmentRelativePath(fileName, LowResolutionPhotosFolder);
            //}
            Logger.Info($"Base Attachment File was saved to ({pathToSaveAttacment}) successfully.");

            return fileInfo;
        }
        /// <summary>
        /// Save Image Async
        /// </summary>
        /// <param name="file"></param>
        /// <returns></returns>
        public async Task<UploadedImageInfo> SaveImageAsync(IFormFile file)
        {
            var fileInfo = new UploadedImageInfo { Type = GetAndCheckImageFileType(file) };
            await CheckFileSizeAsync(file);

            var fileName = GenerateUniqueImageFileName(file.FileName);
            var pathToSave = GetPathToSaveImage(fileName);
            using (var stream = new FileStream(pathToSave, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            Logger.Info($"Base Image File was saved to ({pathToSave}) successfully.");

            fileInfo.PathToSave = pathToSave;
            fileInfo.RelativePath = GetImageRelativePath(fileName);
            return fileInfo;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="patientInfo"></param>
        /// <returns></returns>
        public UploadedImageInfo GeneratePathToSave(string patientInfo, string folderName)
        {

            var fileName = $"QRImage-{patientInfo}-{Guid.NewGuid()}.png";
            var pathToSave = GetPathToSaveAttachment(fileName, folderName);

            var fileInfo = new UploadedImageInfo
            {
                PathToSave = pathToSave,
                RelativePath = GetAttachmentRelativePath(fileName, folderName),
                Type = ImageType.PNG
            };
            return fileInfo;
        }
        /// <summary>
        /// Delete Attachment
        /// </summary>
        /// <param name="fileRelativePath"></param>
        public void DeleteAttachment(string fileRelativePath)
        {
            var pathFile = GetAbsolutePath(fileRelativePath);

            if (!File.Exists(pathFile))
            {
                Logger.Error($"Attachment File ({pathFile}) is not found.");
                return;
            }

            File.Delete(pathFile);

            Logger.Info($"Attachment File ({pathFile}) was deleted successfully.");
        }
        /// <summary>
        /// Check File Size Async
        /// </summary>
        /// <param name="file"></param>
        /// <returns></returns>
        [AbpAllowAnonymous]
        public async Task CheckFileSizeAsync(IFormFile file)
        {
            var maxFileSize = await _settingManager.GetSettingValueAsync<decimal>(AppSettingNames.FileSize);


            if (file.Length >= maxFileSize * 1024 * 1024)
                throw new UserFriendlyException(L("FileSizeExceedsMaxFileSize{0}", maxFileSize));
        }
        /// <summary>
        /// Get And Check File Type
        /// </summary>
        /// <param name="file"></param>
        /// <returns></returns>
        [AbpAllowAnonymous]
        public AttachmentType GetAndCheckFileType(IFormFile file)
        {
            foreach (AttachmentType type in Enum.GetValues(typeof(AttachmentType)))
            {
                if (file.ContentType.Contains(type.ToString().ToLower()))
                    return type;
            }

            throw new UserFriendlyException(L("TheAttachedFileTypeIsNotAcceptable"), $"FileName: {file.FileName}");
        }

        private string L(string key)
        {
            return _localizationSource.GetString(key);
        }

        private string L(string key, params object[] args)
        {
            return _localizationSource.GetString(key, args);
        }

        private string GetAbsolutePath(string fileRelativePath)
        {
            var basePath = _webHostEnvironment.WebRootPath;
            return Path.Combine(basePath, fileRelativePath);
        }

        private string GetPathToSaveAttachment(string fileName, string folderName)
        {
            var basePath = _webHostEnvironment.WebRootPath;
            return Path.Combine(basePath, folderName, fileName);
        }

        private string GetAttachmentRelativePath(string fileName, string folderName)
        {
            return Path.Combine(folderName, fileName);
        }

        private string GenerateUniqueFileName(IFormFile file)
        {
            var fileName = $"{Guid.NewGuid()}_{Clock.Now.Ticks}{Path.GetExtension(file.FileName)}";
            return fileName;
        }

        private string GetPathToSaveImage(string fileName)
        {
            var basePath = _webHostEnvironment.WebRootPath;
            return Path.Combine(basePath, ImagesFolder, fileName);
        }
        /// <summary>
        /// Get Image Relative Path
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public string GetImageRelativePath(string fileName)
        {
            return Path.Combine(ImagesFolder, fileName);
        }

        private string GenerateUniqueImageFileName(string fileName)
        {
            return $"ItemImage-{Guid.NewGuid()}{Path.GetExtension(fileName)}";
        }
        //private static Image ResizeImage(Image image, int size)
        //{
        //    var newSize = new Size(size, size);
        //    var newImage = new Bitmap(newSize.Width, newSize.Height);
        //    using (var graphics = Graphics.FromImage(newImage))
        //    {
        //        graphics.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
        //        graphics.DrawImage(image, new Rectangle(Point.Empty, newSize));
        //    }
        //    return newImage;
        //}
        private ImageType GetAndCheckImageFileType(IFormFile file)
        {
            foreach (ImageType type in Enum.GetValues(typeof(ImageType)))
            {
                if (file.ContentType.Contains(type.ToString().ToLower()))
                    return type;
            }

            throw new UserFriendlyException(L("UploadedImageFileTypeIsNotAcceptable"), $"FileName: {file.FileName}");
        }

        public UploadedImageInfo GeneratePathToSave(string propertyInfo)
        {

            var fileName = $"QRImage-{propertyInfo}-{Guid.NewGuid()}.png";
            var pathToSave = GetPathToSaveAttachment(fileName);

            var fileInfo = new UploadedImageInfo
            {
                PathToSave = pathToSave,
                RelativePath = GetAttachmentRelativePath(fileName),
                Type = (ImageType)AttachmentType.PNG
            };
            return fileInfo;
        }

        private string GetPathToSaveAttachment(string fileName)
        {
            var basePath = _webHostEnvironment.WebRootPath;
            return Path.Combine(basePath, AttachmentsFolder, fileName);
        }

        private string GetAttachmentRelativePath(string fileName)
        {
            return Path.Combine(AttachmentsFolder, fileName);
        }

        public Task<UploadedApk> SaveApk(IFormFile file)
        {
            throw new NotImplementedException();
        }

        public Task CheckApkSizeAsync(IFormFile file)
        {
            throw new NotImplementedException();
        }
    }
}