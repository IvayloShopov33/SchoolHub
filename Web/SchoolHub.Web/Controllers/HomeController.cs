namespace SchoolHub.Web.Controllers
{
    using System.Diagnostics;
    using System.Threading.Tasks;

    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Logging;

    using SchoolHub.Services;
    using SchoolHub.Web.Infrastructure;
    using SchoolHub.Web.ViewModels;

    public class HomeController : Controller
    {
        private readonly ILogger logger;
        private readonly IStudentService studentService;

        public HomeController(ILogger<HomeController> logger, IStudentService studentService)
        {
            this.logger = logger;
            this.studentService = studentService;
        }

        public async Task<IActionResult> Index()
        {
            var userId = this.User.GetId();

            if (this.User.IsStudent())
            {
                var studentId = await this.studentService.GetStudentIdByUserIdAsync(userId);

                return this.RedirectToAction("Statistics", "Student", new { studentId = studentId });
            }

            if (userId != null)
            {
                return this.RedirectToAction("Index", "School");
            }

            return this.View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            var requestId = Activity.Current?.Id ?? this.HttpContext.TraceIdentifier;

            this.logger.LogError($"Error with traceId = {requestId}");

            return this.View(
                new ErrorViewModel { RequestId = requestId });
        }

        public IActionResult Status(int? code)
        {
            if (!code.HasValue)
            {
                this.logger.LogWarning("No status code provided.");

                return this.RedirectToAction("Index");
            }

            var statusCode = code.Value;

            this.logger.LogError($"Error with status code {statusCode} occurred.");

            switch (statusCode)
            {
                case 400:
                    return this.View("BadRequest");
                case 401:
                    return this.View("Unauthorized");
                case 404:
                    return this.View("NotFound");
                case 500:
                    return this.View("ServerError");
                default:
                    return this.View("Error");
            }
        }
    }
}
