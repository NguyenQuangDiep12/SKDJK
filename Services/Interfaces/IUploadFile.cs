using Microsoft.AspNetCore.Http;
using SKDJK.Models.commons;

namespace SKDJK.Services.Interfaces
{
    public interface IUploadFile
    {
        Task<Result<string>> UploadFileImage(IFormFile imagefile, string imagefolder, CancellationToken ct = default);
        Task<Result<string>> UploadFileAudio(IFormFile audiofile, string audiofolder, CancellationToken ct = default);
    }
}