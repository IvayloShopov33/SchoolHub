namespace SchoolHub.Web.Controllers
{
    using System.Threading.Tasks;

    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;

    using SchoolHub.Common;
    using SchoolHub.Services;
    using SchoolHub.Services.Mapping;
    using SchoolHub.Web.ViewModels.Class;

    [Authorize(Roles = GlobalConstants.AdministratorRoleName)]
    public class ClassController : Controller
    {
        private readonly IClassService classService;
        private readonly ITeacherService teacherService;

        public ClassController(IClassService classService, ITeacherService teacherService)
        {
            this.classService = classService;
            this.teacherService = teacherService;
        }

        public async Task<IActionResult> Index(string schoolId)
        {
            var allClasses = await this.classService.GetAllClassesBySchoolIdAsync(schoolId);

            return this.View(allClasses);
        }

        public IActionResult Add(string teacherId)
        {
            return this.View(new ClassFormModel { HomeroomTeacherId = teacherId });
        }

        [HttpPost]
        public async Task<IActionResult> Add(string teacherId, ClassFormModel formModel)
        {
            if (await this.teacherService.IsTeacherAsync(teacherId))
            {
                return this.BadRequest();
            }

            var schoolId = await this.teacherService.GetSchoolIdByTeacherId(teacherId);

            formModel.HomeroomTeacherId = teacherId;
            formModel.SchoolId = schoolId;

            if (!this.ModelState.IsValid || formModel.StartedOn > formModel.EndingOn)
            {
                formModel.HomeroomTeacherId = teacherId;
                formModel.SchoolId = schoolId;

                return this.View(formModel);
            }

            await this.classService.AddClassAsync(formModel);

            return this.RedirectToAction("Index", new { schoolId = schoolId });
        }

        public IActionResult AddBySchool(string schoolId)
        {
            return this.View("Add", new ClassFormModel { SchoolId = schoolId });
        }

        [HttpPost]
        public async Task<IActionResult> AddBySchool(string schoolId, ClassFormModel formModel)
        {
            formModel.SchoolId = schoolId;

            if (!this.ModelState.IsValid || formModel.StartedOn > formModel.EndingOn)
            {
                formModel.SchoolId = schoolId;

                return this.View("Add", formModel);
            }

            await this.classService.AddClassAsync(formModel);

            return this.RedirectToAction("Index", new { schoolId });
        }

        public async Task<IActionResult> Edit(string classId, string teacherId)
        {
            var classById = await this.classService.GetClassByIdAsync(classId);

            classById.HomeroomTeacherId = teacherId;
            classById.SchoolId = await this.classService.GetSchoolIdByClassId(classId);

            return this.View(classById);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(string classId, string teacherId, ClassFormModel formModel)
        {
            if (!string.IsNullOrEmpty(teacherId) && await this.teacherService.IsTeacherAsync(teacherId))
            {
                return this.BadRequest();
            }

            var schoolId = await this.classService.GetSchoolIdByClassId(classId);

            formModel.HomeroomTeacherId = teacherId;
            formModel.SchoolId = schoolId;

            if (!this.ModelState.IsValid || formModel.StartedOn > formModel.EndingOn)
            {
                formModel.HomeroomTeacherId = teacherId;
                formModel.SchoolId = schoolId;

                return this.View(formModel);
            }

            await this.classService.EditClassByIdAsync(classId, formModel);

            return this.RedirectToAction("Index", new { schoolId = schoolId });
        }

        public async Task<IActionResult> Delete(string id)
        {
            var classById = await this.classService.GetClassByIdAsync(id);
            var deleteClassFormModel = AutoMapperConfig.MapperInstance.Map<DeleteClassFormModel>(classById);

            return this.View(deleteClassFormModel);
        }

        [HttpPost]
        public async Task<IActionResult> DeleteConfirmed(DeleteClassFormModel formModel)
        {
            await this.classService.DeleteClassAsync(formModel);

            return this.RedirectToAction("Index", new { schoolId = formModel.SchoolId });
        }
    }
}
