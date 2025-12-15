using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using ValidationDemo.Constants;
using ValidationDemo.Models;
using ValidationDemo.Services;

namespace ValidationDemo.Controllers
{
    [Route("account")]
    public class AccountController : Controller
    {
        private readonly IUserService UserService;
        private readonly ILogger<AccountController> Logger;

        public AccountController(IUserService userService, ILogger<AccountController> logger)
        {
            UserService = userService;
            Logger = logger;
        }

        // GET: /account/login
        [HttpGet("login")]
        public IActionResult Login(string? returnUrl = null)
        {
            ViewBag.ReturnUrl = returnUrl;
            return View();
        }

        // POST: /account/login
        [HttpPost("login")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginModel model, string? returnUrl = null)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            ViewData["ReturnUrl"] = returnUrl;
            // Ensure username and password are not null before passing to ValidateUserAsync
            if (string.IsNullOrWhiteSpace(model.Username) || string.IsNullOrWhiteSpace(model.Password))
            {
                ModelState.AddModelError(string.Empty, "Username and password are required.");
                return View(model);
            }

            // Validate user credentials
            var user = await UserService.ValidateUserAsync(model.Username, model.Password);

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
            HttpContext.Session.SetString("UserName", user.Username!);
            HttpContext.Session.SetString("UserEmail", user.Email!);

            // Create claims
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Name, user.Username!),
                new Claim(ClaimTypes.Email, user.Email!),
                new Claim(AppClaims.UserId, user.Id.ToString()),
                new Claim(ClaimTypes.Role, user.Role),
                new Claim(AppClaims.Role, user.Role)
             };
            if (user.Role == AppRoles.Admin)
            {
                claims.Add(new Claim(AppClaims.CanEditUsers, "true"));
                claims.Add(new Claim(AppClaims.CanDeleteUsers, "true"));
                claims.Add(new Claim(AppClaims.CanViewUsers, "true"));
                claims.Add(new Claim(AppClaims.CanViewDeletedUsers, "true"));
                claims.Add(new Claim(AppClaims.CanRestoreUsers, "true"));
            }
            else
            {
                // Regular users can only view their own profile
                claims.Add(new Claim(AppClaims.CanViewUsers, "false"));
            }


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

            // Redirect based on role
            if (user.Role == AppRoles.Admin)
            {
                return RedirectToAction("List", "UserManagement");
            }
            else
            {
                return RedirectToAction("Dashboard", "Home");
            }

            //// Redirect to returnUrl or user profile
            //if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
            //{
            //    return Redirect(returnUrl);
            //}


            //Logger.LogInformation($"User {user.Username} logged in successfully");

            //TempData["SuccessMessage"] = $"Welcome back, {user.Username}!";

            //return View(model);
        }

        //// GET: /account/logout
        //[HttpGet("logout")]
        //public async Task<IActionResult> Logout()
        //{
        //    var userName = User.Identity?.Name;

        //    await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);

        //    Logger.LogInformation($"User {userName} logged out");

        //    TempData["SuccessMessage"] = "You have been logged out successfully";

        //    return RedirectToAction("Login");
        //}
        // POST: /account/logout
        [HttpPost("logout")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            HttpContext.Session.Clear();
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
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