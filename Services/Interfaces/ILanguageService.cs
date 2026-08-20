using SKDJK.Dtos;
using SKDJK.Models.commons;

namespace SKDJK.Services.Interfaces
{
    // Service chỉ nhận/trả DTO; ViewModel dừng ở Controller.
    public interface ILanguageService
    {
        Task<Result<AdminLanguageListDto>> GetAsync(string? search, CancellationToken cancellationToken = default);
        Task<Result<AdminLanguageFormDto>> GetFormAsync(int? id, CancellationToken cancellationToken = default);
        Task<Result<int>> SaveAsync(AdminLanguageFormDto request, CancellationToken cancellationToken = default);
        Task<Result> DeleteAsync(int id, CancellationToken cancellationToken = default);
    }
}
