using SKDJK.Models;
using SKDJK.Models.commons;
using SKDJK.ViewModels;

namespace SKDJK.Services.Interfaces
{
    public interface ILessonService
    {
        // All
        Task<Result<LessonStudyViewModel>> GetLessonAsync(
            int lessonId,
            int userId,
            CancellationToken cancellationToken = default);

        Task<Result> UpdateProgressAsync(
            int lessonId,
            int userId,
            decimal completionPercent,
            CancellationToken cancellationToken = default);
        // Admin
        Task<Result<List<Lesson>>> GetAllAsync(
            CancellationToken cancellationToken = default);

        Task<Result<AdminLessonFormViewModel>> GetLessonFormAsync(
            int? id,
            CancellationToken cancellationToken = default);

        Task<Result<int>> SaveLessonAsync(
            AdminLessonFormViewModel model,
            CancellationToken cancellationToken = default);

        Task<Result> DeleteLessonAsync(
            int id,
            CancellationToken cancellationToken = default);

        Task<Result<List<Vocabulary>>> GetVocabulariesAsync(
            int lessonId,
            CancellationToken cancellationToken = default);

        Task<Result<AdminVocabularyFormViewModel>> GetVocabularyFormAsync(
            int lessonId,
            int? id,
            CancellationToken cancellationToken = default);

        Task<Result> SaveVocabularyAsync(
            AdminVocabularyFormViewModel model,
            CancellationToken cancellationToken = default);

        Task<Result> DeleteVocabularyAsync(
            int id,
            CancellationToken cancellationToken = default);

        Task<Result<List<LessonSection>>> GetSectionsAsync(
            int lessonId,
            CancellationToken cancellationToken = default);

        Task<Result<AdminLessonSectionFormViewModel>> GetSectionFormAsync(
            int lessonId,
            int? id,
            CancellationToken cancellationToken = default);

        Task<Result> SaveSectionAsync(
            AdminLessonSectionFormViewModel model,
            CancellationToken cancellationToken = default);

        Task<Result> DeleteSectionAsync(
            int id,
            CancellationToken cancellationToken = default);
    }
}
