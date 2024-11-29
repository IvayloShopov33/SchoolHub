namespace SchoolHub.Web.Controllers
{
    using System;
    using System.Linq;
    using System.Threading.Tasks;

    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;

    using SchoolHub.Common;
    using SchoolHub.Services;
    using SchoolHub.Services.Mapping;
    using SchoolHub.Web.Infrastructure;
    using SchoolHub.Web.ViewModels.Teacher;

    [Authorize]
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

        [Authorize(Roles = GlobalConstants.AdministratorRoleName)]
        public async Task<IActionResult> Index(string schoolId, int page = 1, string searchTerm = "")
        {
            const int PageSize = 3;
            var teachers = await this.teacherService.GetAllTeachersBySchoolIdAsync(schoolId);

            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                teachers = teachers
                    .Where(t => t.FullName.Contains(searchTerm, StringComparison.OrdinalIgnoreCase))
                    .ToList();
            }

            var paginatedTeachers = teachers
                .Skip((page - 1) * PageSize)
                .Take(PageSize)
                .ToList();

            var totalTeachers = teachers.Count;
            var totalPages = (int)Math.Ceiling((double)totalTeachers / PageSize);

            return this.View(new PaginatedIndexTeacherViewModel
            {
                Teachers = paginatedTeachers,
                CurrentPage = page,
                TotalPages = totalPages,
                SchoolId = schoolId,
                SearchTerm = searchTerm,
            });
        }

        public async Task<IActionResult> Mine(string userId)
        {
            if (!this.User.IsAdmin() && !await this.teacherService.IsTeacherAsync(userId))
            {
                return this.Unauthorized();
            }

            var teacher = await this.teacherService.GetTeacherByUserIdAsync(userId);
            var teacherClassViewModel = await this.classService.GetTeacherClassByIdAsync(teacher.ClassId);

            return this.View(teacherClassViewModel);
        }

        [Authorize(Roles = GlobalConstants.AdministratorRoleName)]
        public async Task<IActionResult> Add(string schoolId)
            => this.View(new TeacherFormModel
            {
                SchoolId = schoolId,
                Classes = await this.classService.GetAllTeacherClassesBySchoolIdAsync(schoolId),
                Subjects = await this.subjectService.GetAllSubjectsAsync(),
            });

        [Authorize(Roles = GlobalConstants.AdministratorRoleName)]
        [HttpPost]
        public async Task<IActionResult> Add(string schoolId, TeacherFormModel formModel)
        {
            if (!this.ModelState.IsValid)
            {
                formModel.SchoolId = schoolId;
                formModel.Classes = await this.classService.GetAllTeacherClassesBySchoolIdAsync(schoolId);
                formModel.Subjects = await this.subjectService.GetAllSubjectsAsync();

                return this.View(formModel);
            }

            var teacherId = await this.teacherService.AddTeacherAsync(formModel);
            if (formModel.ClassId != null)
            {
                await this.classService.SetHomeroomTeacherIdByClassIdAsync(formModel.ClassId, teacherId);
            }

            return this.RedirectToAction("Details", "School", new { id = schoolId });
        }

        public IActionResult Become()
            => this.View(new BecomeTeacherFormModel { Email = this.User.GetEmail() });

        [HttpPost]
        public async Task<IActionResult> Become(BecomeTeacherFormModel formModel)
        {
            if (!this.ModelState.IsValid)
            {
                return this.View(new BecomeTeacherFormModel { Email = this.User.GetEmail() });
            }

            var teacherUserId = await this.teacherService.SetTeacherUserByFullNameAndBirthDateAsync(this.User.GetId(), formModel.FullName, formModel.BirthDate);

            if (teacherUserId == null)
            {
                this.ModelState.AddModelError(nameof(formModel.BirthDate), "Invalid Full Name or Birth Date.");

                return this.View(new BecomeTeacherFormModel { Email = this.User.GetEmail() });
            }

            return this.RedirectToAction("Mine", new { userId = teacherUserId });
        }

        [Authorize(Roles = GlobalConstants.AdministratorRoleName)]
        public async Task<IActionResult> Edit(string id)
        {
            var teacher = await this.teacherService.GetTeacherByIdAsync(id);

            teacher.Classes = await this.classService.GetAllTeacherClassesBySchoolIdAsync(teacher.SchoolId, teacher.Id);
            teacher.Subjects = await this.subjectService.GetAllSubjectsAsync();

            return this.View(teacher);
        }

        [Authorize(Roles = GlobalConstants.AdministratorRoleName)]
        [HttpPost]
        public async Task<IActionResult> Edit(string id, TeacherFormModel formModel)
        {
            if (!this.ModelState.IsValid)
            {
                formModel.Classes = await this.classService.GetAllTeacherClassesBySchoolIdAsync(formModel.SchoolId, formModel.Id);
                formModel.Subjects = await this.subjectService.GetAllSubjectsAsync();

                return this.View(formModel);
            }

            await this.teacherService.EditTeacherAsync(id, formModel);

            return this.RedirectToAction("Index", new { schoolId = formModel.SchoolId });
        }

        [Authorize(Roles = GlobalConstants.AdministratorRoleName)]
        public async Task<IActionResult> Delete(string id)
        {
            var teacher = await this.teacherService.GetTeacherByIdAsync(id);
            var deleteTeacherViewModel = AutoMapperConfig.MapperInstance.Map<DeleteTeacherViewModel>(teacher);

            return this.View(deleteTeacherViewModel);
        }

        [Authorize(Roles = GlobalConstants.AdministratorRoleName)]
        [HttpPost]
        public async Task<IActionResult> DeleteConfirmed(DeleteTeacherViewModel formModel)
        {
            await this.teacherService.DeleteTeacherAsync(formModel);

            return this.RedirectToAction("Index", new { schoolId = formModel.SchoolId });
        }
    }
}
