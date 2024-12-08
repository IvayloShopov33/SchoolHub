namespace SchoolHub.Services
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    using Microsoft.AspNetCore.Identity;
    using Microsoft.EntityFrameworkCore;

    using SchoolHub.Common;
    using SchoolHub.Data.Common.Repositories;
    using SchoolHub.Data.Models;
    using SchoolHub.Services.Mapping;
    using SchoolHub.Web.ViewModels.Grade;
    using SchoolHub.Web.ViewModels.Student;
    using SchoolHub.Web.ViewModels.Subject;

    using static SchoolHub.Data.Common.ModelsValidationConstraints;

    public class StudentService : IStudentService
    {
        private readonly IDeletableEntityRepository<Student> studentRepository;
        private readonly UserManager<ApplicationUser> userManager;
        private readonly SignInManager<ApplicationUser> signInManager;

        public StudentService(IDeletableEntityRepository<Student> studentRepository, UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager)
        {
            this.studentRepository = studentRepository;
            this.userManager = userManager;
            this.signInManager = signInManager;
        }

        public async Task<bool> IsStudentAsync(string userId)
            => await this.studentRepository.AllAsNoTracking().AnyAsync(x => x.UserId == userId && !x.IsDeleted);

        public async Task<int> GetTotalCountOfStudentsAsync()
            => await this.studentRepository
                .AllAsNoTracking()
                .CountAsync();

        public async Task<StudentFormModel> GetStudentByIdAsync(string id)
            => await this.studentRepository
                .All()
                .Where(x => x.Id == id && !x.IsDeleted)
                .To<StudentFormModel>()
                .FirstOrDefaultAsync();

        public async Task<StudentFormModel> GetStudentByUserIdAsync(string userId)
            => await this.studentRepository
                .All()
                .Where(x => x.UserId == userId && !x.IsDeleted)
                .To<StudentFormModel>()
                .FirstOrDefaultAsync();

        public async Task<string> GetStudentIdByUserIdAsync(string userId)
        {
            var student = await this.studentRepository
               .All()
               .FirstOrDefaultAsync(x => x.UserId == userId && !x.IsDeleted);

            return student.Id;
        }

        public async Task<Student> GetStudentDetailsByIdAsync(string id)
            => await this.studentRepository
                .All()
                .Include(s => s.Grades)
                    .ThenInclude(g => g.Subject)
                .Include(s => s.Grades)
                    .ThenInclude(g => g.Category)
                .Include(s => s.Grades)
                    .ThenInclude(g => g.Teacher)
                .FirstOrDefaultAsync(x => x.Id == id && !x.IsDeleted);

        public List<SubjectGradesViewModel> GetStudentGradesGroupBySubject(Student student)
            => student.Grades
                .GroupBy(g => g.Subject.Name)
                    .Select(g => new SubjectGradesViewModel
                    {
                        SubjectName = g.Key,
                        Grades = g
                        .OrderBy(grade => grade.Date)
                        .Select(grade => new DetailsGradeViewModel
                        {
                            Id = grade.Id,
                            Score = grade.Score,
                            Date = grade.Date.ToString(DateTimeFormat),
                            Category = grade.Category.Name,
                            Teacher = grade.Teacher.FullName,
                        })
                        .ToList(),
                    })
                .OrderBy(x => x.SubjectName)
                .ToList();

        public async Task<StudentStatisticsViewModel> GetStudentStatisticsAsync(string studentId)
        {
            var student = await this.studentRepository
                .All()
                .Include(s => s.Grades)
                .Include(s => s.Absences)
                .Include(s => s.Remarks)
                .FirstOrDefaultAsync(s => s.Id == studentId);

            if (student == null)
            {
                throw new ArgumentException("Student not found.");
            }

            return new StudentStatisticsViewModel
            {
                StudentName = student.FullName,
                AverageGPA = student.Grades.Any() ? student.Grades.Average(g => g.Score) : 0,
                TotalAbsences = student.Absences.Sum(a => a.CategoryId == 1 ? 1 : 0.5),
                PraisingRemarksCount = student.Remarks.Count(r => r.IsPraise),
                NegativeRemarksCount = student.Remarks.Count(r => !r.IsPraise),
            };
        }

        public async Task<string> SetStudentUserByFullNameAndBirthDateAsync(string userId, string fullName, DateTime birthDate)
        {
            var student = await this.studentRepository
                .All()
                .FirstOrDefaultAsync(x => x.FullName == fullName && x.BirthDate == birthDate && !x.IsDeleted);

            if (student == null)
            {
                return null;
            }

            student.UserId = userId;
            await this.studentRepository.SaveChangesAsync();

            student = await this.studentRepository
                .All()
                .Include(t => t.User)
                .FirstOrDefaultAsync(x => x.Id == student.Id);

            if (student.User == null)
            {
                return null;
            }

            await this.userManager.AddToRoleAsync(student.User, GlobalConstants.StudentRoleName);
            await this.RefreshSignInAsync(student.User);

            await this.studentRepository.SaveChangesAsync();

            return student.UserId;
        }

        public async Task AddStudentAsync(StudentFormModel formModel)
        {
            var student = AutoMapperConfig.MapperInstance.Map<Student>(formModel);

            await this.studentRepository.AddAsync(student);
            await this.studentRepository.SaveChangesAsync();
        }

        public async Task EditStudentAsync(string id, StudentFormModel formModel)
        {
            var studentById = await this.studentRepository
                .All()
                .Where(x => x.Id == id && !x.IsDeleted)
                .FirstOrDefaultAsync();

            if (studentById == null)
            {
                throw new ArgumentException($"There is no student with the id - {id}.");
            }

            studentById.FullName = formModel.FullName;
            studentById.BirthDate = formModel.BirthDate;
            studentById.ClassId = formModel.ClassId;
            studentById.SchoolId = formModel.SchoolId;

            await this.studentRepository.SaveChangesAsync();
        }

        public async Task DeleteStudentAsync(DeleteStudentViewModel model)
        {
            var student = await this.studentRepository
                .All()
                .Include(x => x.User)
                .FirstOrDefaultAsync(x => x.Id == model.Id && !x.IsDeleted);

            if (student == null)
            {
                throw new ArgumentException($"There is no such class.");
            }

            student.IsDeleted = true;

            if (student.User != null)
            {
                await this.userManager.RemoveFromRoleAsync(student.User, GlobalConstants.StudentRoleName);
            }

            await this.studentRepository.SaveChangesAsync();
        }

        private async Task RefreshSignInAsync(ApplicationUser user)
        {
            await this.signInManager.SignOutAsync();
            await this.signInManager.SignInAsync(user, isPersistent: false);
        }
    }
}
