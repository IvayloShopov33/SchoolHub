namespace SchoolHub.Services
{
    using System.Collections.Generic;
    using System.Threading.Tasks;

    using SchoolHub.Web.ViewModels.Teacher;

    public interface ISubjectService
    {
        Task<List<TeacherSubjectFormModel>> GetAllSubjects();
    }
}
