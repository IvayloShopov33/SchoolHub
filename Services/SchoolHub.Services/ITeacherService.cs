namespace SchoolHub.Services
{
    using System.Collections.Generic;
    using System.Threading.Tasks;

    using SchoolHub.Web.ViewModels.Teacher;

    public interface ITeacherService
    {
        Task<bool> IsTeacherAsync(string userId);

        Task<string> GetSchoolIdByTeacherId(string teacherId);

        Task<List<IndexTeacherViewModel>> AllTeachersAsync(string schoolId);

        Task<string> AddTeacherAsync(TeacherFormModel formModel);

        Task EditTeacherAsync(string id, TeacherFormModel formModel);

        Task DeleteTeacherAsync(DeleteTeacherViewModel model);

        Task<TeacherFormModel> GetTeacherByIdAsync(string id);
    }
}
