namespace SchoolHub.Services
{
    using System.Threading.Tasks;

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

        public async Task AddGradeAsync(GradeFormModel formModel)
        {
            var grade = AutoMapperConfig.MapperInstance.Map<Grade>(formModel);

            await this.gradeRepository.AddAsync(grade);
            await this.gradeRepository.SaveChangesAsync();
        }
    }
}
