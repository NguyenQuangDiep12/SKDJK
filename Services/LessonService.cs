using Microsoft.EntityFrameworkCore;
using SKDJK.Data;
using SKDJK.Dtos;
using SKDJK.Models.commons;
using SKDJK.Services.Interfaces;

namespace SKDJK.Services;
public class LessonService : ILessonService
{
    private readonly ApplicationDbContext _context;
    public LessonService(ApplicationDbContext dbContext)
    {
        _context = dbContext;
    }
    public async Task<Result<VocabularyLearningDto>> GetVocabularyAsync(int lessonId, CancellationToken ct = default)
    {
        var lesson = await _context
            .Lessons
            .AsNoTracking()
            .Where(l => l.Id == lessonId)
            .Select(l => new VocabularyLearningDto
            {
                LessonId = l.Id,
                LessonTitle = l.Title,
                Vocabularies = l.Vocabularies
                .Select(l => new VocabularyItemDto
                {
                    Word = l.Word,
                    AudioUrl = l.AudioUrl,
                    Example = l.Example,
                    Meaning = l.Meaning,
                    Pronunciation = l.Pronunciation,
                    VocabularyId = l.Id
                }).ToList()
            })
            .FirstOrDefaultAsync(ct);
            
        if(lesson == null)
        {
            return Result<VocabularyLearningDto>.Failure(new Error("User.FindVocabulary", "Khong tim thay bai hoc tuong ung"));
        }

        return Result<VocabularyLearningDto>.Success(lesson);
    }

}