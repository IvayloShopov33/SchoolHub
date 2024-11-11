namespace SchoolHub.Services
{
    using System;
    using System.Threading.Tasks;

    using SchoolHub.Web.ViewModels.Student;

    public interface IStudentService
    {
        Task<bool> IsStudentAsync(string userId);

        Task<StudentFormModel> GetStudentByIdAsync(string id);

        Task<StudentFormModel> GetStudentByUserIdAsync(string userId);

        Task<string> SetStudentUserByFullNameAndBirthDateAsync(string userId, string fullName, DateTime birthDate);

        Task AddStudentAsync(StudentFormModel formModel);

        Task EditStudentAsync(string id, StudentFormModel formModel);

        Task DeleteStudentAsync(DeleteStudentViewModel model);
    }
}
