using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SKDJK.Services.Interfaces;
using SKDJK.ViewModels;

namespace SKDJK.Controllers
{
    // Chỉ lấy tiến độ của người dùng hiện tại từ claim.
    [Authorize]
    public sealed class ProgressController : Controller
    {
        private readonly IProgressService _progressService;

        public ProgressController(IProgressService progressService)
        {
            _progressService = progressService;
        }

        [HttpGet]
        public async Task<IActionResult> Index(CancellationToken cancellationToken = default)
        {
            if (!User.TryGetUserId(out int userId))
            {
                return Unauthorized();
            }

            var result = await _progressService.GetAsync(userId, cancellationToken);
            if (!result.IsSuccess)
            {
                return BadRequest(result.Error.Message);
            }

            // Controller đổi DTO tiến độ thành ViewModel trước khi trả về Razor View.
            var model = new ProgressViewModel
            {
                CompletedLessonCount = result.Value.CompletedLessonCount,
                TotalLessonCount = result.Value.TotalLessonCount,
                CompletedTestCount = result.Value.CompletedTestCount,
                TotalTestCount = result.Value.TotalTestCount,
                OverallProgress = result.Value.OverallProgress,
                TopicProgresses = result.Value.TopicProgresses.Select(item => new TopicProgressViewModel
                {
                    TopicId = item.TopicId,
                    TopicName = item.TopicName,
                    ProgressPercent = item.ProgressPercent
                }).ToList(),
                CompletedLessons = result.Value.CompletedLessons.Select(item => new CompletedLessonViewModel
                {
                    LessonId = item.LessonId,
                    LessonTitle = item.LessonTitle,
                    TopicName = item.TopicName,
                    CompletedAt = item.CompletedAt
                }).ToList(),
                TestHistory = result.Value.TestHistory.Select(item => new TestHistoryItemViewModel
                {
                    ResultId = item.ResultId,
                    TestId = item.TestId,
                    TestTitle = item.TestTitle,
                    TopicName = item.TopicName,
                    Score = item.Score,
                    CorrectCount = item.CorrectCount,
                    TotalQuestions = item.TotalQuestions,
                    SubmittedAt = item.SubmittedAt,
                    Format = item.Format,
                    Mode = item.Mode
                }).ToList()
            };

            return View(model);
        }
    }
}
