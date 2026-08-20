using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SKDJK.Dtos;
using SKDJK.Models.commons;
using SKDJK.Services.Interfaces;
using SKDJK.ViewModels;
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
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }


            // Controller đổi ViewModel của form thành DTO trước khi gọi Service.
            var request = new RegisterRequestDto
            {
                FullName = model.FullName,
                Email = model.Email,
                Password = model.Password
            };

            // Service chỉ nhận DTO nên không phụ thuộc vào giao diện Razor.
            var result = await _authService.RegisterAsync(request);

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

                return View(model);
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
        public async Task<IActionResult> Login(LoginViewModel model)
        {

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            // Controller đổi dữ liệu form đăng nhập thành DTO cho Service.
            var request = new LoginRequestDto
            {
                Email = model.Email,
                Password = model.Password
            };

            // Service xử lý DTO và trả về DTO người dùng đã xác thực.
            var result = await _authService.LoginAsync(request);

            if (!result.IsSuccess)
            {
                ModelState.AddModelError(
                    string.Empty,
                    result.Error.Message);

                return View(model);
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

            var authenticationProperties = new AuthenticationProperties {
                IsPersistent = model.RememberMe
            };

            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal, authenticationProperties);

            return RedirectToAction("Index", "Home");
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
