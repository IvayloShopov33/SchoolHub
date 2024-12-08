namespace SchoolHub.Services
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    using SchoolHub.Data.Models;
    using SchoolHub.Web.ViewModels.Student;
    using SchoolHub.Web.ViewModels.Subject;

    public interface IStudentService
    {
        Task<bool> IsStudentAsync(string userId);

        Task<int> GetTotalCountOfStudentsAsync();

        Task<StudentFormModel> GetStudentByIdAsync(string id);

        Task<StudentFormModel> GetStudentByUserIdAsync(string userId);

        Task<string> GetStudentIdByUserIdAsync(string userId);

        Task<Student> GetStudentDetailsByIdAsync(string id);

        List<SubjectGradesViewModel> GetStudentGradesGroupBySubject(Student student);

        Task<StudentStatisticsViewModel> GetStudentStatisticsAsync(string studentId);

        Task<string> SetStudentUserByFullNameAndBirthDateAsync(string userId, string fullName, DateTime birthDate);

        Task AddStudentAsync(StudentFormModel formModel);

        Task EditStudentAsync(string id, StudentFormModel formModel);

        Task DeleteStudentAsync(DeleteStudentViewModel model);
    }
}
