using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Caching.Memory;

namespace ValidationDemo.Filters
{
    public class PageVisitTimeFilter : IResultFilter
    {
        private readonly IMemoryCache _cache;
        private readonly ILogger<PageVisitTimeFilter> _logger;

        public PageVisitTimeFilter(IMemoryCache cache, ILogger<PageVisitTimeFilter> logger)
        {
            _cache = cache;
            _logger = logger;
        }

        public void OnResultExecuting(ResultExecutingContext context)
        {
            // Generate cache key based on action and user
            var userName = context.HttpContext.User.Identity?.Name ?? "Anonymous";
            var actionName = context.ActionDescriptor.DisplayName;
            var cacheKey = $"PageVisit_{userName}_{actionName}";

            DateTime visitTime;

            // Try to get from cache
            if (_cache.TryGetValue(cacheKey, out DateTime cachedTime))
            {
                visitTime = cachedTime;
                _logger.LogInformation($"Using cached visit time for {userName}: {visitTime}");
            }
            else
            {
                // First visit or cache expired - store new time
                visitTime = DateTime.Now;

                var cacheOptions = new MemoryCacheEntryOptions()
                    .SetAbsoluteExpiration(TimeSpan.FromMinutes(2)); // Cache for 2 minutes

                _cache.Set(cacheKey, visitTime, cacheOptions);
                _logger.LogInformation($"Cached new visit time for {userName}: {visitTime}");
            }

            // Add to ViewBag so it's available in the view
            if (context.Controller is Controller controller)
            {
                controller.ViewBag.PageVisitTime = visitTime;
                controller.ViewBag.IsCachedTime = _cache.TryGetValue(cacheKey, out _);
            }
        }

        public void OnResultExecuted(ResultExecutedContext context)
        {
            // Optional: Log after result is executed
            _logger.LogInformation($"Result executed for {context.ActionDescriptor.DisplayName}");
        }
    }
}