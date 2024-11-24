namespace SchoolHub.Services
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    using Microsoft.EntityFrameworkCore;

    using SchoolHub.Data.Common.Repositories;
    using SchoolHub.Data.Models;
    using SchoolHub.Services.Mapping;
    using SchoolHub.Web.ViewModels.Absence;
    using SchoolHub.Web.ViewModels.Student;

    public class AbsenceService : IAbsenceService
    {
        private readonly IDeletableEntityRepository<Absence> absenceRepository;

        public AbsenceService(IDeletableEntityRepository<Absence> absenceRepository)
        {
            this.absenceRepository = absenceRepository;
        }

        public async Task<AbsenceFormModel> GetAbsenceByIdAsync(string id)
            => await this.absenceRepository
                .All()
                .Where(x => x.Id == id)
                .To<AbsenceFormModel>()
                .FirstOrDefaultAsync();

        public async Task<List<StudentAbsenceViewModel>> GetAbsencesByStudentIdAsync(string studentId)
            => await this.absenceRepository
                .All()
                .Where(a => a.StudentId == studentId)
                .OrderByDescending(x => x.Date)
                .To<StudentAbsenceViewModel>()
                .ToListAsync();

        public async Task AddAbsenceAsync(AbsenceFormModel formModel)
        {
            var absence = AutoMapperConfig.MapperInstance.Map<Absence>(formModel);

            await this.absenceRepository.AddAsync(absence);
            await this.absenceRepository.SaveChangesAsync();
        }

        public async Task EditAbsenceAsync(string id, AbsenceFormModel formModel)
        {
            var absenceById = await this.absenceRepository
                .All()
                .FirstOrDefaultAsync(x => x.Id == id && !x.IsDeleted);

            absenceById.Date = formModel.Date;
            absenceById.CategoryId = formModel.CategoryId;
            absenceById.SubjectId = formModel.SubjectId;
            absenceById.TeacherId = formModel.TeacherId;
            absenceById.StudentId = formModel.StudentId;

            await this.absenceRepository.SaveChangesAsync();
        }

        public async Task DeleteAbsenceAsync(string id)
        {
            var absenceById = await this.absenceRepository
                .All()
                .FirstOrDefaultAsync(x => x.Id == id && !x.IsDeleted);

            absenceById.IsDeleted = true;
            await this.absenceRepository.SaveChangesAsync();
        }
    }
}
