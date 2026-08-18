using Microsoft.EntityFrameworkCore;
using SKDJK.Data;
using SKDJK.Dtos;
using SKDJK.Models;
using SKDJK.Models.commons;
using SKDJK.Services.Interfaces;

namespace SKDJK.Services;

public class HomeService : IHomeService
{
    private readonly ApplicationDbContext _dbContext;
    public HomeService(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }
    public async Task<Result<HomeDto>> GetAsync(int? userId, CancellationToken cancellationToken = default)
    {
        if(userId == null || userId <= 0)
        {
            return Result<HomeDto>.Failure(new Error("Auth.LoginUser", "Nguoi dung chua dang nhap tai khoan"));
        }

        // kiem tra nguoi dung ton tai
        var userExists = await _dbContext
            .Users
            .AsNoTracking()
            .FirstOrDefaultAsync(u => u.Id == userId, cancellationToken);

        if(userExists == null)
        {
            return Result<HomeDto>.Failure(new Error("Auth.EmailNotExist", "Tai khoan nguoi dung khong ton tai!"));
        }

        // query nhung bai hoc user da bat dau
        var userProgress = _dbContext
           .LearningProgress
           .AsNoTracking()
           .Where(lp => lp.UserId == userId &&
                  lp.Status != Models.enums.LearningStatus.NOTSTARTED);


        // 1. Query so chu de da hoc
        var learningTopicCount = await userProgress
            .Select(l => l.Lesson.Id)
            .Distinct()
            .ToListAsync(cancellationToken);

        // 2. So bai kiem tra da lam
        var completedTestCount = await _dbContext
            .TestResults
            .AsNoTracking()
            .Where(tr => tr.UserId == userId &&
                   tr.SubmittedAt != null)
            .Select(tr => tr.Id)
            .Distinct()
            .ToListAsync(cancellationToken);

        // 3. Tien do hoc tap tong the
        var overallProgress = await userProgress
            .Select(lp => (decimal?)lp.CompletionPercent)
            .AverageAsync(cancellationToken) ?? 0m;

        overallProgress = Math.Round(overallProgress, 0);

        // 4. Bai hoc dang hoc gan nhat
        var continueLearning = await userProgress
            .Where(lp => lp.Status == Models.enums.LearningStatus.INPROGRESS &&
                   lp.CompletionPercent < 100)
            .OrderByDescending(lp => lp.LastStudyAt)
            .Select(lp => new ContinueLearningDto
            {
                LessonId = lp.LessonId,
                CompletionPercent = lp.CompletionPercent,
                LessonTitle = lp.Lesson.Title,
                TopicId = lp.Lesson.TopicId,
                TopicName = lp.Lesson.Topic.Name
            })
            .FirstOrDefaultAsync(cancellationToken);

        // 5. nhung chu de da hoc gan nhat
        var learningTopicIds = userProgress
            .Select(lp => lp.Lesson.TopicId)
            .Distinct();

        // 6. Goi y 4 chu de chua hoc
        var suggestedTopic = await _dbContext.Topics
            .AsNoTracking()
            .Where(t => !learningTopicIds.Contains(t.Id))
            .OrderBy(t => t.Id)
            .Take(4)
            .Select(t => new SuggestedTopicDto
            {
                TopicId = t.Id,
                Name = t.Name,
                Level = t.Level,
                Description = t.Description,
                ImageUrl = t.ImageUrl
            }).ToListAsync(cancellationToken);

        var HomeDto = new HomeDto
        {
            LearnedTopicCount = learningTopicCount.Count,

            CompletedTestCount = completedTestCount.Count,

            OverallProgress = overallProgress,

            ContinueLearning = continueLearning,

            SuggestedTopics = suggestedTopic
        };

        return Result<HomeDto>
            .Success(HomeDto);
    }
}