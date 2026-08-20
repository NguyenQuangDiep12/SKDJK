using SKDJK.Dtos;
using SKDJK.Models.commons;

namespace SKDJK.Services.Interfaces
{
    // Service chỉ nhận/trả DTO; ViewModel dừng ở Controller.
    public interface ILanguageService
    {
        Task<Result<AdminLanguageListDto>> GetAllAsync(string? search, CancellationToken cancellationToken = default);
        Task<Result<AdminLanguageFormDto>> GetByIdAsync(int id, CancellationToken cancellationToken = default);
        Task<Result<int>> CreateAsync(CreateLanguageDto request, CancellationToken cancellationToken = default);
        Task<Result> UpdateAsync(int id, UpdateLanguageDto request, CancellationToken cancellationToken = default);
        Task<Result> DeleteAsync(int id, CancellationToken cancellationToken = default);
    }
}
