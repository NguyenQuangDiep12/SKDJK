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

            var dto = result.Value;

            var viewModel = new HomeViewModel
            {
                LearnedTopicCount = dto.LearnedTopicCount,

                CompletedTestCount = dto.CompletedTestCount,

                OverallProgress = dto.OverallProgress,

                ContinueLearning = dto.ContinueLearning == null
            ? null
            : new ContinueLessonViewModel
            {
                LessonId =
                    dto.ContinueLearning.LessonId,

                TopicId =
                    dto.ContinueLearning.TopicId,

                TopicName =
                    dto.ContinueLearning.TopicName,

                LessonTitle =
                    dto.ContinueLearning.LessonTitle,

                CompletionPercent =
                    dto.ContinueLearning.CompletionPercent
            },

                SuggestedTopics = dto.SuggestedTopics
                .Select(x => new SuggestedTopicViewModel
                {
                    TopicId = x.TopicId,
                    Name = x.Name,
                    Level = x.Level,
                    Description = x.Description,
                    ImageUrl = x.ImageUrl
                })
            .ToList()
            };

            return View();
        }


    }
}
