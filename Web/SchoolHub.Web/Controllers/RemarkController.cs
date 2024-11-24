namespace SchoolHub.Web.Controllers
{
    using System.Threading.Tasks;

    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;

    using SchoolHub.Common;
    using SchoolHub.Data.Models;
    using SchoolHub.Services;
    using SchoolHub.Web.Infrastructure;
    using SchoolHub.Web.ViewModels.Remark;
    using SchoolHub.Web.ViewModels.Student;

    public class RemarkController : Controller
    {
        private readonly IRemarkService remarkService;
        private readonly ICategoryService categoryService;
        private readonly ITeacherService teacherService;
        private readonly IStudentService studentService;

        public RemarkController(IRemarkService remarkService, ICategoryService categoryService, ITeacherService teacherService, IStudentService studentService)
        {
            this.remarkService = remarkService;
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
            var remarks = await this.remarkService.GetRemarksByStudentIdAsync(studentId);

            return this.View(new StudentRemarksViewModel
            {
                Id = studentId,
                StudentName = student.FullName,
                ClassId = student.ClassId,
                Remarks = remarks,
            });
        }

        [Authorize(Roles = GlobalConstants.TeacherRoleName)]
        public async Task<IActionResult> Add(string studentId, string teacherId)
        {
            var subjectId = await this.teacherService.GetSubjectIdByTeacherIdAsync(teacherId);

            return this.View(new RemarkFormModel
            {
                StudentId = studentId,
                SubjectId = subjectId,
                TeacherId = teacherId,
            });
        }

        [Authorize(Roles = GlobalConstants.TeacherRoleName)]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Add(string studentId, string teacherId, RemarkFormModel formModel)
        {
            var subjectId = await this.teacherService.GetSubjectIdByTeacherIdAsync(teacherId);

            if (!this.ModelState.IsValid)
            {
                return this.View(new RemarkFormModel
                {
                    StudentId = studentId,
                    SubjectId = subjectId,
                    TeacherId = teacherId,
                });
            }

            formModel.SubjectId = subjectId;

            await this.remarkService.AddRemarkAsync(formModel);

            return this.RedirectToAction("Index", new { studentId = formModel.StudentId });
        }
    }
}
