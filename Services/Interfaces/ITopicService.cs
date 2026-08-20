using SKDJK.Dtos;
using SKDJK.Models.commons;

namespace SKDJK.Services.Interfaces
{
    // Service Topic chỉ dùng DTO ở biên Controller.
    public interface ITopicService
    {
        Task<Result<TopicListDto>> GetAllAsync(string? search, int? languageId, string? level, int page = 1, int pageSize = 12, CancellationToken cancellationToken = default);
        Task<Result<TopicDetailsDto>> GetByIdAsync(int id, int? userId = null, CancellationToken cancellationToken = default);
        Task<Result<AdminTopicListDto>> GetAdminAsync(string? search, int? languageId, string? level, CancellationToken cancellationToken = default);
        Task<Result<AdminTopicFormDto>> GetFormAsync(int? id, CancellationToken cancellationToken = default);
        Task<Result<int>> CreateAsync(CreateTopicDto request, CancellationToken cancellationToken = default);
        Task<Result> UpdateAsync(int id, UpdateTopicDto request, CancellationToken cancellationToken = default);
        Task<Result> DeleteAsync(int id, CancellationToken cancellationToken = default);
    }
}
