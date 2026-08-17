
using SKDJK.Models;
using SKDJK.Models.commons;
using SKDJK.ViewModels;

namespace SKDJK.Services.Interfaces
{
    public interface ITopicService
    {
        // All
        Task<Result<TopicListViewModel>> GetTopicsAsync(
            string? search,
            int? languageId,
            string? level,
            int page = 1,
            int pageSize = 12,
            CancellationToken cancellationToken = default);

        Task<Result<TopicDetailsViewModel>> GetDetailsAsync(
            int topicId,
            int? userId,
            CancellationToken cancellationToken = default);
        // Admin
        Task<Result<List<Topic>>> GetAllAsync(
            CancellationToken cancellationToken = default);

        Task<Result<AdminTopicFormViewModel>> GetFormAsync(
            int? id,
            CancellationToken cancellationToken = default);

        Task<Result<int>> SaveAsync(
            AdminTopicFormViewModel model,
            CancellationToken cancellationToken = default);

        Task<Result> DeleteAsync(
            int id,
            CancellationToken cancellationToken = default);
    }
}
