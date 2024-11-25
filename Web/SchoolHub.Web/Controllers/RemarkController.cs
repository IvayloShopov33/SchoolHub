namespace SchoolHub.Web.Controllers
{
    using System;
    using System.Threading.Tasks;

    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;

    using SchoolHub.Common;
    using SchoolHub.Data.Models;
    using SchoolHub.Services;
    using SchoolHub.Web.Infrastructure;
    using SchoolHub.Web.ViewModels.Absence;
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

        public async Task<IActionResult> Index(string studentId, int page = 1, int itemsPerPage = 10)
        {
            if (!this.User.IsAdmin() && !this.User.IsTeacher() && !this.User.IsStudent())
            {
                return this.Unauthorized();
            }

            var student = await this.studentService.GetStudentByIdAsync(studentId);
            var (remarks, totalCount) = await this.remarkService.GetRemarksByStudentIdAsync(studentId, page, itemsPerPage);

            var totalPages = (int)Math.Ceiling((double)totalCount / itemsPerPage);

            return this.View(new StudentRemarksViewModel
            {
                Id = studentId,
                StudentName = student.FullName,
                ClassId = student.ClassId,
                Remarks = remarks,
                CurrentPage = page,
                TotalPages = totalPages,
                ItemsPerPage = itemsPerPage,
            });
        }

        public async Task<IActionResult> GroupedRemarks(string studentId)
        {
            if (!this.User.IsAdmin() && !this.User.IsTeacher() && !this.User.IsStudent())
            {
                return this.Unauthorized();
            }

            var student = await this.studentService.GetStudentByIdAsync(studentId);

            return this.View(new StudentGroupedRemarksViewModel
            {
                StudentName = student.FullName,
                GroupedRemarks = await this.remarkService.GetGroupedRemarksByStudentAsync(studentId),
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

        [Authorize(Roles = GlobalConstants.TeacherRoleName)]
        public async Task<IActionResult> Edit(string id)
        {
            var remark = await this.remarkService.GetRemarkByIdAsync(id);
            var teacher = await this.teacherService.GetTeacherByUserIdAsync(this.User.GetId());

            if (teacher.SubjectId != remark.SubjectId)
            {
                return this.BadRequest();
            }

            return this.View(remark);
        }

        [Authorize(Roles = GlobalConstants.TeacherRoleName)]
        [HttpPost]
        public async Task<IActionResult> Edit(string id, RemarkFormModel formModel)
        {
            var remark = await this.remarkService.GetRemarkByIdAsync(id);
            var teacher = await this.teacherService.GetTeacherByUserIdAsync(this.User.GetId());

            if (!this.ModelState.IsValid)
            {
                return this.View(remark);
            }

            if (teacher.SubjectId != remark.SubjectId)
            {
                return this.BadRequest();
            }

            await this.remarkService.EditRemarkAsync(id, formModel);

            return this.RedirectToAction("Index", new { studentId = remark.StudentId });
        }

        public async Task<IActionResult> Delete(string id)
        {
            if (!this.User.IsAdmin() && !await this.teacherService.IsTeacherAsync(this.User.GetId()))
            {
                return this.Unauthorized();
            }

            var remark = await this.remarkService.GetRemarkByIdAsync(id);

            await this.remarkService.DeleteRemarkAsync(id);

            return this.RedirectToAction("Index", new { studentId = remark.StudentId });
        }
    }
}
