namespace SchoolHub.Web.Controllers
{
    using System.Threading.Tasks;

    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;

    using SchoolHub.Common;
    using SchoolHub.Services;
    using SchoolHub.Web.ViewModels.Absence;
    using SchoolHub.Web.ViewModels.Grade;

    [Authorize]
    public class AbsenceController : Controller
    {
        private readonly IAbsenceService absenceService;
        private readonly ICategoryService categoryService;
        private readonly ITeacherService teacherService;

        public AbsenceController(IAbsenceService absenceService, ICategoryService categoryService, ITeacherService teacherService)
        {
            this.absenceService = absenceService;
            this.categoryService = categoryService;
            this.teacherService = teacherService;
        }

        [Authorize(Roles = GlobalConstants.TeacherRoleName)]
        public async Task<IActionResult> Add(string studentId, string teacherId)
        {
            var subjectId = await this.teacherService.GetSubjectIdByTeacherIdAsync(teacherId);

            return this.View(new AbsenceFormModel
            {
                StudentId = studentId,
                SubjectId = subjectId,
                TeacherId = teacherId,
                Categories = await this.categoryService.GetAbsenceCategoriesAsync(),
            });
        }

        [Authorize(Roles = GlobalConstants.TeacherRoleName)]
        [HttpPost]
        public async Task<IActionResult> Add(string studentId, string teacherId, AbsenceFormModel formModel)
        {
            var subjectId = await this.teacherService.GetSubjectIdByTeacherIdAsync(teacherId);

            if (!this.ModelState.IsValid)
            {
                return this.View(new AbsenceFormModel
                {
                    StudentId = studentId,
                    SubjectId = subjectId,
                    TeacherId = teacherId,
                    Categories = await this.categoryService.GetAbsenceCategoriesAsync(),
                });
            }

            formModel.SubjectId = subjectId;

            await this.absenceService.AddAbsenceAsync(formModel);

            return this.RedirectToAction("Index", "Home");
        }
    }
}
