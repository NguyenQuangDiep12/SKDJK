using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SKDJK.Services.Interfaces;
using SKDJK.ViewModels;
using System.Security.Claims;
using SKDJK.Models.Commons;

namespace SKDJK.Controllers
{
    public class AuthController : Controller
    {
        private readonly IAuthService _authService;
        public AuthController(IAuthService authService)
        {
            this._authService = authService;
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

            var result = await _authService.RegisterAsync(model.FullName, model.Email, model.Password);

            if (!result.IsSuccess)
            {
                if(result.Error == Error.EmailAlreadyExist)
                {
                    ModelState.AddModelError(
                        nameof(model.Email), result.Error.Message);
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

            var result = await _authService.LoginAsync(model.Email, model.Password);

            if (!result.IsSuccess)
            {
                ModelState.AddModelError(string.Empty, result.Error.Message);

                return View(model);
            }

            var user = result.Value; // tra ve du lieu la authenticatedUserDto


            var claims = new List<Claim>
            {
                new Claim
                (
                    ClaimTypes.NameIdentifier,
                    user.UserId.ToString()
                ),
                new Claim(
                    ClaimTypes.Name,
                    user.FullName
                ),
                new Claim(
                    ClaimTypes.Email,
                    user.Email
                ),
                new Claim(
                    ClaimTypes.Role,
                    user.RoleName
                )
            };

            var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

            var principal = new ClaimsPrincipal(identity);


            var authenticationProperty = new AuthenticationProperties
            {
                IsPersistent = true,
            };

            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal, authenticationProperty);

            return RedirectToAction("Index", "Home");
        }

        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(
                CookieAuthenticationDefaults.AuthenticationScheme);

            // xoa sach session data tam
            HttpContext.Session.Clear();

            return RedirectToAction("Index", "Home");
        }

        // Forbidden
        [HttpGet]
        public IActionResult Forbidden()
        {
            return View();
        }

        // NotFound
        [HttpGet]
        public IActionResult Notfound()
        {
            return View();
        }
    }
}
