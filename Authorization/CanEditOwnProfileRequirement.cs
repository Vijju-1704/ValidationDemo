// Authorization/CanEditOwnProfileRequirement.cs
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace ValidationDemo.Authorization
{
    // Requirement: User can only edit their own profile OR is Admin
    public class CanEditOwnProfileRequirement : IAuthorizationRequirement
    {
        public int UserId { get; }

        public CanEditOwnProfileRequirement(int userId)
        {
            UserId = userId;
        }
    }

    public class CanEditOwnProfileHandler
        : AuthorizationHandler<CanEditOwnProfileRequirement>
    {
        protected override Task HandleRequirementAsync(
            AuthorizationHandlerContext context,
            CanEditOwnProfileRequirement requirement)
        {
            // Check if user is Admin - they can edit anyone
            var roleClaim = context.User.FindFirst(ClaimTypes.Role);
            if (roleClaim?.Value == "Admin")
            {
                context.Succeed(requirement);
                return Task.CompletedTask;
            }

            // Check if user is editing their own profile
            var userIdClaim = context.User.FindFirst("UserId");
            if (userIdClaim != null && int.TryParse(userIdClaim.Value, out int userId))
            {
                if (userId == requirement.UserId)
                {
                    context.Succeed(requirement);
                }
            }

            return Task.CompletedTask;
        }
    }
}