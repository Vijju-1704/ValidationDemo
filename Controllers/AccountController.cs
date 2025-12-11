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
        public IActionResult Login(string? returnUrl = null)
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

            // Get user to check lockout status
            var userForLockoutCheck = await _userService.GetUserByUsernameAsync(model.Username);

            // CHECK IF ACCOUNT IS LOCKED OUT
            if (userForLockoutCheck != null && userForLockoutCheck.IsLockedOut)
            {
                var remainingMinutes = (int)(userForLockoutCheck.LockoutEnd.Value - DateTime.UtcNow).TotalMinutes + 1;
                ModelState.AddModelError(string.Empty,
                    $"Account is locked due to multiple failed login attempts. Please try again in {remainingMinutes} minutes.");

                _logger.LogWarning($"Locked account login attempt: {model.Username}");
                return View(model);
            }

            // VALIDATE CREDENTIALS (with hashed password check)
            var user = await _userService.ValidateUserAsync(model.Username, model.Password);

            if (user == null)
            {
                // FAILED LOGIN - INCREMENT ATTEMPTS
                if (userForLockoutCheck != null)
                {
                    await _userService.IncrementFailedLoginAttemptsAsync(model.Username);

                    var attempts = userForLockoutCheck.FailedLoginAttempts + 1;

                    if (attempts >= 5)
                    {
                        ModelState.AddModelError(string.Empty,
                            "Account has been locked for 15 minutes due to multiple failed login attempts.");
                        _logger.LogWarning($"Account locked: {model.Username}");
                    }
                    else
                    {
                        var remainingAttempts = 5 - attempts;
                        ModelState.AddModelError(string.Empty,
                            $"Invalid username or password. {remainingAttempts} attempts remaining before lockout.");
                    }
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Invalid username or password");
                }

                return View(model);
            }

            // SUCCESSFUL LOGIN

            // Reset failed login attempts
            await _userService.ResetFailedLoginAttemptsAsync(user.Id);

            // Update last login info
            var ipAddress = HttpContext.Connection.RemoteIpAddress?.ToString() ?? "Unknown";
            await _userService.UpdateLastLoginAsync(user.Id, ipAddress);

            // ============================================
            // CREATE RICH CLAIMS
            // ============================================
            var claims = new List<Claim>
            {
                // Standard Claims
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Name, user.Username),
                new Claim(ClaimTypes.Email, user.Email),
                
                // Custom Claims
                new Claim("UserId", user.Id.ToString()),
                new Claim("DateOfBirth", user.DateOfBirth.ToString("yyyy-MM-dd")),
                new Claim("Age", user.Age.ToString()),
                new Claim("Country", user.Country.ToString()),
                
                // ROLE CLAIM (Important for authorization!)
                new Claim(ClaimTypes.Role, user.Role), // "Admin", "Manager", or "User"
                
                // Department Claim (if exists)
                new Claim("Department", user.Department ?? ""),
            };

            // ADD PERMISSION CLAIMS (if user has any)
            if (!string.IsNullOrEmpty(user.Permissions))
            {
                var permissions = user.Permissions.Split(',', StringSplitOptions.RemoveEmptyEntries);
                foreach (var permission in permissions)
                {
                    claims.Add(new Claim("Permission", permission.Trim()));
                }
            }

            // Create Claims Identity
            var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var claimsPrincipal = new ClaimsPrincipal(claimsIdentity);

            // Create Authentication Properties
            var authProperties = new AuthenticationProperties
            {
                IsPersistent = model.RememberMe,
                ExpiresUtc = model.RememberMe
                    ? DateTimeOffset.UtcNow.AddDays(7)
                    : DateTimeOffset.UtcNow.AddHours(1),
                AllowRefresh = true,
                IssuedUtc = DateTimeOffset.UtcNow
            };

            // Sign In
            await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                claimsPrincipal,
                authProperties);

            _logger.LogInformation($"User logged in successfully: {user.Username} (Role: {user.Role})");

            TempData["SuccessMessage"] = $"Welcome back, {user.Username}!";

            // Redirect to return URL or Dashboard
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

            _logger.LogInformation($"User logged out: {userName}");

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