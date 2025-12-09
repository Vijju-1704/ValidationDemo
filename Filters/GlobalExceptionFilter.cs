using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ViewFeatures;

namespace ValidationDemo.Filters
{
    public class GlobalExceptionFilter : IExceptionFilter
    {
        private readonly ILogger<GlobalExceptionFilter> _logger;
        private readonly IWebHostEnvironment _env;

        public GlobalExceptionFilter(ILogger<GlobalExceptionFilter> logger, IWebHostEnvironment env)
        {
            _logger = logger;
            _env = env;
        }

        public void OnException(ExceptionContext context)
        {
            // Log the exception
            _logger.LogError(context.Exception,
                $"An error occurred in {context.ActionDescriptor.DisplayName}");

            // Check if it's an API request
            var isApi = context.HttpContext.Request.Path.StartsWithSegments("/api");

            if (isApi)
            {
                // API error response
                var errorResponse = new
                {
                    success = false,
                    message = _env.IsDevelopment()
                        ? context.Exception.Message
                        : "An error occurred while processing your request.",
                    stackTrace = _env.IsDevelopment() ? context.Exception.StackTrace : null
                };

                context.Result = new JsonResult(errorResponse)
                {
                    StatusCode = 500
                };
            }
            else
            {
                // MVC error page
                var errorMessage = _env.IsDevelopment()
                    ? context.Exception.Message
                    : "An unexpected error occurred. Please try again later.";

                context.Result = new ViewResult
                {
                    ViewName = "Error",
                    ViewData = new ViewDataDictionary(
                        new EmptyModelMetadataProvider(),
                        new ModelStateDictionary())
                    {
                        ["ErrorMessage"] = errorMessage,
                        ["StackTrace"] = _env.IsDevelopment() ? context.Exception.StackTrace : null
                    }
                };
            }

            context.ExceptionHandled = true;
        }
    }
}