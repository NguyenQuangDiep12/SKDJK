using SKDJK.Dtos;
using SKDJK.Models.commons;
using SKDJK.Models.enums;

namespace SKDJK.Services.Interfaces
{
    // Service Test chỉ nhận và trả DTO; Controller chịu trách nhiệm ánh xạ ViewModel.
    public interface ITestService
    {
        Task<Result<TestListDto>> GetTestsAsync(int userId, string? search, TestFormat? format, TestMode? mode, string? level, int page = 1, int pageSize = 10, CancellationToken cancellationToken = default);
        Task<Result<TakeTestDto>> GetTakeAsync(int testId, CancellationToken cancellationToken = default);
        Task<Result<int>> SubmitAsync(int userId, SubmitTestDto request, CancellationToken cancellationToken = default);
        Task<Result<TestResultDto>> GetResultAsync(int userId, int resultId, CancellationToken cancellationToken = default);
        Task<Result<TestHistoryDto>> GetHistoryAsync(int userId, CancellationToken cancellationToken = default);
        Task<Result<AdminTestListDto>> GetAdminAsync(string? search, TestFormat? format, TestMode? mode, CancellationToken cancellationToken = default);
        Task<Result<AdminTestFormDto>> GetTestFormAsync(int? id, CancellationToken cancellationToken = default);
        Task<Result<int>> SaveTestAsync(AdminTestFormDto request, CancellationToken cancellationToken = default);
        Task<Result> DeleteTestAsync(int id, CancellationToken cancellationToken = default);
        Task<Result<AdminQuestionListDto>> GetQuestionsAsync(int testId, CancellationToken cancellationToken = default);
        Task<Result<AdminQuestionFormDto>> GetQuestionFormAsync(int testId, int? id, CancellationToken cancellationToken = default);
        Task<Result> SaveQuestionAsync(AdminQuestionFormDto request, CancellationToken cancellationToken = default);
        Task<Result> DeleteQuestionAsync(int id, CancellationToken cancellationToken = default);
    }
}
