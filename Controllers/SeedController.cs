// Controllers/SeedController.cs (DELETE THIS AFTER RUNNING ONCE)
using Microsoft.AspNetCore.Mvc;
using ValidationDemo.Services;

namespace ValidationDemo.Controllers
{
    [Route("seed")]
    public class SeedController : Controller
    {
        private readonly IUserService _userService;

        public SeedController(IUserService userService)
        {
            _userService = userService;
        }

        // Navigate to: https://localhost:XXXX/seed/admin
        [HttpGet("admin")]
        public async Task<IActionResult> SeedAdmin()
        {
            var result = await _userService.CreateAdminUserAsync(
                username: "admin",
                email: "admin@example.com",
                password: "Admin@123"
            );

            if (result.Success)
            {
                return Content($"✅ {result.Message}<br/><br/>Username: admin<br/>Password: Admin@123<br/><br/><a href='/account/login'>Login Now</a>", "text/html");
            }

            return Content($"❌ {result.Message}", "text/html");
        }
    }
}