using CloudinaryDotNet;
using SKDJK.Models.commons;
using SKDJK.Services.Interfaces;

namespace SKDJK.Services
{
    public class CloudinaryImageUploadFileProvider : IUploadImage
    {
        private readonly Cloudinary _cloudinary;
        public CloudinaryImageUploadFileProvider(Cloudinary cloudinary)
        {
            _cloudinary = cloudinary;
        }

        private readonly string[] _allowExtension = { ".jpg", ".jpeg", ".png", ".gif" };
        private const long _maxFileSize = 5 * 1024 * 1024; // 5 mb
        public async Task<bool> UploadFileImage(IFormFile ImageFile)
        {
            throw new NotImplementedException();
        }
    }
}
