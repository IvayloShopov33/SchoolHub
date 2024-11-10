namespace SchoolHub.Services
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    using Microsoft.EntityFrameworkCore;

    using SchoolHub.Data.Common.Repositories;
    using SchoolHub.Data.Models;
    using SchoolHub.Services.Mapping;
    using SchoolHub.Web.ViewModels.Class;
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

        public async Task<List<IndexClassViewModel>> GetAllClassesBySchoolIdAsync(string schoolId)
            => await this.classRepository
                .All()
                .Where(x => !x.IsDeleted && x.SchoolId == schoolId)
                .To<IndexClassViewModel>()
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

        public async Task<string> GetSchoolIdByClassId(string classId)
        {
            var @class = await this.classRepository
                .AllAsNoTracking()
                .FirstOrDefaultAsync(x => x.Id == classId && !x.IsDeleted);

            return @class.SchoolId;
        }

        public async Task SetHomeroomTeacherIdByClassId(string classId, string homeroomTeacherId)
        {
            var @class = await this.classRepository
                .All()
                .FirstOrDefaultAsync(x => x.Id == classId && !x.IsDeleted);

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

            classById.Name = formModel.Name;
            classById.StartedOn = formModel.StartedOn;
            classById.EndingOn = formModel.EndingOn;

            await this.classRepository.SaveChangesAsync();
        }

        public async Task DeleteClassAsync(DeleteClassFormModel model)
        {
            var @class = await this.classRepository
                .All()
                .FirstOrDefaultAsync(x => x.Id == model.Id && !x.IsDeleted);

            @class.IsDeleted = true;
            await this.classRepository.SaveChangesAsync();
        }
    }
}
