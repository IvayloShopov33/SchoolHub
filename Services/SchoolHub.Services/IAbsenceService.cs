namespace SchoolHub.Services
{
    using System.Collections.Generic;
    using System.Threading.Tasks;

    using SchoolHub.Web.ViewModels.Absence;
    using SchoolHub.Web.ViewModels.Student;

    public interface IAbsenceService
    {
        Task<AbsenceFormModel> GetAbsenceByIdAsync(string id);

        Task<(List<StudentAbsenceViewModel> Absences, int TotalCount)> GetAbsencesByStudentIdAsync(string studentId, int page, int itemsPerPage);

        Task<List<GroupedAbsencesViewModel>> GetGroupedAbsencesByStudentAsync(string studentId);

        Task AddAbsenceAsync(AbsenceFormModel formModel);

        Task EditAbsenceAsync(string id, AbsenceFormModel formModel);

        Task DeleteAbsenceAsync(string id);
    }
}
