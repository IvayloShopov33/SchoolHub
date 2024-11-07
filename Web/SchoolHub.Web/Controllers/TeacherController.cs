namespace SchoolHub.Web.Controllers
{
    using System.Threading.Tasks;

    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;

    using SchoolHub.Common;
    using SchoolHub.Services;
    using SchoolHub.Services.Mapping;
    using SchoolHub.Web.Infrastructure;
    using SchoolHub.Web.ViewModels.Teacher;

    [Authorize(Roles = GlobalConstants.AdministratorRoleName)]
    public class TeacherController : Controller
    {
        private readonly ITeacherService teacherService;
        private readonly IClassService classService;
        private readonly ISubjectService subjectService;

        public TeacherController(ITeacherService teacherService, IClassService classService, ISubjectService subjectService)
        {
            this.teacherService = teacherService;
            this.classService = classService;
            this.subjectService = subjectService;
        }

        public async Task<IActionResult> Index(string schoolId)
        {
            var teachers = await this.teacherService.AllTeachersAsync(schoolId);

            return this.View(teachers);
        }

        public async Task<IActionResult> Add(string schoolId)
        {
            return this.View(new TeacherFormModel
            {
                SchoolId = schoolId,
                Classes = await this.classService.GetAllTeacherClassesBySchoolIdAsync(schoolId),
                Subjects = await this.subjectService.GetAllSubjects(),
            });
        }

        [HttpPost]
        public async Task<IActionResult> Add(string schoolId, TeacherFormModel formModel)
        {
            if (!this.ModelState.IsValid)
            {
                formModel.SchoolId = schoolId;
                formModel.Classes = await this.classService.GetAllTeacherClassesBySchoolIdAsync(schoolId);
                formModel.Subjects = await this.subjectService.GetAllSubjects();

                return this.View(formModel);
            }

            await this.teacherService.AddTeacherAsync(formModel);

            return this.RedirectToAction("Details", "School", new { id = schoolId });
        }

        public async Task<IActionResult> Edit(string id)
        {
            var teacher = await this.teacherService.GetTeacherByIdAsync(id);

            teacher.Classes = await this.classService.GetAllTeacherClassesBySchoolIdAsync(teacher.SchoolId);
            teacher.Subjects = await this.subjectService.GetAllSubjects();

            return this.View(teacher);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(string id, TeacherFormModel formModel)
        {
            if (!this.ModelState.IsValid)
            {
                formModel.Classes = await this.classService.GetAllTeacherClassesBySchoolIdAsync(formModel.SchoolId);
                formModel.Subjects = await this.subjectService.GetAllSubjects();

                return this.View(formModel);
            }

            await this.teacherService.EditTeacherAsync(id, formModel);

            return this.RedirectToAction("Index", new { schoolId = formModel.SchoolId });
        }

        public async Task<IActionResult> Delete(string id)
        {
            var teacher = await this.teacherService.GetTeacherByIdAsync(id);
            var deleteTeacherViewModel = AutoMapperConfig.MapperInstance.Map<DeleteTeacherViewModel>(teacher);

            return this.View(deleteTeacherViewModel);
        }

        [HttpPost]
        public async Task<IActionResult> DeleteConfirmed(DeleteTeacherViewModel formModel)
        {
            await this.teacherService.DeleteTeacherAsync(formModel);

            return this.RedirectToAction("Index", new { schoolId = formModel.SchoolId });
        }
    }
}
