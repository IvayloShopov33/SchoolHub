namespace SchoolHub.Web.Controllers
{
    using System;
    using System.Linq;
    using System.Threading.Tasks;

    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.EntityFrameworkCore;
    using SchoolHub.Common;
    using SchoolHub.Services;
    using SchoolHub.Services.Mapping;
    using SchoolHub.Web.Infrastructure;
    using SchoolHub.Web.ViewModels.Class;

    [Authorize]
    public class ClassController : Controller
    {
        private readonly IClassService classService;
        private readonly ITeacherService teacherService;

        public ClassController(IClassService classService, ITeacherService teacherService)
        {
            this.classService = classService;
            this.teacherService = teacherService;
        }

        public async Task<IActionResult> Index(string schoolId, int page = 1, string searchTerm = "")
        {
            if (!this.User.IsAdmin() && !await this.teacherService.IsTeacherAsync(this.User.GetId()))
            {
                return this.Unauthorized();
            }

            const int PageSize = 3;
            var allClasses = await this.classService.GetAllClassesBySchoolIdAsync(schoolId)
                .To<IndexClassViewModel>()
                .ToListAsync();

            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                allClasses = allClasses
                    .Where(c => c.Name.Contains(searchTerm, StringComparison.OrdinalIgnoreCase))
                    .ToList();
            }

            var paginatedClasses = allClasses
                .Skip((page - 1) * PageSize)
                .Take(PageSize)
                .ToList();

            var totalClasses = allClasses.Count;
            var totalPages = (int)Math.Ceiling((double)totalClasses / PageSize);

            return this.View(new PaginatedIndexClassViewModel
            {
                Classes = paginatedClasses,
                CurrentPage = page,
                TotalPages = totalPages,
                SchoolId = schoolId,
                SearchTerm = searchTerm,
            });
        }

        public async Task<IActionResult> AllStudentsDetails(string id)
        {
            var classById = await this.classService.GetClassByIdAsync(id);

            var classWithStudentsViewModel = AutoMapperConfig.MapperInstance.Map<ClassWithStudentsViewModel>(classById);
            classWithStudentsViewModel.Id = id;
            classWithStudentsViewModel.Students = await this.classService.GetAllStudentsByClassIdAsync(id);

            return this.View(classWithStudentsViewModel);
        }

        public async Task<IActionResult> Chat(string classId)
        {
            if (this.User.IsAdmin())
            {
                return this.Unauthorized();
            }

            var classById = await this.classService.GetClassByIdAsync(classId);
            var teacherById = await this.teacherService.GetTeacherByIdAsync(classById.HomeroomTeacherId);

            this.ViewBag.ClassId = classId;
            this.ViewBag.Class = classById.Name;
            this.ViewBag.Teacher = teacherById.FullName;

            this.ViewBag.UserId = this.User.GetId();

            return this.View();
        }

        [Authorize(Roles = GlobalConstants.AdministratorRoleName)]
        public IActionResult Add(string teacherId)
            => this.View(new ClassFormModel { HomeroomTeacherId = teacherId });

        [Authorize(Roles = GlobalConstants.AdministratorRoleName)]
        [HttpPost]
        public async Task<IActionResult> Add(string teacherId, ClassFormModel formModel)
        {
            if (await this.teacherService.IsTeacherAsync(teacherId))
            {
                return this.BadRequest();
            }

            var schoolId = await this.teacherService.GetSchoolIdByTeacherIdAsync(teacherId);

            formModel.HomeroomTeacherId = teacherId;
            formModel.SchoolId = schoolId;

            if (!this.ModelState.IsValid || formModel.StartedOn > formModel.EndingOn)
            {
                if (formModel.StartedOn > formModel.EndingOn)
                {
                    this.ModelState.AddModelError(nameof(formModel.EndingOn), "The End Date cannot be before the Start Date.");
                }

                formModel.HomeroomTeacherId = teacherId;
                formModel.SchoolId = schoolId;

                return this.View(formModel);
            }

            formModel.Id = Guid.NewGuid().ToString();

            var classId = await this.classService.AddClassAsync(formModel);
            await this.teacherService.SetClassIdByHomeroomTeacherIdAsync(teacherId, classId);

            return this.RedirectToAction("Index", new { schoolId = schoolId });
        }

        [Authorize(Roles = GlobalConstants.AdministratorRoleName)]
        public IActionResult AddBySchool(string schoolId)
        {
            return this.View("Add", new ClassFormModel { SchoolId = schoolId });
        }

        [Authorize(Roles = GlobalConstants.AdministratorRoleName)]
        [HttpPost]
        public async Task<IActionResult> AddBySchool(string schoolId, ClassFormModel formModel)
        {
            formModel.SchoolId = schoolId;

            if (!this.ModelState.IsValid || formModel.StartedOn > formModel.EndingOn)
            {
                if (formModel.StartedOn > formModel.EndingOn)
                {
                    this.ModelState.AddModelError(nameof(formModel.EndingOn), "The End Date cannot be before the Start Date.");
                }

                formModel.SchoolId = schoolId;

                return this.View("Add", formModel);
            }

            formModel.Id = Guid.NewGuid().ToString();
            await this.classService.AddClassAsync(formModel);

            return this.RedirectToAction("Index", new { schoolId });
        }

        [Authorize(Roles = GlobalConstants.AdministratorRoleName)]
        public async Task<IActionResult> Edit(string classId, string teacherId)
        {
            var classById = await this.classService.GetClassByIdAsync(classId);

            classById.HomeroomTeacherId = teacherId;
            classById.SchoolId = await this.classService.GetSchoolIdByClassIdAsync(classId);

            return this.View(classById);
        }

        [Authorize(Roles = GlobalConstants.AdministratorRoleName)]
        [HttpPost]
        public async Task<IActionResult> Edit(string classId, string teacherId, ClassFormModel formModel)
        {
            if (!string.IsNullOrEmpty(teacherId) && await this.teacherService.IsTeacherAsync(teacherId))
            {
                return this.BadRequest();
            }

            var schoolId = await this.classService.GetSchoolIdByClassIdAsync(classId);

            formModel.HomeroomTeacherId = teacherId;
            formModel.SchoolId = schoolId;

            if (!this.ModelState.IsValid || formModel.StartedOn > formModel.EndingOn)
            {
                if (formModel.StartedOn > formModel.EndingOn)
                {
                    this.ModelState.AddModelError(nameof(formModel.EndingOn), "The End Date cannot be before the Start Date.");
                }

                formModel.HomeroomTeacherId = teacherId;
                formModel.SchoolId = schoolId;

                return this.View(formModel);
            }

            await this.classService.EditClassByIdAsync(classId, formModel);

            return this.RedirectToAction("Index", new { schoolId = schoolId });
        }

        [Authorize(Roles = GlobalConstants.AdministratorRoleName)]
        public async Task<IActionResult> Delete(string id)
        {
            var classById = await this.classService.GetClassByIdAsync(id);
            var deleteClassViewModel = AutoMapperConfig.MapperInstance.Map<DeleteClassViewModel>(classById);

            return this.View(deleteClassViewModel);
        }

        [Authorize(Roles = GlobalConstants.AdministratorRoleName)]
        [HttpPost]
        public async Task<IActionResult> DeleteConfirmed(DeleteClassViewModel formModel)
        {
            await this.classService.DeleteClassAsync(formModel);

            return this.RedirectToAction("Index", new { schoolId = formModel.SchoolId });
        }
    }
}
