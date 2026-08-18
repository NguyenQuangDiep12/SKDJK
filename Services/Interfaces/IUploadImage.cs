namespace SKDJK.Services.Interfaces
{
    public interface IUploadImage
    {
        public Task<bool> UploadFileImage(IFormFile ImageFile);
    }
}
