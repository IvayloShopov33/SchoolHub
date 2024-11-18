namespace SchoolHub.Services
{
    using System.Collections.Generic;
    using System.Threading.Tasks;

    using SchoolHub.Web.ViewModels.Grade;

    public interface ICategoryService
    {
        Task<List<GradeCategoryFormModel>> GetGradeCategoriesAsync();
    }
}
