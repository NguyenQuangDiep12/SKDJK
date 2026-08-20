using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using SKDJK.Models.commons;
using SKDJK.Services.Interfaces;
using AppError = SKDJK.Models.commons.Error;

namespace SKDJK.Services
{
    public class CloudinaryOption
    {
        public string CloudName { get; set; } = string.Empty;
        public string ApiKey { get; set; } = string.Empty;
        public string ApiSecret { get; set; } = string.Empty;
        public string RootFolder { get; set; } = string.Empty;
    }
    public class CloudinaryMediaService : IUploadFile
    {
        private readonly Cloudinary _cloudinary;
        private readonly string _rootFolder;

        // Validate Ảnh
        private static readonly string[] _allowedImageExtensions = { ".jpg", ".jpeg", ".png", ".webp" };
        private static readonly string[] _allowedImageMimeTypes = { "image/jpeg", "image/png", "image/webp" };
        private const long _maxImageSize = 5 * 1024 * 1024; // 5MB

        // Validate Audio
        private static readonly string[] _allowedAudioExtensions = { ".mp3", ".wav", ".m4a", ".aac" };
        private static readonly string[] _allowedAudioMimeTypes = { "audio/mpeg", "audio/wav", "audio/x-m4a", "audio/aac", "audio/mp3" };
        private const long _maxAudioSize = 20 * 1024 * 1024; // 20MB

        public CloudinaryMediaService(IOptions<CloudinaryOption> options)
        {
            var config = options.Value;
            var account = new Account(config.CloudName, config.ApiKey, config.ApiSecret);
            _cloudinary = new Cloudinary(account);
            _rootFolder = config.RootFolder.Trim().Trim('/');
        }

        // 1. UPLOAD ẢNH (Khớp với Interface UploadFileImage)
        public async Task<Result<string>> UploadFileImage(IFormFile imagefile, string imagefolder, CancellationToken ct = default)
        {
            // Chỉ cho phép Controller chọn đúng ba thư mục ảnh đã quy ước trên Cloudinary.
            string normalizedFolder = imagefolder.Trim().Trim('/').ToLowerInvariant();
            if (normalizedFolder != "topics" && normalizedFolder != "questions" && normalizedFolder != "vocabularies")
                return Result<string>.Failure(new AppError("Upload.Folder", "Thư mục lưu ảnh không hợp lệ."));

            // RootFolder phải được cấu hình để file không bị tải nhầm ra thư mục gốc của Cloudinary.
            if (string.IsNullOrWhiteSpace(_rootFolder))
                return Result<string>.Failure(new AppError("Upload.Configuration", "Chưa cấu hình thư mục gốc Cloudinary."));

            if (imagefile == null || imagefile.Length == 0 || imagefile.Length > _maxImageSize)
                return Result<string>.Failure(new AppError("Upload.Image", "File ảnh không hợp lệ hoặc vượt quá 5MB"));

            var ext = Path.GetExtension(imagefile.FileName).ToLowerInvariant();
            if (!_allowedImageExtensions.Contains(ext) || !_allowedImageMimeTypes.Contains(imagefile.ContentType.ToLowerInvariant()))
                return Result<string>.Failure(new AppError("Upload.Image", "Định dạng ảnh không được hỗ trợ"));

            try
            {
                await using var stream = imagefile.OpenReadStream();
                var uploadParams = new ImageUploadParams
                {
                    File = new FileDescription(Path.GetFileName(imagefile.FileName), stream),
                    Folder = $"{_rootFolder}/{normalizedFolder}",
                    UniqueFilename = true,
                    Overwrite = false
                };

                var uploadResult = await _cloudinary.UploadAsync(uploadParams, ct);

                if (uploadResult.Error != null || uploadResult.SecureUrl == null)
                    return Result<string>.Failure(new AppError("Upload.Image", uploadResult.Error?.Message ?? "Lỗi upload ảnh"));

                return Result<string>.Success(uploadResult.SecureUrl.ToString());
            }
            catch (Exception ex)
            {
                return Result<string>.Failure(new AppError("Upload.Image", $"Không thể tải ảnh lên Cloudinary: {ex.Message}"));
            }
        }

        // 2. UPLOAD AUDIO (Khớp với Interface UploadFileAudio)
        public async Task<Result<string>> UploadFileAudio(IFormFile audiofile, string audiofolder, CancellationToken ct = default)
        {
            // Audio của câu hỏi chỉ được lưu vào thư mục questionaudio như cấu trúc trên Cloudinary.
            string normalizedFolder = audiofolder.Trim().Trim('/').ToLowerInvariant();
            if (normalizedFolder != "questionaudio")
                return Result<string>.Failure(new AppError("Upload.Folder", "Thư mục lưu audio không hợp lệ."));

            // Không upload khi thiếu RootFolder vì sẽ làm sai cấu trúc thư mục yêu cầu.
            if (string.IsNullOrWhiteSpace(_rootFolder))
                return Result<string>.Failure(new AppError("Upload.Configuration", "Chưa cấu hình thư mục gốc Cloudinary."));

            if (audiofile == null || audiofile.Length == 0 || audiofile.Length > _maxAudioSize)
                return Result<string>.Failure(new AppError("Upload.Audio", "File audio không hợp lệ hoặc vượt quá 20MB"));

            var ext = Path.GetExtension(audiofile.FileName).ToLowerInvariant();
            if (!_allowedAudioExtensions.Contains(ext) || !_allowedAudioMimeTypes.Contains(audiofile.ContentType.ToLowerInvariant()))
                return Result<string>.Failure(new AppError("Upload.Audio", "Định dạng audio không hỗ trợ (chỉ nhận mp3, wav, m4a, aac)"));

            try
            {
                await using var stream = audiofile.OpenReadStream();

                var uploadParams = new VideoUploadParams
                {
                    File = new FileDescription(Path.GetFileName(audiofile.FileName), stream),
                    Folder = $"{_rootFolder}/{normalizedFolder}",
                    UniqueFilename = true,
                    Overwrite = false
                };

                var uploadResult = await _cloudinary.UploadAsync(uploadParams, ct);

                if (uploadResult.Error != null || uploadResult.SecureUrl == null)
                    return Result<string>.Failure(new AppError("Upload.Audio", uploadResult.Error?.Message ?? "Lỗi upload audio"));

                return Result<string>.Success(uploadResult.SecureUrl.ToString());
            }
            catch (Exception ex)
            {
                return Result<string>.Failure(new AppError("Upload.Audio", $"Không thể tải audio lên Cloudinary: {ex.Message}"));
            }
        }
    }
}
