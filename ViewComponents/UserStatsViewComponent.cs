using Microsoft.AspNetCore.Mvc;
using ValidationDemo.Services;

namespace ValidationDemo.ViewComponents
{
    /// <summary>
    /// Simple View Component that shows user statistics
    /// </summary>
    public class UserStatsViewComponent : ViewComponent
    {
        private readonly IUserService UserService;

        // Dependency Injection - just like in controllers
        public UserStatsViewComponent(IUserService userService)
        {
            UserService = userService;
        }

        // Main method - InvokeAsync (must have this name!)
        public async Task<IViewComponentResult> InvokeAsync()
        {
            // Get statistics from service
            var totalUsers = await UserService.GetTotalUsersCountAsync();
            var activeUsers = await UserService.GetActiveUsersCountAsync();
            var deletedUsers = await UserService.GetDeletedUsersCountAsync();

            // Create a simple model
            var stats = new UserStatisticsViewModel
            {
                TotalUsers = totalUsers,
                ActiveUsers = activeUsers,
                DeletedUsers = deletedUsers
            };

            // Return the view with data
            return View(stats);
        }
    }

    // Simple model for the view component
    public class UserStatisticsViewModel
    {
        public int TotalUsers { get; set; }
        public int ActiveUsers { get; set; }
        public int DeletedUsers { get; set; }
    }
}