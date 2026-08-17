using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SKDJK.Models.commons;
using SKDJK.Services.Interfaces;
using System.Security.Claims;

namespace SKDJK.Controllers
{
    public class AuthController : Controller
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(
            string fullName,
            string email,
            string password)
        {
            // Không còn ViewModel nên validate thủ công
            if (string.IsNullOrWhiteSpace(fullName))
            {
                ModelState.AddModelError(
                    "FullName",
                    "Vui lòng nhập họ tên");
            }

            if (string.IsNullOrWhiteSpace(email))
            {
                ModelState.AddModelError(
                    "Email",
                    "Vui lòng nhập email");
            }

            if (string.IsNullOrWhiteSpace(password))
            {
                ModelState.AddModelError(
                    "Password",
                    "Vui lòng nhập mật khẩu");
            }

            if (!ModelState.IsValid)
            {
                return View();
            }

            var result = await _authService.RegisterAsync(
                fullName,
                email,
                password);

            if (!result.IsSuccess)
            {
                if (result.Error == Error.EmailAlreadyExist)
                {
                    ModelState.AddModelError(
                        "Email",
                        result.Error.Message);
                }
                else
                {
                    ModelState.AddModelError(
                        string.Empty,
                        result.Error.Message);
                }

                return View();
            }

            return RedirectToAction(nameof(Login));
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(
            string email,
            string password,
            bool rememberMe)
        {
            // Validate thủ công
            if (string.IsNullOrWhiteSpace(email))
            {
                ModelState.AddModelError(
                    "Email",
                    "Vui lòng nhập email");
            }

            if (string.IsNullOrWhiteSpace(password))
            {
                ModelState.AddModelError(
                    "Password",
                    "Vui lòng nhập mật khẩu");
            }

            if (!ModelState.IsValid)
            {
                return View();
            }

            var result = await _authService.LoginAsync(
                email,
                password);

            if (!result.IsSuccess)
            {
                ModelState.AddModelError(
                    string.Empty,
                    result.Error.Message);

                return View();
            }

            var user = result.Value!;

            var claims = new List<Claim>
            {
                new Claim(
                    ClaimTypes.NameIdentifier,
                    user.UserId.ToString()),

                new Claim(
                    ClaimTypes.Name,
                    user.FullName),

                new Claim(
                    ClaimTypes.Email,
                    user.Email),

                new Claim(
                    ClaimTypes.Role,
                    user.RoleName)
            };

            var identity = new ClaimsIdentity(
                claims,
                CookieAuthenticationDefaults.AuthenticationScheme);

            var principal = new ClaimsPrincipal(identity);

            var authenticationProperties =
                new AuthenticationProperties
                {
                    IsPersistent = rememberMe
                };

            await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                principal,
                authenticationProperties);

            return RedirectToAction(
                "Index",
                "Home");
        }

        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(
                CookieAuthenticationDefaults.AuthenticationScheme);

            HttpContext.Session.Clear();

            return RedirectToAction(
                "Index",
                "Home");
        }

        [HttpGet]
        public IActionResult Forbidden()
        {
            return View();
        }

        [HttpGet]
        public IActionResult Notfound()
        {
            return View();
        }
    }
}