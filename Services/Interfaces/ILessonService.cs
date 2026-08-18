using SKDJK.Dtos;
using SKDJK.Models.commons;

namespace SKDJK.Services.Interfaces;

public interface ILessonService
{
    Task<Result<VocabularyLearningDto>> GetVocabularyAsync(int lessonId, CancellationToken ct = default);
}