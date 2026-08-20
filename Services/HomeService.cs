using Microsoft.EntityFrameworkCore;
using SKDJK.Data;
using SKDJK.Dtos;
using SKDJK.Models.commons;
using SKDJK.Models.enums;
using SKDJK.Services.Interfaces;

namespace SKDJK.Services
{
    // Tổng hợp dashboard người học bằng các truy vấn đọc đơn giản, không hard-code số liệu.
    public sealed class HomeService : IHomeService
    {
        private readonly ApplicationDbContext _dbContext;

        public HomeService(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<Result<HomeDto>> GetAsync(int? userId, CancellationToken cancellationToken = default)
        {
            if (!userId.HasValue || userId.Value <= 0)
            {
                return Result<HomeDto>.Failure(new Error("Auth.LoginUser", "Người dùng chưa đăng nhập."));
            }

            bool userExists = await _dbContext.Users.AsNoTracking().AnyAsync(x => x.Id == userId.Value, cancellationToken);
            if (!userExists)
            {
                return Result<HomeDto>.Failure(new Error("User.NotFound", "Tài khoản người dùng không tồn tại."));
            }

            IQueryable<Models.LearningProgress> startedProgress = _dbContext.LearningProgress
                .AsNoTracking()
                .Where(x => x.UserId == userId.Value && x.Status != LearningStatus.NOTSTARTED);

            int learnedTopicCount = await startedProgress
                .Select(x => x.Lesson.TopicId)
                .Distinct()
                .CountAsync(cancellationToken);

            int completedTestCount = await _dbContext.TestResults
                .AsNoTracking()
                .CountAsync(x => x.UserId == userId.Value && x.SubmittedAt != null, cancellationToken);

            int totalLessonCount = await _dbContext.Lessons.AsNoTracking().CountAsync(cancellationToken);
            int completedLessonCount = await _dbContext.LearningProgress
                .AsNoTracking()
                .CountAsync(x => x.UserId == userId.Value && x.Status == LearningStatus.COMPLETED, cancellationToken);
            decimal overallProgress = totalLessonCount == 0
                ? 0
                : Math.Round(completedLessonCount * 100m / totalLessonCount, 0);

            ContinueLearningDto? continueLearning = await startedProgress
                .Where(x => x.Status == LearningStatus.INPROGRESS && x.CompletionPercent < 100)
                .OrderByDescending(x => x.LastStudyAt)
                .Select(x => new ContinueLearningDto
                {
                    LessonId = x.LessonId,
                    LessonTitle = x.Lesson.Title,
                    TopicId = x.Lesson.TopicId,
                    TopicName = x.Lesson.Topic.Name,
                    CompletionPercent = x.CompletionPercent
                })
                .FirstOrDefaultAsync(cancellationToken);

            IQueryable<int> learnedTopicIds = startedProgress.Select(x => x.Lesson.TopicId).Distinct();
            List<SuggestedTopicDto> suggestedTopics = await _dbContext.Topics
                .AsNoTracking()
                .Where(x => !learnedTopicIds.Contains(x.Id))
                .OrderBy(x => x.Id)
                .Take(4)
                .Select(x => new SuggestedTopicDto
                {
                    TopicId = x.Id,
                    Name = x.Name,
                    Level = x.Level,
                    Description = x.Description,
                    ImageUrl = x.ImageUrl
                })
                .ToListAsync(cancellationToken);

            HomeDto dto = new()
            {
                LearnedTopicCount = learnedTopicCount,
                CompletedTestCount = completedTestCount,
                OverallProgress = overallProgress,
                ContinueLearning = continueLearning,
                SuggestedTopics = suggestedTopics
            };
            return Result<HomeDto>.Success(dto);
        }
    }
}
