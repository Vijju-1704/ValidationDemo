using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Caching.Memory;

namespace ValidationDemo.Filters
{
    public class GlobalTimeFilter : IResultFilter
    {
        private readonly IMemoryCache _cache;

        public GlobalTimeFilter(IMemoryCache cache)
        {
            _cache = cache;
        }

        public void OnResultExecuting(ResultExecutingContext context)
        {
            // Only for views, not for JSON/API
            if (context.Result is not ViewResult)
                return;

            // Try to get cached time
            if (!_cache.TryGetValue("GlobalTime", out DateTime cachedTime))
            {
                // Not in cache, store new time for 20 seconds
                cachedTime = DateTime.Now;
                _cache.Set("GlobalTime", cachedTime, TimeSpan.FromSeconds(20));
            }

            // Add to ViewBag so all views can access it
            if (context.Controller is Controller controller)
            {
                controller.ViewBag.CurrentTime = cachedTime;
            }
        }

        public void OnResultExecuted(ResultExecutedContext context)
        {
            // Nothing to do after result
        }
    }
}