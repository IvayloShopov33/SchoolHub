namespace SchoolHub.Web.Controllers
{
    using System.Threading.Tasks;

    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;

    using SchoolHub.Common;
    using SchoolHub.Services;
    using SchoolHub.Web.Infrastructure;
    using SchoolHub.Web.ViewModels.Absence;
    using SchoolHub.Web.ViewModels.Student;

    [Authorize]
    public class AbsenceController : Controller
    {
        private readonly IAbsenceService absenceService;
        private readonly ICategoryService categoryService;
        private readonly ITeacherService teacherService;
        private readonly IStudentService studentService;

        public AbsenceController(IAbsenceService absenceService, ICategoryService categoryService, ITeacherService teacherService, IStudentService studentService)
        {
            this.absenceService = absenceService;
            this.categoryService = categoryService;
            this.teacherService = teacherService;
            this.studentService = studentService;
        }

        public async Task<IActionResult> Index(string studentId)
        {
            if (!this.User.IsAdmin() && !this.User.IsTeacher() && !this.User.IsStudent())
            {
                return this.Unauthorized();
            }

            var student = await this.studentService.GetStudentByIdAsync(studentId);
            var absences = await this.absenceService.GetAbsencesByStudentIdAsync(studentId);

            return this.View(new StudentAbsencesViewModel
            {
                Id = studentId,
                StudentName = student.FullName,
                ClassId = student.ClassId,
                Absences = absences,
            });
        }

        public async Task<IActionResult> GroupedAbsences(string studentId)
        {
            if (!this.User.IsAdmin() && !this.User.IsTeacher() && !this.User.IsStudent())
            {
                return this.Unauthorized();
            }

            var groupedAbsences = await this.absenceService.GetGroupedAbsencesByStudentAsync(studentId);

            return this.View(groupedAbsences);
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

            return this.RedirectToAction("Index", new { studentId = studentId });
        }

        [Authorize(Roles = GlobalConstants.TeacherRoleName)]
        public async Task<IActionResult> Edit(string id)
        {
            var absence = await this.absenceService.GetAbsenceByIdAsync(id);
            var teacher = await this.teacherService.GetTeacherByUserIdAsync(this.User.GetId());

            if (teacher.SubjectId != absence.SubjectId)
            {
                return this.BadRequest();
            }

            absence.Categories = await this.categoryService.GetAbsenceCategoriesAsync();

            return this.View(absence);
        }

        [Authorize(Roles = GlobalConstants.TeacherRoleName)]
        [HttpPost]
        public async Task<IActionResult> Edit(string id, AbsenceFormModel formModel)
        {
            var absence = await this.absenceService.GetAbsenceByIdAsync(id);
            var teacher = await this.teacherService.GetTeacherByUserIdAsync(this.User.GetId());

            if (teacher.SubjectId != absence.SubjectId)
            {
                return this.BadRequest();
            }

            formModel.SubjectId = absence.SubjectId;
            formModel.StudentId = absence.StudentId;
            formModel.TeacherId = absence.TeacherId;

            await this.absenceService.EditAbsenceAsync(id, formModel);

            return this.RedirectToAction("Index", new { studentId = absence.StudentId });
        }

        public async Task<IActionResult> Delete(string id)
        {
            if (!this.User.IsAdmin() && !await this.teacherService.IsTeacherAsync(this.User.GetId()))
            {
                return this.Unauthorized();
            }

            var absence = await this.absenceService.GetAbsenceByIdAsync(id);

            await this.absenceService.DeleteAbsenceAsync(id);

            return this.RedirectToAction("Index", new { studentId = absence.StudentId });
        }
    }
}
