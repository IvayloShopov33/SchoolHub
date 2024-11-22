namespace SchoolHub.Services
{
    using System.Threading.Tasks;

    using SchoolHub.Data.Common.Repositories;
    using SchoolHub.Data.Models;
    using SchoolHub.Services.Mapping;
    using SchoolHub.Web.ViewModels.Absence;

    public class AbsenceService : IAbsenceService
    {
        private readonly IDeletableEntityRepository<Absence> absenceRepository;

        public AbsenceService(IDeletableEntityRepository<Absence> absenceRepository)
        {
            this.absenceRepository = absenceRepository;
        }

        public async Task AddAbsenceAsync(AbsenceFormModel formModel)
        {
            var absence = AutoMapperConfig.MapperInstance.Map<Absence>(formModel);

            await this.absenceRepository.AddAsync(absence);
            await this.absenceRepository.SaveChangesAsync();
        }
    }
}
