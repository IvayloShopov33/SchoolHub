namespace SchoolHub.Web.Controllers
{
    using System.Threading.Tasks;

    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.EntityFrameworkCore;

    using SchoolHub.Common;
    using SchoolHub.Services;
    using SchoolHub.Services.Mapping;
    using SchoolHub.Web.Infrastructure;
    using SchoolHub.Web.ViewModels.Student;

    [Authorize]
    public class StudentController : Controller
    {
        private readonly IStudentService studentService;
        private readonly IClassService classService;

        public StudentController(IStudentService studentService, IClassService classService)
        {
            this.studentService = studentService;
            this.classService = classService;
        }

        public async Task<IActionResult> Mine(string userId)
        {
            if (!this.User.IsAdmin() && !await this.studentService.IsStudentAsync(userId))
            {
                return this.Unauthorized();
            }

            var student = await this.studentService.GetStudentByUserIdAsync(userId);

            return this.RedirectToAction("AllStudentsDetails", "Class", new { id = student.ClassId });
        }

        [Authorize(Roles = GlobalConstants.AdministratorRoleName)]
        public async Task<IActionResult> Add(string schoolId)
            => this.View(new StudentFormModel
            {
                SchoolId = schoolId,
                Classes = await this.classService.GetAllClassesBySchoolIdAsync(schoolId)
                    .To<StudentClassFormModel>()
                    .ToListAsync(),
            });

        [Authorize(Roles = GlobalConstants.AdministratorRoleName)]
        [HttpPost]
        public async Task<IActionResult> Add(string schoolId, StudentFormModel formModel)
        {
            if (!this.ModelState.IsValid)
            {
                formModel.SchoolId = schoolId;
                formModel.Classes = await this.classService.GetAllClassesBySchoolIdAsync(schoolId)
                    .To<StudentClassFormModel>()
                    .ToListAsync();

                return this.View(formModel);
            }

            await this.studentService.AddStudentAsync(formModel);

            return this.RedirectToAction("AllStudentsDetails", "Class", new { id = formModel.ClassId });
        }

        public IActionResult Become()
            => this.View(new BecomeStudentFormModel { Email = this.User.GetEmail() });

        [HttpPost]
        public async Task<IActionResult> Become(BecomeStudentFormModel formModel)
        {
            if (!this.ModelState.IsValid)
            {
                return this.View(new BecomeStudentFormModel { Email = this.User.GetEmail() });
            }

            var studentUserId = await this.studentService.SetStudentUserByFullNameAndBirthDateAsync(this.User.GetId(), formModel.FullName, formModel.BirthDate);

            if (studentUserId == null)
            {
                this.ModelState.AddModelError(nameof(formModel.BirthDate), "Invalid Full Name or Birth Date.");

                return this.View(new BecomeStudentFormModel { Email = this.User.GetEmail() });
            }

            return this.RedirectToAction("Mine", new { userId = studentUserId });
        }

        public async Task<IActionResult> Edit(string id)
        {
            var student = await this.studentService.GetStudentByIdAsync(id);
            student.Classes = await this.classService.GetAllClassesBySchoolIdAsync(student.SchoolId)
                .To<StudentClassFormModel>()
                .ToListAsync();

            return this.View(student);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(string id, StudentFormModel formModel)
        {
            if (!this.ModelState.IsValid)
            {
                formModel.Classes = await this.classService.GetAllClassesBySchoolIdAsync(formModel.SchoolId)
                    .To<StudentClassFormModel>()
                    .ToListAsync();

                return this.View(formModel);
            }

            await this.studentService.EditStudentAsync(id, formModel);

            return this.RedirectToAction("AllStudentsDetails", "Class", new { id = formModel.ClassId });
        }

        [Authorize(Roles = GlobalConstants.AdministratorRoleName)]
        public async Task<IActionResult> Delete(string id)
        {
            var student = await this.studentService.GetStudentByIdAsync(id);
            var deleteStudentViewModel = AutoMapperConfig.MapperInstance.Map<DeleteStudentViewModel>(student);

            return this.View(deleteStudentViewModel);
        }

        [Authorize(Roles = GlobalConstants.AdministratorRoleName)]
        [HttpPost]
        public async Task<IActionResult> DeleteConfirmed(DeleteStudentViewModel formModel)
        {
            await this.studentService.DeleteStudentAsync(formModel);

            return this.RedirectToAction("AllStudentsDetails", "Class", new { id = formModel.ClassId });
        }
    }
}
