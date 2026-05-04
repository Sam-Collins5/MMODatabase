using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MMOngo.Models;
using MMOngo.Services.Interfaces;
using MMOngo.ViewModels.Auth;

namespace MMOngo.Controllers
{
    public class UserLoginController : Controller
    {
        private readonly IUserService _userService;

        public UserLoginController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult Login()
        {
            if (User.Identity != null && User.Identity.IsAuthenticated)
            {
                // Temporary testing response:
                // return Content("You are logged in as " + User.Identity.Name);

                // Normal redirect:
                return RedirectToAction("Index", "Home");
            }

            return View(new LoginInputModel());
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginInputModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            User? user = await _userService.ValidateLoginAsync(model.UsernameOrEmail, model.Password);

            if (user == null)
            {
                ModelState.AddModelError(string.Empty, "Invalid username/email or password.");
                return View(model);
            }

            List<Claim> claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id ?? string.Empty),
                new Claim(ClaimTypes.Name, user.Username),
                new Claim(ClaimTypes.Email, user.Email)
            };

            ClaimsIdentity identity = new ClaimsIdentity(
                claims,
                CookieAuthenticationDefaults.AuthenticationScheme
            );

            AuthenticationProperties authProperties = new AuthenticationProperties
            {
                IsPersistent = model.RememberMe
            };

            await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                new ClaimsPrincipal(identity),
                authProperties
            );

            // Temporary testing redirect:
            // return RedirectToAction("Login", "UserLogin");

            // Normal redirect:
            return RedirectToAction("Index", "Home");
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult Register()
        {
            if (User.Identity != null && User.Identity.IsAuthenticated)
            {
                // Temporary testing response:
                // return Content("You are logged in as " + User.Identity.Name);

                // Normal redirect:
                return RedirectToAction("Index", "Home");
            }

            return View(new RegisterInputModel());
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterInputModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            User user = new User
            {
                Username = model.Username.Trim(),
                Email = model.Email.Trim()
            };

            User? createdUser = await _userService.RegisterUserAsync(user, model.Password);

            if (createdUser == null)
            {
                ModelState.AddModelError(string.Empty, "That username or email is already being used.");
                return View(model);
            }

            List<Claim> claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, createdUser.Id ?? string.Empty),
                new Claim(ClaimTypes.Name, createdUser.Username),
                new Claim(ClaimTypes.Email, createdUser.Email)
            };

            ClaimsIdentity identity = new ClaimsIdentity(
                claims,
                CookieAuthenticationDefaults.AuthenticationScheme
            );

            await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                new ClaimsPrincipal(identity)
            );

            // Temporary testing redirect:
            // return RedirectToAction("Login", "UserLogin");

            // Normal redirect:
            return RedirectToAction("Index", "Home");
        }

        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);

            // Temporary testing redirect:
            // return RedirectToAction("Login", "UserLogin");

            // Normal redirect:
            return RedirectToAction("Index", "Home");
        }
    }
}