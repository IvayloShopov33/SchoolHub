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

    public class SubjectService : ISubjectService
    {
        private readonly IDeletableEntityRepository<Subject> subjectRepository;

        public SubjectService(IDeletableEntityRepository<Subject> subjectRepository)
        {
            this.subjectRepository = subjectRepository;
        }

        public async Task<List<TeacherSubjectFormModel>> GetAllSubjects()
            => await this.subjectRepository
                .All()
                .Where(x => !x.IsDeleted)
                .To<TeacherSubjectFormModel>()
                .ToListAsync();
    }
}
