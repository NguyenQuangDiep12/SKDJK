using Microsoft.AspNetCore.Mvc;
using SKDJK.Services.Interfaces;
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

            return View();
        }


    }
}
