namespace SchoolHub.Services
{
    using System.Threading.Tasks;

    using Microsoft.EntityFrameworkCore;

    using SchoolHub.Data.Common.Repositories;
    using SchoolHub.Data.Models;

    public class TeacherService : ITeacherService
    {
        private readonly IDeletableEntityRepository<Teacher> teacherRepository;

        public TeacherService(IDeletableEntityRepository<Teacher> teacherRepository)
        {
            this.teacherRepository = teacherRepository;
        }

        public async Task<bool> IsTeacher(string userId)
            => await this.teacherRepository.AllAsNoTracking().AnyAsync(x => x.UserId == userId);
    }
}
