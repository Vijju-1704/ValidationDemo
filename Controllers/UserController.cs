using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using ValidationDemo.Constants;
using ValidationDemo.Models;
using ValidationDemo.Services;

namespace ValidationDemo.Controllers
{
    [Route("user")]
    public class UserController : Controller
    {
        private readonly IUserService UserService;
        private readonly IAuthorizationService _authorizationService;

        public UserController(
            IUserService userService,
            IAuthorizationService authorizationService)
        {
            UserService = userService;
            _authorizationService = authorizationService;
        }

        // GET: /user/register
        [HttpGet("register")]
        [AllowAnonymous] // Anyone can register
        public IActionResult Register([FromQuery] string? referral = null)
        {
            if (!string.IsNullOrEmpty(referral))
            {
                ViewBag.Referral = $"Referred by: {referral}";
            }

            return View(new UserRegistrationModel());
        }

        // POST: /user/register
        [HttpPost("register")]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register([FromForm] UserRegistrationModel model)
        {
            if (ModelState.IsValid)
            {
                var result = await UserService.RegisterUserAsync(model);

                if (!result.Success)
                {
                    ModelState.AddModelError(string.Empty, result.Message);
                    return View(model);
                }

                TempData["SuccessMessage"] = result.Message;
                return RedirectToAction("Success", new { id = result.User.Id });
            }

            return View(model);
        }

        // GET: /user/success/{id}
        [HttpGet("success/{id}")]
        [AllowAnonymous]
        public async Task<IActionResult> Success([FromRoute] int id)
        {
            var user = await UserService.GetUserByIdAsync(id);

            if (user == null)
            {
                return NotFound();
            }

            ViewBag.Username = user.Username;
            ViewBag.Email = user.Email;
            return View();
        }

        // GET: /user/details/{id}
        [HttpGet("details/{id}")]
        [Authorize] // Must be logged in
        public async Task<IActionResult> Details([FromRoute] int id)
        {
            var user = await UserService.GetUserByIdAsync(id);

            if (user == null || !user.IsActive)
            {
                return NotFound();
            }

            // Get current logged-in user ID
            var currentUserIdClaim = User.FindFirst(AppClaims.UserId);
            var currentUserId = currentUserIdClaim != null ? int.Parse(currentUserIdClaim.Value) : 0;

            // Check if user is viewing their own profile OR is Admin
            var isAdmin = User.IsInRole(AppRoles.Admin);
            var isOwnProfile = currentUserId == id;

            if (!isAdmin && !isOwnProfile)
            {
                return RedirectToAction("AccessDenied", "Account");
            }

            return View(user);
        }

        // GET: /user/edit/{id}
        [HttpGet("edit/{id}")]
        [Authorize] // Must be logged in
        public async Task<IActionResult> Edit([FromRoute] int id)
        {
            var user = await UserService.GetUserByIdAsync(id);

            if (user == null || !user.IsActive)
            {
                return NotFound();
            }

            // Get current logged-in user ID
            var currentUserIdClaim = User.FindFirst(AppClaims.UserId);
            var currentUserId = currentUserIdClaim != null ? int.Parse(currentUserIdClaim.Value) : 0;

            // Check authorization: Can only edit own profile OR is Admin
            var isAdmin = User.IsInRole(AppRoles.Admin);
            var isOwnProfile = currentUserId == id;

            if (!isAdmin && !isOwnProfile)
            {
                return RedirectToAction("AccessDenied", "Account");
            }

            // Map UserEntity to EditUserModel
            var editModel = new EditUserModel
            {
                Id = user.Id,
                Username = user.Username,
                Email = user.Email,
                Age = user.Age,
                PhoneNumber = user.PhoneNumber,
                Website = user.Website,
                Gender = user.Gender,
                Country = user.Country,
                DateOfBirth = user.DateOfBirth,
                SubscribeNewsletter = user.SubscribeNewsletter
            };

            return View(editModel);
        }

        // POST: /user/edit/{id}
        [HttpPost("edit/{id}")]
        [Authorize]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit([FromRoute] int id, [FromForm] EditUserModel model)
        {
            if (id != model.Id)
            {
                return BadRequest();
            }

            // Get current logged-in user ID
            var currentUserIdClaim = User.FindFirst(AppClaims.UserId);
            var currentUserId = currentUserIdClaim != null ? int.Parse(currentUserIdClaim.Value) : 0;

            // Check authorization
            var isAdmin = User.IsInRole(AppRoles.Admin);
            var isOwnProfile = currentUserId == id;

            if (!isAdmin && !isOwnProfile)
            {
                return RedirectToAction("AccessDenied", "Account");
            }

            if (ModelState.IsValid)
            {
                var result = await UserService.UpdateUserAsync(model);

                if (!result.Success)
                {
                    ModelState.AddModelError(string.Empty, result.Message);
                    return View(model);
                }

                TempData["SuccessMessage"] = result.Message;
                return RedirectToAction("Details", new { id = result.User.Id });
            }

            return View(model);
        }
    }
}