namespace SchoolHub.Services
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    using Microsoft.EntityFrameworkCore;

    using SchoolHub.Data.Common.Repositories;
    using SchoolHub.Data.Models;
    using SchoolHub.Services.Mapping;
    using SchoolHub.Web.ViewModels.Class;
    using SchoolHub.Web.ViewModels.Student;
    using SchoolHub.Web.ViewModels.Teacher;

    public class ClassService : IClassService
    {
        private readonly IDeletableEntityRepository<Class> classRepository;

        public ClassService(IDeletableEntityRepository<Class> classRepository)
        {
            this.classRepository = classRepository;
        }

        public async Task<List<TeacherClassFormModel>> GetAllTeacherClassesBySchoolIdAsync(string schoolId)
            => await this.classRepository
                .All()
                .Where(x => !x.IsDeleted && x.SchoolId == schoolId && x.HomeroomTeacherId == null)
                .To<TeacherClassFormModel>()
                .ToListAsync();

        public async Task<List<TeacherClassFormModel>> GetAllTeacherClassesBySchoolIdAsync(string schoolId, string homeroomTeacherId)
            => await this.classRepository
                .All()
                .Where(x => !x.IsDeleted && x.SchoolId == schoolId && x.HomeroomTeacherId == homeroomTeacherId)
                .To<TeacherClassFormModel>()
                .ToListAsync();

        public IQueryable<Class> GetAllClassesBySchoolIdAsync(string schoolId)
            => this.classRepository
                .All()
                .Where(x => !x.IsDeleted && x.SchoolId == schoolId);

        public async Task<List<IndexStudentViewModel>> GetAllStudentsByClassIdAsync(string classId)
            => await this.classRepository
                .All()
                .Where(x => x.Id == classId && !x.IsDeleted)
                .OrderBy(x => x.Name)
                .SelectMany(x => x.Students)
                .To<IndexStudentViewModel>()
                .ToListAsync();

        public async Task<ClassFormModel> GetClassByIdAsync(string id)
            => await this.classRepository
                .All()
                .Where(x => x.Id == id && !x.IsDeleted)
                .To<ClassFormModel>()
                .FirstOrDefaultAsync();

        public async Task<MyClassViewModel> GetTeacherClassByIdAsync(string id)
            => await this.classRepository
                .All()
                .Where(x => x.Id == id && !x.IsDeleted)
                .To<MyClassViewModel>()
                .FirstOrDefaultAsync();

        public async Task<string> GetSchoolIdByClassIdAsync(string classId)
        {
            var @class = await this.classRepository
                .AllAsNoTracking()
                .FirstOrDefaultAsync(x => x.Id == classId && !x.IsDeleted);

            return @class.SchoolId;
        }

        public async Task SetHomeroomTeacherIdByClassIdAsync(string classId, string homeroomTeacherId)
        {
            var @class = await this.classRepository
                .All()
                .FirstOrDefaultAsync(x => x.Id == classId && !x.IsDeleted);

            if (@class == null)
            {
                throw new ArgumentException($"There is no class with id - {classId}.");
            }

            @class.HomeroomTeacherId = homeroomTeacherId;
            await this.classRepository.SaveChangesAsync();
        }

        public async Task<string> AddClassAsync(ClassFormModel formModel)
        {
            var @class = AutoMapperConfig.MapperInstance.Map<Class>(formModel);

            await this.classRepository.AddAsync(@class);
            await this.classRepository.SaveChangesAsync();

            return @class.Id;
        }

        public async Task EditClassByIdAsync(string id, ClassFormModel formModel)
        {
            var classById = await this.classRepository
                .All()
                .FirstOrDefaultAsync(x => x.Id == id && !x.IsDeleted);

            if (classById == null)
            {
                throw new ArgumentException($"There is no class with the id - {id}.");
            }

            classById.Name = formModel.Name;
            classById.StartedOn = formModel.StartedOn;
            classById.EndingOn = formModel.EndingOn;

            await this.classRepository.SaveChangesAsync();
        }

        public async Task DeleteClassAsync(DeleteClassViewModel model)
        {
            var @class = await this.classRepository
                .All()
                .FirstOrDefaultAsync(x => x.Id == model.Id && !x.IsDeleted);

            if (@class == null)
            {
                throw new ArgumentException($"There is no such class.");
            }

            @class.IsDeleted = true;
            await this.classRepository.SaveChangesAsync();
        }
    }
}
