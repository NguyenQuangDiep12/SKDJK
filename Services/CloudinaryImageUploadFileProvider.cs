using CloudinaryDotNet;
using SKDJK.Services.Interfaces;
using Microsoft.Extensions.Options;
using CloudinaryDotNet.Actions;
using SKDJK.Models.commons;

namespace SKDJK.Services
{
    public class CloudinaryOption
    {
        public string CloudName { get; set; } = string.Empty;
        public string ApiKey { get; set; } = string.Empty;
        public string ApiSecret { get; set; } = string.Empty;
    }
    public class CloudinaryImageUploadFileProvider : IUploadImage
    {
        private readonly CloudinaryOption _cloudinaryOption;
        public CloudinaryImageUploadFileProvider( IOptions<CloudinaryOption> options)
        {
            _cloudinaryOption = options.Value;
        }

        private static readonly string[] _allowExtension = { ".jpg", ".jpeg", ".png", ".gif" };
        private static readonly string[] _allowedContentTypes = { "image/jpeg", "image/png", "image/gif", "image/webp" };
        private const long _maxFileSize = 5 * 1024 * 1024; // 5 mb

        public async Task<Result<string>> UploadFileImage(IFormFile imagefile, string imagefolder, CancellationToken ct = default)
        {
            if(imagefile == null || imagefile.Length == 0 || imagefile.Length > _maxFileSize)
            {
                return Result<string>.Failure(new Models.commons.Error("User.UploadFile", "Anh khong duoc tai len hoac vuot qua kich thuoc 5mb"));
            }
            var extension = Path.GetExtension(imagefile.FileName);
            if(!_allowExtension.Contains(extension) || !_allowedContentTypes.Contains(imagefile.ContentType))
            {
                return Result<string>.Failure(new Models.commons.Error("User.UploadImage", $"Anh khong dung dinh dang cho phep{string.Join(",", _allowExtension)}"));
            }

            if(string.IsNullOrWhiteSpace(_cloudinaryOption.CloudName) ||
               string.IsNullOrWhiteSpace(_cloudinaryOption.ApiKey)    ||
               string.IsNullOrWhiteSpace(_cloudinaryOption.ApiSecret))
            {
                return Result<string>.Failure(new Models.commons.Error("Provider.Configuration", "Cau hinh Cloudinary khong dung hoac thieu"));
            }

            try
            {
                ct.ThrowIfCancellationRequested();

                var cloudinary = new Cloudinary(new Account(_cloudinaryOption.CloudName, _cloudinaryOption.ApiKey, _cloudinaryOption.ApiSecret));

                await using var stream = imagefile.OpenReadStream();
                var uploadParam = new ImageUploadParams
                {
                    File = new FileDescription(imagefile.FileName, stream),
                    Folder = $"skdjk/project/{imagefolder}",
                    UseFilename = false,
                    UniqueFilename = true,
                    Overwrite = false
                };

                var upload = await cloudinary.UploadAsync(uploadParam);
                ct.ThrowIfCancellationRequested();

                if(upload.Error != null || upload.SecureUrl == null)
                {
                    return Result<string>.Failure(new Models.commons.Error("User.UploadImage", "tai anh len that bai"));
                }
                return Result<string>.Success(upload.SecureUrl.ToString());
            }catch(Exception)
            {
                throw;
            }
        }
    }
}
