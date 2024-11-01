namespace SchoolHub.Services
{
    using System.Collections.Generic;
    using System.Threading.Tasks;

    using SchoolHub.Web.ViewModels.School;

    public interface ISchoolService
    {
        Task<List<IndexSchoolViewModel>> AllAsync();

        Task<DetailsSchoolViewModel> GetSchoolDetailsByIdAsync(string id);

        Task AddSchoolAsync(SchoolFormModel formModel);

        Task EditSchoolAsync(string id, SchoolFormModel formModel);

        Task DeleteSchoolAsync(DeleteSchoolViewModel model);
    }
}
