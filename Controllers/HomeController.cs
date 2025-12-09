using Microsoft.AspNetCore.Mvc;
using ValidationDemo.Services;
using ValidationDemo.Filters;

namespace ValidationDemo.Controllers
{
    [Route("")]
    public class HomeController : Controller
    {
        private readonly IUserService _userService;

        public HomeController(IUserService userService)
        {
            _userService = userService;
        }

        // GET: /
        [HttpGet("")]
        public IActionResult Index()
        {
            return View();
        }

        // GET: /dashboard (Protected - requires login)
        [HttpGet("dashboard")]
        [CustomAuthorize] // Our custom authorization filter
        [ServiceFilter(typeof(PageVisitTimeFilter))] // Cache visit time
        public async Task<IActionResult> Dashboard()
        {
            var userId = int.Parse(User.FindFirst("UserId").Value);
            var user = await _userService.GetUserByIdAsync(userId);

            ViewBag.Username = user.Username;
            ViewBag.Email = user.Email;

            return View();
        }

        // GET: /profile (Protected)
        [HttpGet("profile")]
        [CustomAuthorize]
        [ServiceFilter(typeof(PageVisitTimeFilter))]
        public async Task<IActionResult> Profile()
        {
            var userId = int.Parse(User.FindFirst("UserId").Value);
            var user = await _userService.GetUserByIdAsync(userId);

            if (user == null || !user.IsActive)
            {
                return RedirectToAction("Logout", "Account");
            }

            return View(user);
        }
    }
}