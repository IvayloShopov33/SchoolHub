namespace SchoolHub.Services
{
    using System.Threading.Tasks;

    public interface ITeacherService
    {
        Task<bool> IsTeacher(string userId);
    }
}
