namespace SchoolHub.Services
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Microsoft.EntityFrameworkCore;
    using SchoolHub.Data.Common.Repositories;
    using SchoolHub.Data.Models;
    using SchoolHub.Services.Mapping;
    using SchoolHub.Web.ViewModels.Category;

    public class CategoryService : ICategoryService
    {
        private readonly IDeletableEntityRepository<Category> categoryRepository;

        public CategoryService(IDeletableEntityRepository<Category> categoryRepository)
        {
            this.categoryRepository = categoryRepository;
        }

        public async Task<List<CategoryFormModel>> GetGradeCategoriesAsync()
            => await this.categoryRepository
                .All()
                .Where(x => x.Id > 2)
                .To<CategoryFormModel>()
                .ToListAsync();

        public async Task<List<CategoryFormModel>> GetAbsenceCategoriesAsync()
            => await this.categoryRepository
                .All()
                .Where(x => x.Id <= 2)
                .To<CategoryFormModel>()
                .ToListAsync();
    }
}
