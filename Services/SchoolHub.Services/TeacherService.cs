namespace SchoolHub.Services
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    using Microsoft.AspNetCore.Identity;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.DependencyInjection;
    using SchoolHub.Common;
    using SchoolHub.Data.Common.Repositories;
    using SchoolHub.Data.Models;
    using SchoolHub.Services.Mapping;
    using SchoolHub.Web.ViewModels.Teacher;

    public class TeacherService : ITeacherService
    {
        private readonly IDeletableEntityRepository<Teacher> teacherRepository;
        private readonly UserManager<ApplicationUser> userManager;
        private readonly SignInManager<ApplicationUser> signInManager;
        private readonly IServiceProvider serviceProvider;

        public TeacherService(IDeletableEntityRepository<Teacher> teacherRepository, UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager, IServiceProvider serviceProvider)
        {
            this.teacherRepository = teacherRepository;
            this.userManager = userManager;
            this.signInManager = signInManager;
            this.serviceProvider = serviceProvider;
        }

        public async Task<bool> IsTeacherAsync(string userId)
            => await this.teacherRepository.AllAsNoTracking().AnyAsync(x => x.UserId == userId && !x.IsDeleted);

        public async Task<TeacherFormModel> GetTeacherByIdAsync(string id)
            => await this.teacherRepository
                .All()
                .Where(x => x.Id == id && !x.IsDeleted)
                .To<TeacherFormModel>()
                .FirstOrDefaultAsync();

        public async Task<TeacherFormModel> GetTeacherByUserIdAsync(string userId)
            => await this.teacherRepository
                .All()
                .Where(x => x.UserId == userId && !x.IsDeleted)
                .To<TeacherFormModel>()
                .FirstOrDefaultAsync();

        public async Task<string> GetTeacherIdByUserIdAsync(string userId)
        {
            var teacher = await this.teacherRepository
                .All()
                .FirstOrDefaultAsync(x => x.UserId == userId && !x.IsDeleted);

            return teacher.Id;
        }

        public async Task<string> GetSchoolIdByTeacherIdAsync(string teacherId)
        {
            var teacher = await this.teacherRepository
                .AllAsNoTracking()
                .FirstOrDefaultAsync(x => x.Id == teacherId && !x.IsDeleted);

            return teacher.SchoolId;
        }

        public async Task<int> GetSubjectIdByTeacherIdAsync(string teacherId)
        {
            var teacher = await this.teacherRepository
                .AllAsNoTracking()
                .FirstOrDefaultAsync(x => (x.Id == teacherId || x.UserId == teacherId) && !x.IsDeleted);

            return teacher.SubjectId;
        }

        public async Task<List<IndexTeacherViewModel>> GetAllTeachersBySchoolIdAsync(string schoolId)
            => await this.teacherRepository
                .All()
                .Where(x => !x.IsDeleted && x.SchoolId == schoolId)
                .To<IndexTeacherViewModel>()
                .ToListAsync();

        public async Task SetClassIdByHomeroomTeacherIdAsync(string homeroomTeacherId, string classId)
        {
            var teacher = await this.teacherRepository
                .All()
                .FirstOrDefaultAsync(x => x.Id == homeroomTeacherId && !x.IsDeleted);

            if (teacher == null)
            {
                throw new ArgumentException($"There is no teacher with id - {homeroomTeacherId}.");
            }

            teacher.ClassId = classId;
            await this.teacherRepository.SaveChangesAsync();
        }

        public async Task<string> SetTeacherUserByFullNameAndBirthDateAsync(string userId, string fullName, DateTime birthDate)
        {
            var teacher = await this.teacherRepository
                .All()
                .FirstOrDefaultAsync(x => x.FullName == fullName && x.BirthDate == birthDate && !x.IsDeleted);

            if (teacher == null)
            {
                return null;
            }

            teacher.UserId = userId;
            await this.teacherRepository.SaveChangesAsync();

            teacher = await this.teacherRepository
                .All()
                .Include(t => t.User)
                .FirstOrDefaultAsync(x => x.Id == teacher.Id);

            if (teacher.User == null)
            {
                return null;
            }

            await this.userManager.AddToRoleAsync(teacher.User, GlobalConstants.TeacherRoleName);
            await this.RefreshSignInAsync(teacher.User);

            await this.teacherRepository.SaveChangesAsync();

            return teacher.UserId;
        }

        public async Task SetClassIdToNullByTeacherIdAsync(string teacherId)
        {
            var teacher = await this.teacherRepository
                .All()
                .FirstOrDefaultAsync(x => x.Id == teacherId && !x.IsDeleted);

            if (teacher == null)
            {
                throw new ArgumentException($"There is no such teacher.");
            }

            teacher.ClassId = null;
            await this.teacherRepository.SaveChangesAsync();
        }

        public async Task<string> AddTeacherAsync(TeacherFormModel formModel)
        {
            var teacher = AutoMapperConfig.MapperInstance.Map<Teacher>(formModel);

            await this.teacherRepository.AddAsync(teacher);
            await this.teacherRepository.SaveChangesAsync();

            return teacher.Id;
        }

        public async Task EditTeacherAsync(string id, TeacherFormModel formModel)
        {
            var teacherById = await this.teacherRepository
                .All()
                .FirstOrDefaultAsync(x => x.Id == id && !x.IsDeleted);

            if (teacherById == null)
            {
                throw new ArgumentException($"There is no teacher with id - {id}.");
            }

            teacherById.FullName = formModel.FullName;
            teacherById.BirthDate = formModel.BirthDate;
            teacherById.ClassId = formModel.ClassId;
            teacherById.SubjectId = formModel.SubjectId;

            await this.teacherRepository.SaveChangesAsync();
        }

        public async Task DeleteTeacherAsync(DeleteTeacherViewModel model)
        {
            var teacher = await this.teacherRepository
                .All()
                .Include(x => x.User)
                .FirstOrDefaultAsync(x => x.Id == model.Id && !x.IsDeleted);

            if (teacher == null)
            {
                throw new ArgumentException($"There is no such teacher.");
            }

            if (teacher.ClassId != null)
            {
                var classService = this.serviceProvider.GetRequiredService<IClassService>();
                await classService.SetHomeroomTeacherIdToNullByClassIdAsync(teacher.ClassId);
            }

            teacher.ClassId = null;
            teacher.IsDeleted = true;

            if (teacher.User != null)
            {
                await this.userManager.RemoveFromRoleAsync(teacher.User, GlobalConstants.TeacherRoleName);
            }

            await this.teacherRepository.SaveChangesAsync();
        }

        private async Task RefreshSignInAsync(ApplicationUser user)
        {
            await this.signInManager.SignOutAsync();
            await this.signInManager.SignInAsync(user, isPersistent: false);
        }
    }
}
