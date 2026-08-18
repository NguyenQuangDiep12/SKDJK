using SKDJK.Models.commons;

namespace SKDJK.Services.Interfaces
{
    public interface IUploadImage
    {
        public Task<Result<string>> UploadFileImage(IFormFile imagefile, string imagefolder, CancellationToken ct = default);
    }
}
