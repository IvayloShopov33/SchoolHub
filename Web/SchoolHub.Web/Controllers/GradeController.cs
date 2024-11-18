namespace SchoolHub.Web.Controllers
{
    using System.Threading.Tasks;

    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using SchoolHub.Common;
    using SchoolHub.Services;
    using SchoolHub.Web.ViewModels.Grade;
    using SchoolHub.Web.ViewModels.Student;

    [Authorize]
    public class GradeController : Controller
    {
        private readonly IGradeService gradeService;
        private readonly IStudentService studentService;
        private readonly ITeacherService teacherService;
        private readonly ICategoryService categoryService;

        public GradeController(IGradeService gradeService, IStudentService studentService, ITeacherService teacherService, ICategoryService categoryService)
        {
            this.gradeService = gradeService;
            this.studentService = studentService;
            this.teacherService = teacherService;
            this.categoryService = categoryService;
        }

        public async Task<IActionResult> Index(string studentId)
        {
            var student = await this.studentService.GetStudentDetailsByIdAsync(studentId);
            if (student == null)
            {
                return this.NotFound();
            }

            return this.View(new StudentGradesViewModel
            {
                StudentId = studentId,
                StudentName = student.FullName,
                SubjectGrades = this.studentService.GetStudentGradesGroupBySubjectAsync(student),
            });
        }

        [Authorize(Roles = GlobalConstants.TeacherRoleName)]
        public async Task<IActionResult> Add(string studentId, string teacherId)
        {
            var subjectId = await this.teacherService.GetSubjectIdByTeacherId(teacherId);

            return this.View(new GradeFormModel
            {
                StudentId = studentId,
                SubjectId = subjectId,
                TeacherId = teacherId,
                Categories = await this.categoryService.GetGradeCategoriesAsync(),
            });
        }

        [Authorize(Roles = GlobalConstants.TeacherRoleName)]
        [HttpPost]
        public async Task<IActionResult> Add(string studentId, string teacherId, GradeFormModel formModel)
        {
            var subjectId = await this.teacherService.GetSubjectIdByTeacherId(teacherId);

            if (!this.ModelState.IsValid)
            {
                return this.View(new GradeFormModel
                {
                    StudentId = studentId,
                    SubjectId = subjectId,
                    TeacherId = teacherId,
                    Categories = await this.categoryService.GetGradeCategoriesAsync(),
                });
            }

            formModel.SubjectId = subjectId;

            await this.gradeService.AddGradeAsync(formModel);

            return this.RedirectToAction("Index", new { studentId = studentId });
        }
    }
}
