namespace SchoolHub.Services
{
    using System.Collections.Generic;
    using System.Threading.Tasks;

    using SchoolHub.Web.ViewModels.Category;

    public interface ICategoryService
    {
        Task<List<CategoryFormModel>> GetGradeCategoriesAsync();

        Task<List<CategoryFormModel>> GetAbsenceCategoriesAsync();
    }
}
