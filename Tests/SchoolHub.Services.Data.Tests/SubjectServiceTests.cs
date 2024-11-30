namespace SchoolHub.Services.Data.Tests
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    using Microsoft.EntityFrameworkCore;
    using Moq;

    using SchoolHub.Data;
    using SchoolHub.Data.Common.Repositories;
    using SchoolHub.Data.Models;
    using SchoolHub.Services.Mapping;
    using SchoolHub.Web.ViewModels.Teacher;

    using Xunit;

    public class SubjectServiceTests
    {
        public SubjectServiceTests()
        {
            AutoMapperConfig.RegisterMappings(
                typeof(Subject).Assembly,
                typeof(TeacherSubjectFormModel).Assembly);
        }

        [Fact]
        public async Task GetAllSubjectsAsync_ShouldReturnAllNonDeletedSubjects()
        {
            // Arrange
            var mockRepo = await this.GetMockSubjectRepositoryAsync("TestDb_GetAllSubjectsAsync");
            var subjectService = new SubjectService(mockRepo);

            // Act
            var result = await subjectService.GetAllSubjectsAsync();

            // Assert
            Assert.NotNull(result);
            Assert.Equal(3, result.Count);
            Assert.Contains(result, s => s.Name == "Mathematics");
            Assert.Contains(result, s => s.Name == "History");
            Assert.Contains(result, s => s.Name == "Physics");
        }

        private async Task<IDeletableEntityRepository<Subject>> GetMockSubjectRepositoryAsync(string dbName)
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: dbName)
                .Options;
            var context = new ApplicationDbContext(options);

            // Seed data
            context.Subjects.AddRange(new List<Subject>
            {
                new Subject { Name = "Mathematics", Description = "Mathematic teaches problem-solving and analytical skills through numbers, equations, and various mathematical concepts.", },
                new Subject { Name = "History", Description = "Studies past events, civilizations, and historical figures to understand human development and cultural heritage." },
                new Subject { Name = "Physics", Description = "Explores the fundamental principles of the universe, such as motion, energy, and forces." },
                new Subject { Name = "Art", Description = "Involves creative expression through drawing, painting, sculpture, and other visual media.", IsDeleted = true },
            });
            await context.SaveChangesAsync();

            var mockRepo = new Mock<IDeletableEntityRepository<Subject>>();
            mockRepo.Setup(r => r.All())
                .Returns(() => context.Subjects.Where(s => !s.IsDeleted).AsQueryable());

            return mockRepo.Object;
        }
    }
}
