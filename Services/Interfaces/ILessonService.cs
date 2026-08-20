using SKDJK.Dtos;
using SKDJK.Models.commons;

namespace SKDJK.Services.Interfaces
{
    // Tất cả dữ liệu đi qua Service/Controller là DTO, không phải ViewModel.
    public interface ILessonService
    {
        Task<Result<MyLessonPageDto>> GetMyLessonsAsync(int userId, CancellationToken cancellationToken = default);
        Task<Result<LessonStudyDto>> GetStudyAsync(int lessonId, int userId, CancellationToken cancellationToken = default);
        Task<Result<VocabularyLearningDto>> GetVocabularyAsync(int lessonId, CancellationToken cancellationToken = default);
        Task<Result<GrammarLearningDto>> GetGrammarAsync(int lessonId, CancellationToken cancellationToken = default);
        Task<Result<ListeningLearningDto>> GetListeningAsync(int lessonId, CancellationToken cancellationToken = default);
        Task<Result> CompleteAsync(int lessonId, int userId, CancellationToken cancellationToken = default);
        Task<Result<AdminLessonPageDto>> GetAdminPageAsync(int? selectedLessonId, CancellationToken cancellationToken = default);
        Task<Result<AdminLessonFormDto>> GetLessonFormAsync(int? id, CancellationToken cancellationToken = default);
        Task<Result<int>> SaveLessonAsync(AdminLessonFormDto request, CancellationToken cancellationToken = default);
        Task<Result> DeleteLessonAsync(int id, CancellationToken cancellationToken = default);
        Task<Result<AdminVocabularyFormDto>> GetVocabularyFormAsync(int lessonId, int? id, CancellationToken cancellationToken = default);
        Task<Result> SaveVocabularyAsync(AdminVocabularyFormDto request, CancellationToken cancellationToken = default);
        Task<Result> DeleteVocabularyAsync(int id, CancellationToken cancellationToken = default);
    }
}
