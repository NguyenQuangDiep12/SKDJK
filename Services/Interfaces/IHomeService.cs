using SKDJK.Dtos;
using SKDJK.Models.commons;

namespace SKDJK.Services.Interfaces
{
    public interface IHomeService
    {
        Task<Result<HomeDto>> GetAsync(int? userId, CancellationToken cancellationToken = default);
    }
}
