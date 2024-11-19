namespace SchoolHub.Services
{
    using System.Threading.Tasks;

    using Microsoft.EntityFrameworkCore;

    using SchoolHub.Data.Common.Repositories;
    using SchoolHub.Data.Models;
    using SchoolHub.Services.Mapping;
    using SchoolHub.Web.ViewModels.Grade;

    public class GradeService : IGradeService
    {
        private readonly IDeletableEntityRepository<Grade> gradeRepository;

        public GradeService(IDeletableEntityRepository<Grade> gradeRepository)
        {
            this.gradeRepository = gradeRepository;
        }

        public async Task<DetailsGradeViewModel> GetGradeDetailsByIdAsync(string id)
            => await this.gradeRepository
                .AllAsNoTracking()
                .To<DetailsGradeViewModel>()
                .FirstOrDefaultAsync(x => x.Id == id);

        public async Task<Grade> GetGradeFurtherDetailsByIdAsync(string id)
            => await this.gradeRepository
                .AllAsNoTracking()
                .FirstOrDefaultAsync(x => x.Id == id);

        public async Task AddGradeAsync(GradeFormModel formModel)
        {
            var grade = AutoMapperConfig.MapperInstance.Map<Grade>(formModel);

            await this.gradeRepository.AddAsync(grade);
            await this.gradeRepository.SaveChangesAsync();
        }

        public async Task EditGradeAsync(string id, GradeFormModel formModel)
        {
            var gradeById = await this.gradeRepository
                .All()
                .FirstOrDefaultAsync(x => x.Id == id && !x.IsDeleted);

            gradeById.Score = formModel.Score;
            gradeById.Date = formModel.Date;
            gradeById.CategoryId = formModel.CategoryId;
            gradeById.SubjectId = formModel.SubjectId;
            gradeById.TeacherId = formModel.TeacherId;
            gradeById.StudentId = formModel.StudentId;

            await this.gradeRepository.SaveChangesAsync();
        }

        public async Task DeleteGradeAsync(string id)
        {
            var gradeById = await this.gradeRepository
                .All()
                .FirstOrDefaultAsync(x => x.Id == id && !x.IsDeleted);

            gradeById.IsDeleted = true;
            await this.gradeRepository.SaveChangesAsync();
        }
    }
}
