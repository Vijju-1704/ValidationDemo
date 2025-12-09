using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using ValidationDemo.Models;

namespace ValidationDemo.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
//using Microsoft.AspNetCore.Mvc;
//using System.Reflection;
//using ValidationDemo.Models;

//namespace ValidationDemo.Controllers
//{
//    [Route("user")]
//    public class UserController : Controller
//    {
//        [HttpGet("register")]
//        public IActionResult Register([FromQuery] string referral = null)
//        {
//            if (!string.IsNullOrEmpty(referral))
//            {
//                ViewBag.Referral = $"Referred by : {referral}";
//            }
//            return View(new UserRegistrationModel());
//        }

//        [HttpPost("register")]
//        [ValidateAntiForgeryToken]
//        public IActionResult Register([FromForm] UserRegistrationModel model)
//        {
//            if (ModelState.IsValid)
//            {
//                TempData["SuccessMessage"] = $"Registration successful for {model.Username}!";
//                return RedirectToAction("Success", new { id = model.Username });
//            }
//            return View(model);
//        }


//        // GET: /user/success/johndoe
//        // FromRoute example - the {id} parameter comes from the route
//        [HttpGet("success/{id}")]
//        public IActionResult Success([FromRoute] string id)
//        {
//            ViewBag.Username = id;
//            return View();
//        }

//        [HttpPost("api/register")]
//        public IActionResult ApiRegister([FromBody] UserRegistrationModel model)
//        {
//            if (ModelState.IsValid)
//            {
//                return Ok(new { success = true, message = "User registered successfully", username = model.Username });
//            }

//            var errors = ModelState.Values
//                .SelectMany(v => v.Errors)
//                .Select(e => e.ErrorMessage);

//            return BadRequest(new { success = false, errors });
//        }

//        [HttpGet("details/{userId}")]
//        public IActionResult GetUserDetails(
//            [FromRoute] int userId,
//            [FromQuery] string format = "json",
//            [FromHeader(Name = "User-Agent")] string userAgent = null)
//        {
//            return Ok(new
//            {
//                userId,
//                format,
//                userAgent,
//                message = "This demonstrates FromRoute, FromQuery, and FromHeader"
//            });
//        }
//    }
//}

