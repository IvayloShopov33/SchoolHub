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

        public async Task<(List<StudentAbsenceViewModel> Absences, int TotalCount)> GetAbsencesByStudentIdAsync(string studentId, int page, int itemsPerPage)
        {
            var absences = await this.absenceRepository
               .AllAsNoTracking()
               .Where(a => a.StudentId == studentId)
               .OrderByDescending(x => x.Date)
               .Skip((page - 1) * itemsPerPage)
               .Take(itemsPerPage)
               .To<StudentAbsenceViewModel>()
               .ToListAsync();

            var totalCount = await this.absenceRepository
                .AllAsNoTracking()
                .CountAsync(a => a.StudentId == studentId);

            return (absences, totalCount);
        }

        public async Task<List<GroupedAbsencesViewModel>> GetGroupedAbsencesByStudentAsync(string studentId)
            => await this.absenceRepository
                .AllAsNoTracking()
                .Where(a => a.StudentId == studentId)
                .GroupBy(a => new { a.Subject.Name, TeacherName = a.Teacher.FullName })
                .Select(g => new GroupedAbsencesViewModel
                {
                    SubjectName = g.Key.Name,
                    TeacherName = g.Key.TeacherName,
                    TotalAbsences = g.Sum(a => a.Category.Name == "Absent" ? 1 : 0.5),
                })
                .OrderByDescending(x => x.TotalAbsences)
                .ThenBy(x => x.SubjectName)
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

            if (absenceById == null)
            {
                throw new ArgumentException($"There is no absence with the id - {id}.");
            }

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

            if (absenceById == null)
            {
                throw new ArgumentException($"There is no absence with the id - {id}.");
            }

            absenceById.IsDeleted = true;
            await this.absenceRepository.SaveChangesAsync();
        }
    }
}
