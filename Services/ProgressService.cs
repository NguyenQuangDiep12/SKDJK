using Microsoft.EntityFrameworkCore;
using SKDJK.Data;
using SKDJK.Dtos;
using SKDJK.Models.commons;
using SKDJK.Models.enums;
using SKDJK.Services.Interfaces;

namespace SKDJK.Services
{
    // Tính dashboard tiến độ hoàn toàn từ LearningProgress và TestResult của user hiện tại.
    public sealed class ProgressService : IProgressService
    {
        private readonly ApplicationDbContext _dbContext;

        public ProgressService(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<Result<ProgressDto>> GetAsync(int userId, CancellationToken cancellationToken = default)
        {
            bool userExists = await _dbContext.Users.AsNoTracking().AnyAsync(x => x.Id == userId, cancellationToken);
            if (!userExists)
            {
                return Result<ProgressDto>.Failure(new Error("User.NotFound", "Không tìm thấy người học."));
            }

            int totalLessonCount = await _dbContext.Lessons.AsNoTracking().CountAsync(cancellationToken);
            int completedLessonCount = await _dbContext.LearningProgress
                .AsNoTracking()
                .CountAsync(x => x.UserId == userId && x.Status == LearningStatus.COMPLETED, cancellationToken);
            int totalTestCount = await _dbContext.Tests.AsNoTracking().CountAsync(cancellationToken);
            int completedTestCount = await _dbContext.TestResults
                .AsNoTracking()
                .CountAsync(x => x.UserId == userId && x.SubmittedAt != null, cancellationToken);

            decimal overallProgress = totalLessonCount == 0
                ? 0
                : Math.Round(completedLessonCount * 100m / totalLessonCount, 0);

            List<TopicProgressDto> topicProgresses = await _dbContext.Topics
                .AsNoTracking()
                .Where(topic => topic.Lessons.Any())
                .OrderBy(topic => topic.Name)
                .Select(topic => new TopicProgressDto
                {
                    TopicId = topic.Id,
                    TopicName = topic.Name,
                    ProgressPercent = topic.Lessons.Count == 0
                        ? 0
                        : topic.Lessons
                            .SelectMany(lesson => lesson.LearningProgresses.Where(progress => progress.UserId == userId))
                            .Select(progress => (decimal?)progress.CompletionPercent)
                            .Average() ?? 0
                })
                .ToListAsync(cancellationToken);

            List<CompletedLessonDto> completedLessons = await _dbContext.LearningProgress
                .AsNoTracking()
                .Where(x => x.UserId == userId && x.Status == LearningStatus.COMPLETED)
                .OrderByDescending(x => x.LastStudyAt)
                .Take(10)
                .Select(x => new CompletedLessonDto
                {
                    LessonId = x.LessonId,
                    LessonTitle = x.Lesson.Title,
                    TopicName = x.Lesson.Topic.Name,
                    CompletedAt = x.LastStudyAt
                })
                .ToListAsync(cancellationToken);

            List<TestHistoryItemDto> history = await _dbContext.TestResults
                .AsNoTracking()
                .Where(x => x.UserId == userId)
                .OrderByDescending(x => x.SubmittedAt)
                .Take(20)
                .Select(x => new TestHistoryItemDto
                {
                    ResultId = x.Id,
                    TestId = x.TestId,
                    TestTitle = x.Test.Title,
                    TopicName = x.Test.Lesson.Topic.Name,
                    Format = x.Test.Format,
                    Mode = x.Test.Mode,
                    Score = x.Score,
                    CorrectCount = x.CorrectCount,
                    TotalQuestions = x.TotalQuestions > 0 ? x.TotalQuestions : x.Test.Questions.Count,
                    SubmittedAt = x.SubmittedAt
                })
                .ToListAsync(cancellationToken);

            ProgressDto dto = new()
            {
                CompletedLessonCount = completedLessonCount,
                TotalLessonCount = totalLessonCount,
                CompletedTestCount = completedTestCount,
                TotalTestCount = totalTestCount,
                OverallProgress = overallProgress,
                TopicProgresses = topicProgresses,
                CompletedLessons = completedLessons,
                TestHistory = history
            };

            return Result<ProgressDto>.Success(dto);
        }
    }
}
