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

        public StudentService(IDeletableEntityRepository<Student> studentRepository, UserManager<ApplicationUser> userManager)
        {
            this.studentRepository = studentRepository;
            this.userManager = userManager;
        }

        public async Task<bool> IsStudentAsync(string userId)
            => await this.studentRepository.AllAsNoTracking().AnyAsync(x => x.UserId == userId && !x.IsDeleted);

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

        public async Task<Student> GetStudentDetailsByIdAsync(string id)
            => await this.studentRepository
                .All()
                .Include(s => s.Grades)
                    .ThenInclude(g => g.Subject)
                .Include(s => s.Grades)
                    .ThenInclude(g => g.Category)
                .Include(s => s.Grades)
                    .ThenInclude(g => g.Teacher)
                .FirstOrDefaultAsync(x => x.Id == id);

        public List<SubjectGradesViewModel> GetStudentGradesGroupBySubjectAsync(Student student)
            => student.Grades
                .GroupBy(g => g.Subject.Name)
                    .Select(g => new SubjectGradesViewModel
                    {
                        SubjectName = g.Key,
                        Grades = g.Select(grade => new DetailsGradeViewModel
                        {
                            Score = grade.Score,
                            Date = grade.Date.ToString(DateTimeFormat),
                            Category = grade.Category.Name,
                            Teacher = grade.Teacher.FullName,
                        })
                        .ToList(),
                    })
                .ToList();

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

            student.IsDeleted = true;
            await this.userManager.RemoveFromRoleAsync(student.User, GlobalConstants.StudentRoleName);
            await this.studentRepository.SaveChangesAsync();
        }
    }
}
