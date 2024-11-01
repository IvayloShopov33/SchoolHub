namespace SchoolHub.Web.Controllers
{
    using System.Threading.Tasks;

    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;

    using SchoolHub.Common;
    using SchoolHub.Services;
    using SchoolHub.Services.Mapping;
    using SchoolHub.Web.Infrastructure;
    using SchoolHub.Web.ViewModels.School;

    public class SchoolController : Controller
    {
        private readonly ISchoolService schoolService;

        public SchoolController(ISchoolService schoolService)
        {
            this.schoolService = schoolService;
        }

        [AllowAnonymous]
        public async Task<IActionResult> Index()
        {
            var schools = await this.schoolService.AllAsync();

            return this.View(schools);
        }

        [AllowAnonymous]
        public async Task<IActionResult> Details(string id)
        {
            var schoolById = await this.schoolService.GetSchoolDetailsByIdAsync(id);

            return this.View(schoolById);
        }

        [Authorize(Roles = GlobalConstants.AdministratorRoleName)]
        public IActionResult Add()
        {
            if (!this.User.IsAdmin())
            {
                return this.BadRequest();
            }

            return this.View();
        }

        [Authorize(Roles = GlobalConstants.AdministratorRoleName)]
        [HttpPost]
        public async Task<IActionResult> Add(SchoolFormModel formModel)
        {
            if (!this.ModelState.IsValid)
            {
                return this.View(formModel);
            }

            await this.schoolService.AddSchoolAsync(formModel);

            return this.RedirectToAction("Index");
        }

        [Authorize(Roles = GlobalConstants.AdministratorRoleName)]
        public async Task<IActionResult> Edit(string id)
        {
            if (!this.User.IsAdmin())
            {
                return this.BadRequest();
            }

            var school = await this.schoolService.GetSchoolDetailsByIdAsync(id);
            var schoolFormModel = AutoMapperConfig.MapperInstance.Map<SchoolFormModel>(school);

            return this.View(schoolFormModel);
        }

        [Authorize(Roles = GlobalConstants.AdministratorRoleName)]
        [HttpPost]
        public async Task<IActionResult> Edit(string id, SchoolFormModel school)
        {
            if (!this.User.IsAdmin())
            {
                return this.Unauthorized();
            }

            if (!this.ModelState.IsValid)
            {
                return this.View(school);
            }

            await this.schoolService.EditSchoolAsync(id, school);

            return this.RedirectToAction("Details", new { id = id });
        }

        [Authorize(Roles = GlobalConstants.AdministratorRoleName)]
        public async Task<IActionResult> Delete(string id)
        {
            if (!this.User.IsAdmin())
            {
                return this.Unauthorized();
            }

            var school = await this.schoolService.GetSchoolDetailsByIdAsync(id);
            var deleteSchoolViewModel = AutoMapperConfig.MapperInstance.Map<DeleteSchoolViewModel>(school);

            return this.View(deleteSchoolViewModel);
        }

        [Authorize(Roles = GlobalConstants.AdministratorRoleName)]
        [HttpPost]
        public async Task<IActionResult> DeleteConfirmed(DeleteSchoolViewModel formModel)
        {
            if (!this.User.IsAdmin())
            {
                return this.Unauthorized();
            }

            await this.schoolService.DeleteSchoolAsync(formModel);

            return this.RedirectToAction("Index");
        }
    }
}
