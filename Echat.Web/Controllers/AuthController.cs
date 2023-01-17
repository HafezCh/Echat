using CoreLayer.Services.Users;
using CoreLayer.ViewModels.Auth;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Echat.Web.Controllers
{
    public class AuthController : Controller
    {
        private readonly IUserService _userService;

        public AuthController(IUserService userService)
        {
            _userService = userService;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return RedirectToAction("Index");
            }
            var result = await _userService.RegisterUser(model);
            if (!result)
            {
                ModelState.AddModelError(model.UserName, "نام کاربری تکراری است");
                return RedirectToAction("Index");
            }
            return Redirect("/auth#login");
        }

        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return RedirectToAction("Index");
            }

            var user = await _userService.LoginUser(model);

            if (user == null)
            {
                ModelState.AddModelError("UserName", "کاربری با مشخصات وارد شده یافت نشد");
                return RedirectToAction("Index");
            }

            var claims = new List<Claim>
            {
                new(ClaimTypes.NameIdentifier,user.Id.ToString()),
                new(ClaimTypes.Name,user.UserName)
            };

            var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var principal = new ClaimsPrincipal(identity);
            var properties = new AuthenticationProperties
            {
                IsPersistent = true
            };

            await HttpContext.SignInAsync(principal, properties);

            return Redirect("/");
        }
    }
}
