using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace ValidationDemo.Filters
{
    public class CustomAuthorizeFilter : IAuthorizationFilter
    {
        public void OnAuthorization(AuthorizationFilterContext context)
        {
            // Check if user is authenticated
            if (!context.HttpContext.User.Identity?.IsAuthenticated ?? true)
            {
                // Store the return URL
                var returnUrl = context.HttpContext.Request.Path + context.HttpContext.Request.QueryString;

                context.Result = new RedirectToActionResult(
                    "Login",
                    "Account",
                    new { returnUrl = returnUrl });
            }
        }
    }

    // Attribute version for easier use
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public class CustomAuthorizeAttribute : TypeFilterAttribute
    {
        public CustomAuthorizeAttribute() : base(typeof(CustomAuthorizeFilter))
        {
        }
    }
}