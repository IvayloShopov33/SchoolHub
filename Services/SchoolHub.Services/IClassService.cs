namespace SchoolHub.Services
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    using SchoolHub.Data.Models;
    using SchoolHub.Web.ViewModels.Class;
    using SchoolHub.Web.ViewModels.Student;
    using SchoolHub.Web.ViewModels.Teacher;

    public interface IClassService
    {
        Task<List<TeacherClassFormModel>> GetAllTeacherClassesBySchoolIdAsync(string schoolId);

        Task<List<TeacherClassFormModel>> GetAllTeacherClassesBySchoolIdAsync(string schoolId, string homeroomTeacherId);

        IQueryable<Class> GetAllClassesBySchoolIdAsync(string schoolId);

        Task<List<IndexStudentViewModel>> GetAllStudentsByClassIdAsync(string classId);

        Task<ClassFormModel> GetClassByIdAsync(string id);

        Task<MyClassViewModel> GetTeacherClassByIdAsync(string id);

        Task<string> GetSchoolIdByClassIdAsync(string classId);

        Task SetHomeroomTeacherIdByClassIdAsync(string classId, string homeroomTeacherId);

        Task SetHomeroomTeacherIdToNullByClassIdAsync(string classId);

        Task<string> AddClassAsync(ClassFormModel formModel);

        Task EditClassByIdAsync(string id, ClassFormModel formModel);

        Task DeleteClassAsync(DeleteClassViewModel model);
    }
}
