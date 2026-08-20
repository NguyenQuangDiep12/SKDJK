using SKDJK.Dtos;
using SKDJK.Models.commons;

namespace SKDJK.Services.Interfaces
{
    public interface IProgressService
    {
        Task<Result<ProgressDto>> GetAsync(int userId, CancellationToken cancellationToken = default);
    }
}
