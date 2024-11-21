namespace SchoolHub.Web.Controllers
{
    using System.Diagnostics;

    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Logging;

    using SchoolHub.Web.Infrastructure;
    using SchoolHub.Web.ViewModels;

    public class HomeController : Controller
    {
        private readonly ILogger logger;

        public HomeController(ILogger<HomeController> logger)
        {
            this.logger = logger;
        }

        public IActionResult Index()
        {
            if (this.User.GetId() != null)
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
    }
}
