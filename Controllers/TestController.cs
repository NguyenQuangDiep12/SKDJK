using Microsoft.AspNetCore.Mvc;


namespace SKDJK.Controllers
{
    public class TestController : Controller
    {
        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }
        [HttpGet]
        public IActionResult Result()
        {
            return View();
        }
        [HttpGet]
        public IActionResult Take()
        {
            return View();
        }
    }
}
