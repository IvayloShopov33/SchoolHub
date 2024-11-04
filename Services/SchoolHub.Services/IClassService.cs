namespace SchoolHub.Services
{
    using System.Collections.Generic;
    using System.Threading.Tasks;

    using SchoolHub.Web.ViewModels.Teacher;

    public interface IClassService
    {
        Task<List<TeacherClassFormModel>> GetAllClassesBySchoolId(string schoolId);
    }
}
