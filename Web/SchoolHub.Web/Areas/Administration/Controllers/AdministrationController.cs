namespace SchoolHub.Web.Areas.Administration.Controllers
{
    using System.Threading.Tasks;

    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;

    using SchoolHub.Common;
    using SchoolHub.Services;
    using SchoolHub.Web.Controllers;
    using SchoolHub.Web.ViewModels.School;

    [Authorize(Roles = GlobalConstants.AdministratorRoleName)]
    [Area("Administration")]
    public class AdministrationController : BaseController
    {
        private readonly ISchoolService schoolService;

        public AdministrationController(ISchoolService schoolService)
        {
            this.schoolService = schoolService;
        }

        public IActionResult Add()
        {
            return this.View();
        }

        [HttpPost]
        public async Task<IActionResult> Add(SchoolFormModel formModel)
        {
            if (!this.ModelState.IsValid)
            {
                return this.View(formModel);
            }

            await this.schoolService.AddSchoolAsync(formModel);

            return this.RedirectToAction("Index", "School", new { area = string.Empty });
        }
    }
}
