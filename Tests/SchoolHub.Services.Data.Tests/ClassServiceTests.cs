namespace SchoolHub.Services.Data.Tests
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    using Microsoft.EntityFrameworkCore;

    using Moq;

    using SchoolHub.Data;
    using SchoolHub.Data.Common.Repositories;
    using SchoolHub.Data.Models;
    using SchoolHub.Services.Mapping;
    using SchoolHub.Web.ViewModels.Class;

    using Xunit;

    public class ClassServiceTests
    {
        private readonly string schoolId = Guid.NewGuid().ToString();
        private readonly string teacherId = Guid.NewGuid().ToString();

        private readonly string firstClassId = Guid.NewGuid().ToString();
        private readonly string secondClassId = Guid.NewGuid().ToString();

        private readonly string firstStudentId = Guid.NewGuid().ToString();
        private readonly string secondStudentId = Guid.NewGuid().ToString();

        public ClassServiceTests()
        {
            AutoMapperConfig.RegisterMappings(
                typeof(Class).Assembly,
                typeof(IndexClassViewModel).Assembly);
        }

        [Fact]
        public async Task GetAllTeacherClassesBySchoolIdWithoutSetTeacherAsync_ShouldReturnAvailableClasses()
        {
            // Arrange
            var mockRepo = await this.GetMockClassRepositoryAsync("TestDb_GetAllTeacherClassesBySchoolIdWithoutSetTeacherAsync");
            var classService = new ClassService(mockRepo);

            // Act
            var result = await classService.GetAllTeacherClassesBySchoolIdAsync(this.schoolId);

            // Assert
            Assert.NotNull(result);
            Assert.Single(result);
            Assert.Equal("Class B", result.First().Name);
        }

        [Fact]
        public async Task GetAllTeacherClassesBySchoolIdWithSetTeacherAsync_ShouldReturnAvailableClasses()
        {
            // Arrange
            var mockRepo = await this.GetMockClassRepositoryAsync("TestDb_GetAllTeacherClassesBySchoolIdWithSetTeacherAsync");
            var classService = new ClassService(mockRepo);

            // Act
            var result = await classService.GetAllTeacherClassesBySchoolIdAsync(this.schoolId, this.teacherId);

            // Assert
            Assert.NotNull(result);
            Assert.Single(result);
            Assert.Equal("Class A", result.First().Name);
        }

        [Fact]
        public async Task GetAllClassesBySchoolIdAsync_ShouldReturnAvailableClasses()
        {
            // Arrange
            var mockRepo = await this.GetMockClassRepositoryAsync("TestDb_GetAllClassesBySchoolIdAsync");
            var classService = new ClassService(mockRepo);

            // Act
            var result = await classService.GetAllClassesBySchoolIdAsync(this.schoolId).ToListAsync();

            // Assert
            Assert.NotNull(result);
            Assert.Equal(2, result.Count);
            Assert.Equal("Class A", result.First().Name);
            Assert.Equal("Class B", result.Last().Name);
        }

        [Fact]
        public async Task GetAllStudentsByClassIdAsync_ShouldReturnStudentsInClass()
        {
            // Arrange
            var mockRepo = await this.GetMockClassRepositoryAsync("TestDb_GetAllStudentsByClassIdAsync");
            var classService = new ClassService(mockRepo);

            // Act
            var result = await classService.GetAllStudentsByClassIdAsync(this.firstClassId);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(2, result.Count);
            Assert.Equal("Alice Doe", result.First().FullName);
            Assert.Equal("Bob Brown", result.Last().FullName);
        }

        [Fact]
        public async Task GetClassByIdAsync_ShouldReturnCorrectClass()
        {
            // Arrange
            var mockRepo = await this.GetMockClassRepositoryAsync("TestDb_GetClassByIdAsync");
            var classService = new ClassService(mockRepo);

            // Act
            var firstResult = await classService.GetClassByIdAsync(this.firstClassId);
            var secondResult = await classService.GetClassByIdAsync(this.secondClassId);

            // Assert
            Assert.NotNull(firstResult);
            Assert.NotNull(secondResult);
            Assert.Equal("Class A", firstResult.Name);
            Assert.Equal("Class B", secondResult.Name);
        }

        [Fact]
        public async Task GetTeacherClassByIdAsync_ShouldReturnCorrectClass()
        {
            // Arrange
            var mockRepo = await this.GetMockClassRepositoryAsync("TestDb_GetTeacherClassByIdAsync");
            var classService = new ClassService(mockRepo);

            // Act
            var result = await classService.GetTeacherClassByIdAsync(this.firstClassId);

            // Assert
            Assert.NotNull(result);
            Assert.NotNull(result.HomeroomTeacher);
            Assert.Equal("Class A", result.Name);
        }

        [Fact]
        public async Task GetSchoolIdByClassIdAsync_ShouldReturnCorrectClass()
        {
            // Arrange
            var mockRepo = await this.GetMockClassRepositoryAsync("TestDb_GetSchoolIdByClassIdAsync");
            var classService = new ClassService(mockRepo);

            // Act
            var firstResult = await classService.GetSchoolIdByClassIdAsync(this.firstClassId);
            var secondResult = await classService.GetSchoolIdByClassIdAsync(this.secondClassId);

            // Assert
            Assert.NotNull(firstResult);
            Assert.NotNull(secondResult);
            Assert.Equal(this.schoolId, firstResult);
            Assert.Equal(firstResult, secondResult);
        }

        [Fact]
        public async Task SetHomeroomTeacherIdByClassIdAsync_ShouldAssignTeacherToClass()
        {
            // Arrange
            var mockRepo = await this.GetMockClassRepositoryAsync("TestDb_SetHomeroomTeacherIdByClassIdAsync");
            var classService = new ClassService(mockRepo);

            // Act
            await classService.SetHomeroomTeacherIdByClassIdAsync(this.secondClassId, this.teacherId);

            // Assert
            var updatedClass = mockRepo.All().FirstOrDefault(x => x.Id == this.secondClassId);
            Assert.Equal(this.teacherId, updatedClass.HomeroomTeacherId);
        }

        [Fact]
        public async Task SetHomeroomTeacherIdByClassIdAsync_ShouldThrowAnException()
        {
            // Arrange
            var mockRepo = await this.GetMockClassRepositoryAsync("TestDb_SetHomeroomTeacherIdByClassIdAsyncException");
            var classService = new ClassService(mockRepo);
            var thirdClassId = "3";

            // Act
            var result = classService.SetHomeroomTeacherIdByClassIdAsync(thirdClassId, this.teacherId);

            // Assert
            Assert.True(result.IsFaulted);
        }

        [Fact]
        public async Task AddClassAsync_ShouldAddClass()
        {
            // Arrange
            var mockRepo = await this.GetMockClassRepositoryAsync("TestDb_AddClassAsync");
            var classService = new ClassService(mockRepo);

            var formModel = new ClassFormModel
            {
                Name = "Class C",
                StartedOn = DateTime.UtcNow.AddMonths(-1),
                EndingOn = DateTime.UtcNow.AddMonths(10),
                SchoolId = this.schoolId,
            };

            // Act
            var result = await classService.AddClassAsync(formModel);

            // Assert
            var classes = mockRepo.All().ToList();

            Assert.NotNull(result);
            Assert.Equal(3, classes.Count);
        }

        [Fact]
        public async Task EditClassByIdAsync_ShouldUpdateClassDetails()
        {
            // Arrange
            var mockRepo = await this.GetMockClassRepositoryAsync("TestDb_EditClassByIdAsync");
            var classService = new ClassService(mockRepo);

            var classId = this.firstClassId;
            var updatedStartedOn = DateTime.UtcNow.AddMonths(-1);
            var updatedEndingOn = DateTime.UtcNow.AddMonths(10);

            var formModel = new ClassFormModel
            {
                Name = "Updated Class A",
                StartedOn = updatedStartedOn,
                EndingOn = updatedEndingOn,
                SchoolId = this.schoolId,
            };

            // Act
            await classService.EditClassByIdAsync(classId, formModel);

            // Assert
            var updatedClass = mockRepo.All().First(x => x.Id == this.firstClassId);
            Assert.Equal(updatedStartedOn, updatedClass.StartedOn);
            Assert.Equal(updatedEndingOn, updatedClass.EndingOn);
        }

        [Fact]
        public async Task EditClassByIdAsync_ShouldThrowAnException()
        {
            // Arrange
            var mockRepo = await this.GetMockClassRepositoryAsync("TestDb_EditClassByIdAsync");
            var classService = new ClassService(mockRepo);
            var thirdClassId = "3";

            // Act
            var result = classService.EditClassByIdAsync(thirdClassId, null);

            // Assert
            Assert.True(result.IsFaulted);
        }

        [Fact]
        public async Task DeleteClassAsync_ShouldMarkClassAsDeleted()
        {
            // Arrange
            var mockRepo = await this.GetMockClassRepositoryAsync("TestDb_DeleteClassByIdAsync");
            var classService = new ClassService(mockRepo);

            var classId = this.firstClassId;
            var deleteModel = new DeleteClassViewModel { Id = classId };

            // Act
            var result = classService.DeleteClassAsync(deleteModel);

            // Assert
            var @class = mockRepo.AllAsNoTracking().FirstOrDefault(x => x.Id == this.firstClassId);
            Assert.Null(@class);
        }

        [Fact]
        public async Task DeleteClassByIdAsync_ShouldThrowAnException()
        {
            // Arrange
            var mockRepo = await this.GetMockClassRepositoryAsync("TestDb_EditClassByIdAsync");
            var classService = new ClassService(mockRepo);
            var deleteModel = new DeleteClassViewModel { Id = "3" };

            // Act
            var result = classService.DeleteClassAsync(deleteModel);

            // Assert
            Assert.True(result.IsFaulted);
        }

        private async Task<IDeletableEntityRepository<Class>> GetMockClassRepositoryAsync(string dbName)
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: dbName)
                .Options;
            var context = new ApplicationDbContext(options);

            // Seed data
            this.SeedData(context);
            await context.SaveChangesAsync();

            var mockRepo = new Mock<IDeletableEntityRepository<Class>>();
            mockRepo.Setup(r => r.All())
            .Returns(() => context.Classes.Where(s => !s.IsDeleted).AsQueryable());

            mockRepo.Setup(r => r.AllAsNoTracking())
                    .Returns(() => context.Classes.Where(s => !s.IsDeleted).AsQueryable());

            mockRepo.Setup(r => r.AddAsync(It.IsAny<Class>()))
                    .Callback((Class @class) =>
                    {
                        context.Classes.Add(@class);
                    });

            mockRepo.Setup(r => r.SaveChangesAsync())
                    .Callback(async () => await context.SaveChangesAsync());

            return mockRepo.Object;
        }

        private void SeedData(ApplicationDbContext context)
        {
            context.Subjects.Add(new Subject { Name = "Mathematics", Description = "Mathematic teaches problem-solving and analytical skills through numbers, equations, and various mathematical concepts.", });

            context.Schools.Add(new School { Id = this.schoolId, Name = "Alpha School", Address = "Alpha St", WebsiteUrl = "https://alpha.edu" });
            context.Teachers.Add(new Teacher { Id = this.teacherId, FullName = "John Smith", BirthDate = new DateTime(1976, 08, 02), ClassId = this.firstClassId, SchoolId = this.schoolId, SubjectId = 1 });

            context.Classes.AddRange(new Class[]
            {
                new Class
                {
                    Id = this.firstClassId,
                    Name = "Class A",
                    SchoolId = this.schoolId,
                    EndingOn = DateTime.Now.AddDays(20),
                    HomeroomTeacherId = this.teacherId,
                    Students = new List<Student>
                    {
                        new Student { Id = this.firstStudentId, FullName = "Alice Doe", BirthDate = new DateTime(2005, 02, 02), ClassId = this.firstClassId, SchoolId = this.schoolId },
                        new Student { Id = this.secondStudentId, FullName = "Bob Brown", BirthDate = new DateTime(2005, 05, 05), ClassId = this.firstClassId, SchoolId = this.schoolId },
                    },
                },
                new Class
                {
                    Id = this.secondClassId,
                    Name = "Class B",
                    SchoolId = this.schoolId,
                    EndingOn = DateTime.Now.AddDays(50),
                },
            });
        }
    }
}
