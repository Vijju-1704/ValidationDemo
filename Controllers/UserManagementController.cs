using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ValidationDemo.Constants;
using ValidationDemo.Services;

namespace ValidationDemo.Controllers
{
    [Route("user/management")]
    [Authorize(Policy = AppPolicies.AdminOnly)] // ALL actions require Admin role
    public class UserManagementController : Controller
    {
        private readonly IUserService UserService;

        public UserManagementController(IUserService userService)
        {
            UserService = userService;
        }

        // GET: /user/management/list
        // Only Admin can access
        [HttpGet("list")]
        public async Task<IActionResult> List()
        {
            var users = await UserService.GetAllActiveUsersAsync();
            return View(users);
        }

        // GET: /user/management/delete/{id}
        // Only Admin can delete
        [HttpGet("delete/{id}")]
        public async Task<IActionResult> Delete([FromRoute] int id)
        {
            var user = await UserService.GetUserByIdAsync(id);
            if (user == null || !user.IsActive)
            {
                return NotFound();
            }

            return View(user);
        }

        // POST: /user/management/delete/{id}
        // Only Admin can delete
        [HttpPost("delete/{id}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed([FromRoute] int id)
        {
            var result = await UserService.DeleteUserAsync(id);

            if (!result.Success)
            {
                TempData["ErrorMessage"] = result.Message;
                return RedirectToAction("List");
            }

            TempData["SuccessMessage"] = result.Message;
            return RedirectToAction("List");
        }

        // GET: /user/management/deleted
        // Only Admin can view deleted users
        [HttpGet("deleted")]
        [Authorize(Policy = AppPolicies.CanViewDeletedUsers)]
        public async Task<IActionResult> DeletedUsers()
        {
            var deletedUsers = await UserService.GetAllDeletedUsersAsync();
            return View(deletedUsers);
        }

        // POST: /user/management/restore/{id}
        // Only Admin can restore
        [HttpPost("restore/{id}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Restore([FromRoute] int id)
        {
            var result = await UserService.RestoreUserAsync(id);

            if (!result.Success)
            {
                TempData["ErrorMessage"] = result.Message;
            }
            else
            {
                TempData["SuccessMessage"] = result.Message;
            }

            return RedirectToAction("DeletedUsers");
        }
    }
}