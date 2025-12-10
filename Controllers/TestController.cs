using Microsoft.AspNetCore.Mvc;

namespace ValidationDemo.Controllers
{
    public class TestController : Controller
    {
        private readonly ILogger<TestController> _logger;

        public TestController(ILogger<TestController> logger)
        {
            _logger = logger;
        }

        // Test 1: Throw a simple exception
        public IActionResult ThrowError()
        {
            _logger.LogInformation("About to throw a test exception");
            throw new Exception("This is a test exception to see if GlobalExceptionFilter works!");
        }

        // Test 2: Null reference exception
        public IActionResult NullReferenceError()
        {
            string? name = null;
            // This will throw NullReferenceException
            var length = name!.Length;
            return View();
        }

        // Test 3: Division by zero
        public IActionResult DivisionError()
        {
            int x = 10;
            int y = 0;
            int result = x / y; // DivideByZeroException
            return View();
        }

        // Test 4: Invalid operation
        public IActionResult InvalidOperationError()
        {
            var list = new List<string>();
            var firstItem = list.First(); // InvalidOperationException
            return View();
        }

        // Test 5: Argument exception
        public IActionResult ArgumentError()
        {
            throw new ArgumentException("Invalid argument provided", "testParam");
        }

        // Test 6: Custom error with message
        public IActionResult CustomError(string message = "Default error message")
        {
            throw new InvalidOperationException(message);
        }

        // Test 7: Simulate database error
        public IActionResult DatabaseError()
        {
            throw new Exception("Database connection failed: Unable to connect to SQL Server");
        }

        // Test Page - Lists all test options
        public IActionResult Index()
        {
            return View();
        }
    }
}