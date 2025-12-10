using Microsoft.AspNetCore.Mvc.Filters;

namespace ValidationDemo.Filters
{
    // Custom Action Filter
    public class LogActionFilter : IActionFilter
    {
        private readonly ILogger<LogActionFilter> Logger;

        public LogActionFilter(ILogger<LogActionFilter> logger)
        {
            Logger = logger;
        }

        // Runs BEFORE the action executes
        public void OnActionExecuting(ActionExecutingContext context)
        {
            Logger.LogInformation($"Executing action: {context.ActionDescriptor.DisplayName}");
            Logger.LogInformation($"Arguments: {string.Join(", ", context.ActionArguments)}");
        }

        // Runs AFTER the action executes
        public void OnActionExecuted(ActionExecutedContext context)
        {
            Logger.LogInformation($"Action executed: {context.ActionDescriptor.DisplayName}");

            if (context.Exception != null)
            {
                Logger.LogError($"Exception occurred: {context.Exception.Message}");
            }
        }
    }
}
