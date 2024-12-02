namespace SchoolHub.Services.Data.Tests
{
    using System;
    using System.Linq;
    using System.Threading.Tasks;

    using Microsoft.EntityFrameworkCore;
    using Moq;

    using SchoolHub.Data;
    using SchoolHub.Data.Common.Repositories;
    using SchoolHub.Data.Models;
    using SchoolHub.Services;
    using SchoolHub.Services.Mapping;
    using SchoolHub.Web.ViewModels.Grade;

    using Xunit;

    public class GradeServiceTests
    {
        private readonly string schoolId = Guid.NewGuid().ToString();
        private readonly string classId = Guid.NewGuid().ToString();

        private readonly string firstStudentId = Guid.NewGuid().ToString();
        private readonly string secondStudentId = Guid.NewGuid().ToString();

        private readonly string firstTeacherId = Guid.NewGuid().ToString();
        private readonly string secondTeacherId = Guid.NewGuid().ToString();

        private readonly string firstGradeId = Guid.NewGuid().ToString();
        private readonly string secondGradeId = Guid.NewGuid().ToString();
        private readonly string thirdGradeId = Guid.NewGuid().ToString();

        public GradeServiceTests()
        {
            AutoMapperConfig.RegisterMappings(
                typeof(Grade).Assembly,
                typeof(GradeFormModel).Assembly);
        }

        [Fact]
        public async Task IsGradeAppliedByTeacherAsync_ShouldReturnTrue_WhenGradeExists()
        {
            // Arrange
            var mockRepo = await this.GetMockGradeRepositoryAsync("GradeTestDb_IsGradeAppliedByTeacherAsync");
            var gradeService = new GradeService(mockRepo);

            // Act
            var result = await gradeService.IsGradeAppliedByTeacherAsync(this.firstTeacherId, this.firstGradeId);

            // Assert
            Assert.True(result);
        }

        [Fact]
        public async Task GetGradeDetailsByIdAsync_ShouldReturnCorrectDetails_WhenGradeExists()
        {
            // Arrange
            var mockRepo = await this.GetMockGradeRepositoryAsync("GradeTestDb_GetGradeDetailsByIdAsync");
            var gradeService = new GradeService(mockRepo);

            // Act
            var result = await gradeService.GetGradeDetailsByIdAsync(this.firstGradeId);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(this.firstGradeId, result.Id);
            Assert.Equal(6, result.Score);
        }

        [Fact]
        public async Task GetGradeFurtherDetailsByIdAsync_ShouldReturnCorrectDetails_WhenGradeExists()
        {
            // Arrange
            var mockRepo = await this.GetMockGradeRepositoryAsync("GradeTestDb_GetGradeFurtherDetailsByIdAsync");
            var gradeService = new GradeService(mockRepo);

            // Act
            var result = await gradeService.GetGradeFurtherDetailsByIdAsync(this.firstGradeId);

            // Assert
            Assert.NotNull(result);

            Assert.Equal(this.firstGradeId, result.Id);
            Assert.Equal(this.firstTeacherId, result.TeacherId);
            Assert.Equal(this.firstStudentId, result.StudentId);

            Assert.Equal(1, result.SubjectId);
            Assert.Equal(1, result.CategoryId);

            Assert.Equal(6, result.Score);
        }

        [Fact]
        public async Task AddGradeAsync_ShouldAddGradeSuccessfully()
        {
            // Arrange
            var mockRepo = await this.GetMockGradeRepositoryAsync("GradeTestDb_AddGradeAsync");
            var gradeService = new GradeService(mockRepo);

            var newGrade = new GradeFormModel
            {
                Score = 4,
                Date = DateTime.UtcNow,
                TeacherId = this.secondTeacherId,
                StudentId = this.secondStudentId,
                SubjectId = 2,
                CategoryId = 1,
            };

            // Act
            await gradeService.AddGradeAsync(newGrade);

            // Assert
            var grades = mockRepo.All().ToList();
            Assert.Equal(4, grades.Count);
            Assert.Contains(grades, g => g.Score == 4 && g.TeacherId == this.secondTeacherId);
        }

        [Fact]
        public async Task EditGradeAsync_ShouldEditGradeSuccessfully()
        {
            // Arrange
            var mockRepo = await this.GetMockGradeRepositoryAsync("GradeTestDb_EditGradeAsync");
            var gradeService = new GradeService(mockRepo);

            var updatedGrade = new GradeFormModel
            {
                Score = 6,
                Date = DateTime.UtcNow,
                TeacherId = this.firstTeacherId,
                StudentId = this.firstStudentId,
                SubjectId = 2,
                CategoryId = 2,
            };

            // Act
            await gradeService.EditGradeAsync(this.firstGradeId, updatedGrade);

            // Assert
            var grade = mockRepo.All().FirstOrDefault(g => g.Id == this.firstGradeId);
            Assert.NotNull(grade);
            Assert.Equal(6, grade.Score);
        }

        [Fact]
        public async Task DeleteGradeAsync_ShouldMarkGradeAsDeleted()
        {
            // Arrange
            var mockRepo = await this.GetMockGradeRepositoryAsync("GradeTestDb_DeleteGradeAsync");
            var gradeService = new GradeService(mockRepo);

            // Act
            await gradeService.DeleteGradeAsync(this.firstGradeId);

            // Assert
            var grades = await mockRepo.All().ToListAsync();
            Assert.DoesNotContain(grades, s => s.Id == this.firstGradeId);
        }

        private async Task<IDeletableEntityRepository<Grade>> GetMockGradeRepositoryAsync(string dbName)
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: dbName)
                .Options;

            var context = new ApplicationDbContext(options);

            // Seed data
            this.SeedData(context);

            await context.SaveChangesAsync();

            var mockRepo = new Mock<IDeletableEntityRepository<Grade>>();
            mockRepo.Setup(r => r.All())
                .Returns(() => context.Grades.Where(g => !g.IsDeleted).AsQueryable());

            mockRepo.Setup(r => r.AllAsNoTracking())
                .Returns(() => context.Grades.Where(g => !g.IsDeleted).AsNoTracking().AsQueryable());

            mockRepo.Setup(r => r.AddAsync(It.IsAny<Grade>()))
                .Callback((Grade grade) =>
                {
                    context.Grades.Add(grade);
                });

            mockRepo.Setup(r => r.SaveChangesAsync())
                .Callback(async () => await context.SaveChangesAsync());

            return mockRepo.Object;
        }

        private void SeedData(ApplicationDbContext context)
        {
            context.Subjects.AddRange(new[]
            {
                new Subject { Name = "Mathematics", Description = "Mathematic teaches problem-solving and analytical skills through numbers, equations, and various mathematical concepts." },
                new Subject { Name = "History", Description = "Studies past events, civilizations, and historical figures to understand human development and cultural heritage." },
            });

            context.Categories.AddRange(new[]
            {
                new Category { Name = "Exam" },
                new Category { Name = "Homework" },
            });

            context.Schools.Add(new School
            {
                Id = this.schoolId,
                Name = "Alpha School",
                Address = "Alpha St",
                WebsiteUrl = "https://alpha.edu",
            });

            context.Classes.Add(new Class
            {
                Id = this.classId,
                Name = "Math and History",
                EndingOn = DateTime.Now,
                SchoolId = this.schoolId,
            });

            context.Students.AddRange(new[]
            {
                new Student
                {
                    Id = this.firstStudentId,
                    FullName = "Alice Johnson",
                    BirthDate = new DateTime(2005, 9, 15),
                    ClassId = this.classId,
                    SchoolId = this.schoolId,
                },
                new Student
                {
                    Id = this.secondStudentId,
                    FullName = "Bob Smith",
                    BirthDate = new DateTime(2004, 6, 20),
                    ClassId = this.classId,
                    SchoolId = this.schoolId,
                },
            });

            context.Teachers.AddRange(new[]
            {
                new Teacher
                {
                    Id = this.firstTeacherId,
                    FullName = "John Doe",
                    BirthDate = new DateTime(1985, 5, 10),
                    SchoolId = this.schoolId,
                    SubjectId = 1,
                },
                new Teacher
                {
                    Id = this.secondTeacherId,
                    FullName = "Jane Smith",
                    BirthDate = new DateTime(1990, 8, 20),
                    SchoolId = this.schoolId,
                    SubjectId = 2,
                },
            });

            context.Grades.AddRange(new[]
            {
                new Grade { Id = this.firstGradeId, Score = 6, TeacherId = this.firstTeacherId, StudentId = this.firstStudentId, SubjectId = 1, CategoryId = 1 },
                new Grade { Id = this.secondGradeId, Score = 5, TeacherId = this.secondTeacherId, StudentId = this.secondStudentId, SubjectId = 2, CategoryId = 2 },
                new Grade { Id = this.thirdGradeId, Score = 6, TeacherId = this.firstTeacherId, StudentId = this.firstStudentId, SubjectId = 1, CategoryId = 1 },
            });
        }
    }
}
