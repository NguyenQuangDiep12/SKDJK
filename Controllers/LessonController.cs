using Microsoft.AspNetCore.Mvc;

namespace SKDJK.Controllers
{
    public class LessonController : Controller
    {
        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }
    }
}
