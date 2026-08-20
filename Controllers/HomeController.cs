using Microsoft.AspNetCore.Mvc;
using SKDJK.Services.Interfaces;
using SKDJK.ViewModels;
using System.Security.Claims;

namespace SKDJK.Controllers
{
    public class HomeController : Controller
    {
        private readonly IHomeService _homeService;
        private readonly IHttpContextAccessor _contextAccessor;
        public HomeController(IHomeService homeService, IHttpContextAccessor contextAccessor)
        {
            _homeService = homeService;
            _contextAccessor = contextAccessor;
        }
        [HttpGet]
        public async Task<IActionResult> Index(CancellationToken ct)
        {
            if (User.Identity?.IsAuthenticated != true)
            {
                var guestViewModel = new HomeViewModel
                    {
                        IsAuthenticated = false,
                        FullName = "Người học"
                    };
                return View(guestViewModel);
            }
            // Đã đăng nhập
            var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!int.TryParse(userIdClaim, out var userId))
            {
                return Unauthorized();
            }
            var result = await _homeService.GetAsync(userId, ct);
            if (!result.IsSuccess)
            {
                ViewBag.ErrorMessage = result.Error.Message;

                return View(new HomeViewModel
                    {
                        IsAuthenticated = true,
                        FullName =
                            User.Identity?.Name
                            ?? "Người học"
                    }
                );
            }
            var dto = result.Value;
            var viewModel = new HomeViewModel{
                IsAuthenticated = true,
                FullName = User.Identity?.Name ?? "Người học",
                CompletedTestCount = dto.CompletedTestCount,
                LearnedTopicCount = dto.LearnedTopicCount,
                OverallProgress = dto.OverallProgress,
                ContinueLesson = dto.ContinueLearning == null ? null : new ContinueLessonViewModel
                {
                    LessonId = dto.ContinueLearning.LessonId,

                    LessonTitle = dto.ContinueLearning.LessonTitle,

                    TopicName = dto.ContinueLearning.TopicName,

                    CompletionPercent = dto.ContinueLearning.CompletionPercent
                },
                SuggestedTopics = dto.SuggestedTopics
                    .Select(x =>
                        new SuggestedTopicViewModel
                        {
                            TopicId = x.TopicId,
                            Name = x.Name,
                            Description = x.Description,
                            ImageUrl = x.ImageUrl,
                            Level = x.Level
                        })
                    .ToList()
            };
            return View(viewModel);
        }
    }
}
