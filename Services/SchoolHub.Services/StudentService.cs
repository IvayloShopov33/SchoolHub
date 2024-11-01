namespace SchoolHub.Services
{
    using System.Threading.Tasks;

    using Microsoft.EntityFrameworkCore;

    using SchoolHub.Data.Common.Repositories;
    using SchoolHub.Data.Models;

    public class StudentService : IStudentService
    {
        private readonly IDeletableEntityRepository<Student> studentRepository;

        public StudentService(IDeletableEntityRepository<Student> studentRepository)
        {
            this.studentRepository = studentRepository;
        }

        public async Task<bool> IsStudent(string userId)
            => await this.studentRepository.AllAsNoTracking().AnyAsync(x => x.UserId == userId);
    }
}
