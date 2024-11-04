namespace SchoolHub.Services
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    using Microsoft.EntityFrameworkCore;

    using SchoolHub.Data.Common.Repositories;
    using SchoolHub.Data.Models;
    using SchoolHub.Services.Mapping;
    using SchoolHub.Web.ViewModels.Teacher;

    public class ClassService : IClassService
    {
        private readonly IDeletableEntityRepository<Class> classRepository;

        public ClassService(IDeletableEntityRepository<Class> classRepository)
        {
            this.classRepository = classRepository;
        }

        public async Task<List<TeacherClassFormModel>> GetAllClassesBySchoolId(string schoolId)
            => await this.classRepository
                .All()
                .Where(x => !x.IsDeleted && x.SchoolId == schoolId)
                .To<TeacherClassFormModel>()
                .ToListAsync();
    }
}
