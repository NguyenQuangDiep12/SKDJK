using Microsoft.AspNetCore.Mvc;


namespace SKDJK.Controllers
{
    public class TopicController : Controller
    {
        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }
        [HttpGet]
        public IActionResult Details()
        {
            return View();
        }
    }
}
