using SKDJK.Models;
using SKDJK.Models.commons;
using SKDJK.ViewModels;

namespace SKDJK.Services.Interfaces
{
    public interface ITestService
    {
        // All
        Task<Result<TestListViewModel>> GetTestsAsync(
            int userId,
            string? search,
            string? level,
            CancellationToken cancellationToken = default);

        Task<Result<TakeTestViewModel>> GetTakeAsync(
            int testId,
            CancellationToken cancellationToken = default);

        Task<Result<int>> SubmitAsync(
            int userId,
            SubmitTestViewModel model,
            CancellationToken cancellationToken = default);

        Task<Result<TestResultViewModel>> GetResultAsync(
            int userId,
            int resultId,
            CancellationToken cancellationToken = default);

        Task<Result<TestHistoryViewModel>> GetHistoryAsync(
            int userId,
            CancellationToken cancellationToken = default);

        // Admin
        Task<Result<List<Test>>> GetAllAsync(
            CancellationToken cancellationToken = default);

        Task<Result<AdminTestFormViewModel>> GetTestFormAsync(
            int? id,
            CancellationToken cancellationToken = default);

        Task<Result<int>> SaveTestAsync(
            AdminTestFormViewModel model,
            CancellationToken cancellationToken = default);

        Task<Result> DeleteTestAsync(
            int id,
            CancellationToken cancellationToken = default);

        Task<Result<List<Question>>> GetQuestionsAsync(
            int testId,
            CancellationToken cancellationToken = default);

        Task<Result<AdminQuestionFormViewModel>> GetQuestionFormAsync(
            int testId,
            int? id,
            CancellationToken cancellationToken = default);

        Task<Result> SaveQuestionAsync(
            AdminQuestionFormViewModel model,
            CancellationToken cancellationToken = default);

        Task<Result> DeleteQuestionAsync(
            int id,
            CancellationToken cancellationToken = default);
    }
}
