using SKDJK.Models.commons;
using SKDJK.ViewModels;

namespace SKDJK.Services.Interfaces
{
    public interface IProgressService
    {
        Task<Result<ProgressViewModel>> GetAsync(
            int userId,
            CancellationToken cancellationToken = default);
    }
}
