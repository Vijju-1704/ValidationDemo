using Microsoft.AspNetCore.Mvc;
using ValidationDemo.Models;
using ValidationDemo.Services;

namespace ValidationDemo.Controllers
{
    [Route("user")]
    public class UserController : Controller
    {
        private readonly IUserService UserService;

        public UserController(IUserService userService)
        {
            UserService = userService;
        }

        // GET: /user/register
        [HttpGet("register")]
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

                //HttpContext.Session.SetString("Registered", "true");

                TempData["SuccessMessage"] = result.Message;
                return RedirectToAction("Success", new { id = result.User.Id });
            }

            return View(model);
        }

        // GET: /user/success/{id}
        [HttpGet("success/{id}")]
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
        public async Task<IActionResult> Details([FromRoute] int id)
        {
            var user = await UserService.GetUserByIdAsync(id);

            if (user == null || !user.IsActive)
            {
                return NotFound();
            }

            return View(user);
        }

        // GET: /user/edit/{id}
        [HttpGet("edit/{id}")]
        public async Task<IActionResult> Edit([FromRoute] int id)
        {
            var user = await UserService.GetUserByIdAsync(id);

            if (user == null || !user.IsActive)
            {
                return NotFound();
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
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit([FromRoute] int id, [FromForm] EditUserModel model)
        {
            if (id != model.Id)
            {
                return BadRequest();
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