namespace SchoolHub.Services
{
    using System.Linq;
    using System.Threading.Tasks;

    using SchoolHub.Data.Common.Repositories;
    using SchoolHub.Data.Models;
    using SchoolHub.Services.Mapping;
    using SchoolHub.Web.ViewModels.School;

    public class SchoolService : ISchoolService
    {
        private readonly IDeletableEntityRepository<School> schoolRepository;

        public SchoolService(IDeletableEntityRepository<School> schoolRepository)
        {
            this.schoolRepository = schoolRepository;
        }

        public IQueryable All()
            => this.schoolRepository
                .All()
                .Where(x => !x.IsDeleted);

        public async Task AddSchoolAsync(SchoolFormModel formModel)
        {
            var school = AutoMapperConfig.MapperInstance.Map<School>(formModel);

            await this.schoolRepository.AddAsync(school);
            await this.schoolRepository.SaveChangesAsync();
        }
    }
}
