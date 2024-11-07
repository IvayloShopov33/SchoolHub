namespace SchoolHub.Services
{
    using System.Collections.Generic;
    using System.Threading.Tasks;

    using SchoolHub.Web.ViewModels.Class;
    using SchoolHub.Web.ViewModels.Teacher;

    public interface IClassService
    {
        Task<List<TeacherClassFormModel>> GetAllTeacherClassesBySchoolIdAsync(string schoolId);

        Task<List<IndexClassViewModel>> GetAllClassesBySchoolIdAsync(string schoolId);

        Task<ClassFormModel> GetClassByIdAsync(string id);

        Task AddClassAsync(ClassFormModel formModel);

        Task EditClassByIdAsync(string id, ClassFormModel formModel);

        Task DeleteClassAsync(DeleteClassFormModel model);
    }
}
