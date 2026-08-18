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
            var userId = _contextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
            var result = await _homeService.GetAsync(Convert.ToInt32(userId), ct);

            if (!result.IsSuccess)
            {
                ViewBag.ErrorMessage = result.Error.Message;
                return View("Error");
            }

            var viewModel = new HomeViewModel
            {
                IsAuthenticated = User.Identity?.IsAuthenticated == true,
                FullName = User.Identity?.Name,
                CompletedTestCount = result.Value.CompletedTestCount,
                ContinueLesson = result.Value.ContinueLearning == null ? null : new ContinueLessonViewModel
                {
                    LessonId = result.Value.ContinueLearning.LessonId,
                    CompletionPercent = result.Value.ContinueLearning.CompletionPercent,
                    LessonTitle = result.Value.ContinueLearning.LessonTitle,
                    TopicName = result.Value.ContinueLearning.TopicName,
                },
                LearnedTopicCount = result.Value.LearnedTopicCount,
                OverallProgress = result.Value.OverallProgress,
                SuggestedTopics = result.Value.SuggestedTopics.Select(x => new SuggestedTopicViewModel
                {
                    TopicId = x.TopicId,
                    Name = x.Name,
                    Description = x.Description,
                    ImageUrl = x.ImageUrl,
                    Level = x.Level,
                }).ToList()
            };

            return View(viewModel);
        }


    }
}
