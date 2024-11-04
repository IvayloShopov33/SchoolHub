namespace SchoolHub.Services
{
    using System.Collections.Generic;
    using System.Threading.Tasks;

    using SchoolHub.Web.ViewModels.Teacher;

    public interface ITeacherService
    {
        Task<bool> IsTeacherAsync(string userId);

        Task<List<IndexTeacherViewModel>> AllTeachersAsync(string schoolId);

        Task AddTeacherAsync(TeacherFormModel formModel);

        Task EditTeacherAsync(string id, TeacherFormModel formModel);

        Task DeleteTeacherAsync(DeleteTeacherViewModel model);

        Task<TeacherFormModel> GetTeacherByIdAsync(string id);
    }
}
