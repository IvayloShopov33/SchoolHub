namespace SchoolHub.Services
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    using SchoolHub.Web.ViewModels.Teacher;

    public interface ITeacherService
    {
        Task<bool> IsTeacherAsync(string userId);

        Task<TeacherFormModel> GetTeacherByIdAsync(string id);

        Task<TeacherFormModel> GetTeacherByUserIdAsync(string userId);

        Task<string> GetTeacherIdByUserIdAsync(string userId);

        Task<string> GetSchoolIdByTeacherIdAsync(string teacherId);

        Task<int> GetSubjectIdByTeacherIdAsync(string teacherId);

        Task<List<IndexTeacherViewModel>> GetAllTeachersBySchoolIdAsync(string schoolId);

        Task SetClassIdByHomeroomTeacherIdAsync(string homeroomTeacherId, string classId);

        Task<string> SetTeacherUserByFullNameAndBirthDateAsync(string userId, string fullName, DateTime birthDate);

        Task<string> AddTeacherAsync(TeacherFormModel formModel);

        Task EditTeacherAsync(string id, TeacherFormModel formModel);

        Task DeleteTeacherAsync(DeleteTeacherViewModel model);
    }
}
