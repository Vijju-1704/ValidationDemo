using Microsoft.AspNetCore.Mvc;
using ValidationDemo.Models;
using ValidationDemo.Services;
using System.Linq;

namespace ValidationDemo.Controllers
{
    [Route("api/user")]
    [ApiController]
    public class UserApiController : ControllerBase
    {
        private readonly IUserService UserService;

        public UserApiController(IUserService userService)
        {
            UserService = userService;
        }

        /// <summary>
        /// API endpoint for user registration (JSON)
        /// POST: /api/user/register
        /// </summary>
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] UserRegistrationModel model)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values
                    .SelectMany(v => v.Errors)
                    .Select(e => e.ErrorMessage)
                    .ToList();

                return BadRequest(new
                {
                    success = false,
                    message = "Validation failed",
                    errors = errors
                });
            }

            var result = await UserService.RegisterUserAsync(model);

            if (!result.Success)
            {
                return BadRequest(new
                {
                    success = false,
                    message = result.Message
                });
            }

            return Ok(new
            {
                success = true,
                message = result.Message,
                data = new
                {
                    id = result.User.Id,
                    username = result.User.Username,
                    email = result.User.Email
                }
            });
        }

        /// <summary>
        /// API endpoint to get user details by ID
        /// GET: /api/user/{id}
        /// </summary>
        [HttpGet("{id}")]
        public async Task<IActionResult> GetUser([FromRoute] int id)
        {
            var user = await UserService.GetUserByIdAsync(id);

            if (user == null || !user.IsActive)
            {
                return NotFound(new
                {
                    success = false,
                    message = "User not found"
                });
            }

            return Ok(new
            {
                success = true,
                data = new
                {
                    id = user.Id,
                    username = user.Username,
                    email = user.Email,
                    age = user.Age,
                    phoneNumber = user.PhoneNumber,
                    website = user.Website,
                    gender = user.Gender,
                    country = user.Country,
                    dateOfBirth = user.DateOfBirth
                }
            });
        }

        /// <summary>
        /// API endpoint to get all active users
        /// GET: /api/user/list
        /// </summary>
        [HttpGet("list")]
        public async Task<IActionResult> GetAllUsers()
        {
            var users = await UserService.GetAllActiveUsersAsync();

            return Ok(new
            {
                success = true,
                data = users.Select(u => new
                {
                    id = u.Id,
                    username = u.Username,
                    email = u.Email,
                    age = u.Age,
                    country = u.Country
                })
            });
        }
    }
}