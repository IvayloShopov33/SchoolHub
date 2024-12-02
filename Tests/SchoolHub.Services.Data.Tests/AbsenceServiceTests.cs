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
    using SchoolHub.Services.Mapping;
    using SchoolHub.Web.ViewModels.Absence;

    using Xunit;

    public class AbsenceServiceTests
    {
        private readonly string schoolId = Guid.NewGuid().ToString();
        private readonly string classId = Guid.NewGuid().ToString();

        private readonly string firstStudentId = Guid.NewGuid().ToString();
        private readonly string secondStudentId = Guid.NewGuid().ToString();

        private readonly string firstTeacherId = Guid.NewGuid().ToString();
        private readonly string secondTeacherId = Guid.NewGuid().ToString();

        private readonly string firstAbsenceId = Guid.NewGuid().ToString();
        private readonly string secondAbsenceId = Guid.NewGuid().ToString();
        private readonly string thirdAbsenceId = Guid.NewGuid().ToString();

        public AbsenceServiceTests()
        {
            AutoMapperConfig.RegisterMappings(
                typeof(Absence).Assembly,
                typeof(AbsenceFormModel).Assembly);
        }

        [Fact]
        public async Task GetAbsenceByIdAsync_ShouldReturnCorrectAbsence()
        {
            // Arrange
            var repo = await this.GetMockAbsenceRepositoryAsync("TestDb_GetAbsenceByIdAsync");
            var service = new AbsenceService(repo);

            // Act
            var result = await service.GetAbsenceByIdAsync(this.firstAbsenceId);

            // Assert
            Assert.NotNull(result);

            Assert.Equal(1, result.CategoryId);
            Assert.Equal(1, result.SubjectId);

            Assert.Equal(this.firstStudentId, result.StudentId);
            Assert.Equal(this.firstTeacherId, result.TeacherId);

            Assert.Equal(new DateTime(2024, 03, 04), result.Date);
        }

        [Fact]
        public async Task GetAbsencesByStudentIdAsync_ShouldReturnAbsencesForStudent()
        {
            // Arrange
            var repo = await this.GetMockAbsenceRepositoryAsync("TestDb_GetAbsencesByStudentIdAsync");
            var service = new AbsenceService(repo);

            // Act
            var (absences, totalCount) = await service.GetAbsencesByStudentIdAsync(this.secondStudentId, 1, 10);

            // Assert
            Assert.Equal(2, totalCount);
            Assert.Equal(2, absences.Count);
        }

        [Fact]
        public async Task GetGroupedAbsencesByStudentAsync_ShouldGroupAbsencesCorrectly()
        {
            // Arrange
            var repo = await this.GetMockAbsenceRepositoryAsync("TestDb_GetGroupedAbsencesByStudentAsync");
            var service = new AbsenceService(repo);

            // Act
            var result = await service.GetGroupedAbsencesByStudentAsync(this.secondStudentId);

            // Assert
            Assert.NotNull(result);
            Assert.Single(result);
            Assert.Equal("Mathematics", result.First().SubjectName);
            Assert.Equal("John Doe", result.First().TeacherName);
            Assert.Equal(1.5, result.First().TotalAbsences);
        }

        [Fact]
        public async Task AddAbsenceAsync_ShouldAddNewAbsence()
        {
            // Arrange
            var repo = await this.GetMockAbsenceRepositoryAsync("TestDb_AddAbsenceAsync");
            var service = new AbsenceService(repo);

            var formModel = new AbsenceFormModel
            {
                StudentId = this.secondStudentId,
                TeacherId = this.firstTeacherId,
                SubjectId = 1,
                CategoryId = 2,
                Date = DateTime.Now.AddDays(-5),
            };

            // Act
            await service.AddAbsenceAsync(formModel);

            // Assert
            var absences = repo.All().ToList();
            Assert.Equal(4, absences.Count);
        }

        [Fact]
        public async Task EditAbsenceAsync_ShouldUpdateAbsenceDetails()
        {
            // Arrange
            var repo = await this.GetMockAbsenceRepositoryAsync("TestDb_EditAbsenceAsync");
            var service = new AbsenceService(repo);

            var updatedDate = DateTime.Now.AddDays(-10);
            var formModel = new AbsenceFormModel
            {
                StudentId = this.firstStudentId,
                TeacherId = this.firstTeacherId,
                SubjectId = 2,
                CategoryId = 1,
                Date = updatedDate,
            };

            // Act
            await service.EditAbsenceAsync(this.firstAbsenceId, formModel);

            // Assert
            var updatedAbsence = repo.All().First(x => x.Id == this.firstAbsenceId);
            Assert.Equal(updatedDate, updatedAbsence.Date);
        }

        [Fact]
        public async Task EditAbsenceByIdAsync_ShouldThrowAnException()
        {
            // Arrange
            var mockRepo = await this.GetMockAbsenceRepositoryAsync("TestDb_EditAbsenceByIdAsync");
            var absenceService = new AbsenceService(mockRepo);
            var fourthAbsenceId = "4";

            // Act
            var result = absenceService.EditAbsenceAsync(fourthAbsenceId, null);

            // Assert
            Assert.True(result.IsFaulted);
        }

        [Fact]
        public async Task DeleteAbsenceAsync_ShouldMarkAbsenceAsDeleted()
        {
            // Arrange
            var repo = await this.GetMockAbsenceRepositoryAsync("TestDb_DeleteAbsenceAsync");
            var service = new AbsenceService(repo);

            // Act
            await service.DeleteAbsenceAsync(this.firstAbsenceId);

            // Assert
            var absence = repo.AllAsNoTracking().FirstOrDefault(x => x.Id == this.firstAbsenceId);
            Assert.Null(absence);
        }

        [Fact]
        public async Task DeleteAbsenceByIdAsync_ShouldThrowAnException()
        {
            // Arrange
            var mockRepo = await this.GetMockAbsenceRepositoryAsync("TestDb_DeleteAbsenceByIdAsync");
            var absenceService = new AbsenceService(mockRepo);
            var fourthAbsenceId = "4";

            // Act
            var result = absenceService.DeleteAbsenceAsync(fourthAbsenceId);

            // Assert
            Assert.True(result.IsFaulted);
        }

        private async Task<IDeletableEntityRepository<Absence>> GetMockAbsenceRepositoryAsync(string dbName)
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: dbName)
                .Options;

            var context = new ApplicationDbContext(options);

            // Seed data
            this.SeedData(context);

            await context.SaveChangesAsync();

            var mockRepo = new Mock<IDeletableEntityRepository<Absence>>();
            mockRepo.Setup(r => r.All())
                .Returns(() => context.Absences.Where(g => !g.IsDeleted).AsQueryable());

            mockRepo.Setup(r => r.AllAsNoTracking())
                .Returns(() => context.Absences.Where(g => !g.IsDeleted).AsNoTracking().AsQueryable());

            mockRepo.Setup(r => r.AddAsync(It.IsAny<Absence>()))
                .Callback((Absence absence) =>
                {
                    context.Absences.Add(absence);
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
                new Category { Name = "Absent" },
                new Category { Name = "Late" },
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

            context.Absences.AddRange(new Absence[]
            {
                new Absence
                {
                    Id = this.firstAbsenceId,
                    CategoryId = 1,
                    SubjectId = 1,
                    StudentId = this.firstStudentId,
                    TeacherId = this.firstTeacherId,
                    Date = new DateTime(2024, 03, 04),
                },
                new Absence
                {
                    Id = this.secondAbsenceId,
                    CategoryId = 2,
                    SubjectId = 1,
                    StudentId = this.secondStudentId,
                    TeacherId = this.firstTeacherId,
                    Date = new DateTime(2024, 05, 07),
                },
                new Absence
                {
                    Id = this.thirdAbsenceId,
                    CategoryId = 1,
                    SubjectId = 1,
                    StudentId = this.secondStudentId,
                    TeacherId = this.firstTeacherId,
                    Date = new DateTime(2024, 01, 29),
                },
            });
        }
    }
}
