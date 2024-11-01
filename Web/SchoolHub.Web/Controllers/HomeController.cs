namespace SchoolHub.Web.Controllers
{
    using System.Diagnostics;

    using Microsoft.AspNetCore.Mvc;

    using SchoolHub.Web.Infrastructure;
    using SchoolHub.Web.ViewModels;

    public class HomeController : Controller
    {
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
            return this.View(
                new ErrorViewModel { RequestId = Activity.Current?.Id ?? this.HttpContext.TraceIdentifier });
        }
    }
}
