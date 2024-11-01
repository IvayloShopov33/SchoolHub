namespace SchoolHub.Services
{
    using System.Threading.Tasks;

    public interface IStudentService
    {
        Task<bool> IsStudent(string userId);
    }
}
