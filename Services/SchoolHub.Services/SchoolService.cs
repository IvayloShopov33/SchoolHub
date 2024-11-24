namespace SchoolHub.Services
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    using Microsoft.EntityFrameworkCore;

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

        public async Task<List<IndexSchoolViewModel>> AllAsync()
            => await this.schoolRepository
                .AllAsNoTracking()
                .Where(x => !x.IsDeleted)
                .OrderBy(x => x.Name)
                .To<IndexSchoolViewModel>()
                .ToListAsync();

        public async Task<List<IndexSchoolViewModel>> SearchAsync(string searchQuery)
            => await this.schoolRepository
                .AllAsNoTracking()
                .Where(s => s.Name.ToLower().Contains(searchQuery.ToLower()) || s.WebsiteUrl.ToLower().Contains(searchQuery.ToLower()))
                .OrderBy(x => x.Name)
                .To<IndexSchoolViewModel>()
                .ToListAsync();

        public async Task<DetailsSchoolViewModel> GetSchoolDetailsByIdAsync(string id)
            => await this.schoolRepository
            .AllAsNoTracking()
            .To<DetailsSchoolViewModel>()
            .FirstOrDefaultAsync(x => x.Id == id);

        public async Task AddSchoolAsync(SchoolFormModel formModel)
        {
            var school = AutoMapperConfig.MapperInstance.Map<School>(formModel);

            await this.schoolRepository.AddAsync(school);
            await this.schoolRepository.SaveChangesAsync();
        }

        public async Task EditSchoolAsync(string id, SchoolFormModel formModel)
        {
            var schoolById = await this.schoolRepository
                .All()
                .FirstOrDefaultAsync(x => x.Id == id && !x.IsDeleted);

            schoolById.Name = formModel.Name;
            schoolById.Address = formModel.Address;
            schoolById.WebsiteUrl = formModel.WebsiteUrl;

            await this.schoolRepository.SaveChangesAsync();
        }

        public async Task DeleteSchoolAsync(DeleteSchoolViewModel model)
        {
            var schoolById = await this.schoolRepository
                .All()
                .FirstOrDefaultAsync(x => x.Id == model.Id && !x.IsDeleted);

            schoolById.IsDeleted = true;
            await this.schoolRepository.SaveChangesAsync();
        }
    }
}
