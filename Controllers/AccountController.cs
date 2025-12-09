using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using System.Security.Claims;
using ValidationDemo.Models;
using ValidationDemo.Services;

namespace ValidationDemo.Controllers
{
    [Route("account")]
    public class AccountController : Controller
    {
        private readonly IUserService _userService;
        private readonly ILogger<AccountController> _logger;

        public AccountController(IUserService userService, ILogger<AccountController> logger)
        {
            _userService = userService;
            _logger = logger;
        }

        // GET: /account/login
        [HttpGet("login")]
        public IActionResult Login(string returnUrl = null)
        {
            ViewBag.ReturnUrl = returnUrl;
            return View();
        }

        // POST: /account/login
        [HttpPost("login")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginModel model, string returnUrl = null)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            // Validate user credentials
            var user = await _userService.ValidateUserAsync(model.Username, model.Password);

            if (user == null)
            {
                ModelState.AddModelError(string.Empty, "Invalid username or password");
                return View(model);
            }

            if (!user.IsActive)
            {
                ModelState.AddModelError(string.Empty, "Your account has been deactivated");
                return View(model);
            }

            // Create claims
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Name, user.Username),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim("UserId", user.Id.ToString())
            };

            var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var claimsPrincipal = new ClaimsPrincipal(claimsIdentity);

            var authProperties = new AuthenticationProperties
            {
                IsPersistent = model.RememberMe,
                ExpiresUtc = model.RememberMe
                    ? DateTimeOffset.UtcNow.AddDays(7)
                    : DateTimeOffset.UtcNow.AddHours(1)
            };

            // Sign in the user
            await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                claimsPrincipal,
                authProperties);

            _logger.LogInformation($"User {user.Username} logged in successfully");

            TempData["SuccessMessage"] = $"Welcome back, {user.Username}!";

            // Redirect to return URL or dashboard
            if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }

            return RedirectToAction("Dashboard", "Home");
        }

        // GET: /account/logout
        [HttpGet("logout")]
        public async Task<IActionResult> Logout()
        {
            var userName = User.Identity?.Name;

            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);

            _logger.LogInformation($"User {userName} logged out");

            TempData["SuccessMessage"] = "You have been logged out successfully";

            return RedirectToAction("Login");
        }

        // GET: /account/accessdenied
        [HttpGet("accessdenied")]
        public IActionResult AccessDenied()
        {
            return View();
        }
    }
}