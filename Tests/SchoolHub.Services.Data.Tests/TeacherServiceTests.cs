namespace SchoolHub.Services.Data.Tests
{
    using System;
    using System.Linq;
    using System.Threading.Tasks;

    using Microsoft.AspNetCore.Authentication;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.Logging;
    using Microsoft.Extensions.Options;

    using Moq;
    using SchoolHub.Common;
    using SchoolHub.Data;
    using SchoolHub.Data.Common.Repositories;
    using SchoolHub.Data.Models;
    using SchoolHub.Services.Mapping;
    using SchoolHub.Web.ViewModels.Student;
    using SchoolHub.Web.ViewModels.Teacher;

    using Xunit;

    public class TeacherServiceTests
    {
        private readonly Mock<UserManager<ApplicationUser>> userManagerMock;
        private readonly Mock<SignInManager<ApplicationUser>> signInManagerMock;

        private readonly string schoolId = Guid.NewGuid().ToString();
        private readonly string classId = Guid.NewGuid().ToString();

        private readonly string userId = Guid.NewGuid().ToString();
        private readonly string firstTeacherId = Guid.NewGuid().ToString();
        private readonly string secondTeacherId = Guid.NewGuid().ToString();

        public TeacherServiceTests()
        {
            this.userManagerMock = this.MockUserManager();
            this.signInManagerMock = this.MockSignInManager();

            AutoMapperConfig.RegisterMappings(
                typeof(Teacher).Assembly,
                typeof(IndexTeacherViewModel).Assembly);
        }

        [Fact]
        public async Task IsTeacherAsync_ShouldReturnTrueIfUserIsStudent()
        {
            // Arrange
            var mockRepo = await this.GetMockTeacherRepositoryAsync("TestDb_IsTeacherAsync");
            var teacherService = new TeacherService(mockRepo, this.userManagerMock.Object, this.signInManagerMock.Object);

            // Act
            var result = await teacherService.IsTeacherAsync(this.userId);

            // Assert
            Assert.True(result);
        }

        [Fact]
        public async Task IsTeacherAsync_ShouldReturnFalseForInvalidUserId()
        {
            // Arrange
            var mockRepo = await this.GetMockTeacherRepositoryAsync("TestDb_IsTeacherAsyncInvalidUserId");
            var service = new TeacherService(mockRepo, this.userManagerMock.Object, this.signInManagerMock.Object);

            // Act
            var result = await service.IsTeacherAsync("invalidUserId");

            // Assert
            Assert.False(result);
        }

        [Fact]
        public async Task GetTeacherByIdAsync_ShouldReturnCorrectTeacher()
        {
            // Arrange
            var teacherRepo = await this.GetMockTeacherRepositoryAsync("TestDb_GetTeacherByIdAsync");
            var service = new TeacherService(teacherRepo, this.userManagerMock.Object, this.signInManagerMock.Object);

            // Act
            var result = await service.GetTeacherByIdAsync(this.firstTeacherId);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(this.firstTeacherId, result.Id);
            Assert.Equal("Jane Taylor", result.FullName);
        }

        [Fact]
        public async Task GetTeacherByUserIdAsync_ShouldReturnCorrectTeacher()
        {
            // Arrange
            var teacherRepo = await this.GetMockTeacherRepositoryAsync("TestDb_GetTeacherByUserIdAsync");
            var service = new TeacherService(teacherRepo, this.userManagerMock.Object, this.signInManagerMock.Object);

            // Act
            var result = await service.GetTeacherByUserIdAsync(this.userId);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(this.firstTeacherId, result.Id);
            Assert.Equal("Jane Taylor", result.FullName);
        }

        [Fact]
        public async Task GetTeacherIdByUserIdAsync_ShouldReturnCorrectTeacherId()
        {
            // Arrange
            var teacherRepo = await this.GetMockTeacherRepositoryAsync("TestDb_GetTeacherIdByUserIdAsync");
            var service = new TeacherService(teacherRepo, this.userManagerMock.Object, this.signInManagerMock.Object);

            // Act
            var result = await service.GetTeacherIdByUserIdAsync(this.userId);

            // Assert
            Assert.Equal(this.firstTeacherId, result);
        }

        [Fact]
        public async Task GetSchoolIdByTeacherIdAsync_ShouldReturnCorrectSchoolId()
        {
            // Arrange
            var teacherRepo = await this.GetMockTeacherRepositoryAsync("TestDb_GetSchoolIdByTeacherIdAsync");
            var service = new TeacherService(teacherRepo, this.userManagerMock.Object, this.signInManagerMock.Object);

            // Act
            var result = await service.GetSchoolIdByTeacherIdAsync(this.firstTeacherId);

            // Assert
            Assert.Equal(this.schoolId, result);
        }

        [Fact]
        public async Task GetSubjectIdByTeacherIdAsync_ShouldReturnCorrectSubjectId()
        {
            // Arrange
            var teacherRepo = await this.GetMockTeacherRepositoryAsync("TestDb_GetSubjectIdByTeacherIdAsync");
            var service = new TeacherService(teacherRepo, this.userManagerMock.Object, this.signInManagerMock.Object);

            // Act
            var result = await service.GetSubjectIdByTeacherIdAsync(this.secondTeacherId);

            // Assert
            Assert.Equal(2, result);
        }

        [Fact]
        public async Task GetAllTeachersBySchoolIdAsync_ShouldReturnAllTeachers()
        {
            // Arrange
            var teacherRepo = await this.GetMockTeacherRepositoryAsync("TestDb_GetAllTeachersBySchoolIdAsync");
            var service = new TeacherService(teacherRepo, this.userManagerMock.Object, this.signInManagerMock.Object);

            // Act
            var result = await service.GetAllTeachersBySchoolIdAsync(this.schoolId);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(2, result.Count);
        }

        [Fact]
        public async Task SetClassIdByHomeroomTeacherIdAsync_ShouldUpdateClassId()
        {
            // Arrange
            var teacherRepo = await this.GetMockTeacherRepositoryAsync("TestDb_SetClassIdByHomeroomTeacherIdAsync");
            var service = new TeacherService(teacherRepo, this.userManagerMock.Object, this.signInManagerMock.Object);

            // Act
            await service.SetClassIdByHomeroomTeacherIdAsync(this.firstTeacherId, this.classId);

            // Assert
            var updatedTeacher = teacherRepo.All().FirstOrDefault(x => x.Id == this.firstTeacherId);
            Assert.Equal(this.classId, updatedTeacher.ClassId);
        }

        [Fact]
        public async Task SetClassIdByHomeroomTeacherIdAsync_ShouldThrowAnException()
        {
            // Arrange
            var mockRepo = await this.GetMockTeacherRepositoryAsync("TestDb_SetClassIdByHomeroomTeacherIdAsyncException");
            var service = new TeacherService(mockRepo, this.userManagerMock.Object, this.signInManagerMock.Object);
            var thirdTeacherId = "3";

            // Act
            var result = service.SetClassIdByHomeroomTeacherIdAsync(thirdTeacherId, this.classId);

            // Assert
            Assert.True(result.IsFaulted);
        }

        [Fact]
        public async Task SetTeacherUserByFullNameAndBirthDateAsync_ShouldSetUserAndAddRole()
        {
            // Arrange
            var teacherRepo = await this.GetMockTeacherRepositoryAsync($"TestDb_SetTeacherUserByFullNameAndBirthDateAsync");
            var service = new TeacherService(teacherRepo, this.userManagerMock.Object, this.signInManagerMock.Object);

            // Act
            var result = await service.SetTeacherUserByFullNameAndBirthDateAsync(this.userId, "Malone Cole", new DateTime(1994, 07, 15));

            // Assert
            Assert.NotNull(result);
            Assert.Equal(this.userId, result);
            this.userManagerMock.Verify(u => u.AddToRoleAsync(It.IsAny<ApplicationUser>(), GlobalConstants.TeacherRoleName), Times.Once);
        }

        [Fact]
        public async Task SetTeacherUserByFullNameAndBirthDateAsync_ShouldReturnNullBecauseOfInvalidTeacher()
        {
            // Arrange
            var mockRepo = await this.GetMockTeacherRepositoryAsync("TestDb_SetTeacherUserByFullNameAndBirthDateAsyncInvalidTeacher");
            var service = new TeacherService(mockRepo, this.userManagerMock.Object, this.signInManagerMock.Object);

            // Act
            var result = await service.SetTeacherUserByFullNameAndBirthDateAsync(this.userId, "Mike Swift", new DateTime(2000, 05, 05));

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task SetTeacherUserByFullNameAndBirthDateAsync_ShouldReturnNullBecauseOfInvalidUserId()
        {
            // Arrange
            var mockRepo = await this.GetMockTeacherRepositoryAsync("TestDb_SetTeacherUserByFullNameAndBirthDateAsyncInvalidUserId");
            var service = new TeacherService(mockRepo, this.userManagerMock.Object, this.signInManagerMock.Object);
            var secondUserId = "2";

            // Act
            var result = await service.SetTeacherUserByFullNameAndBirthDateAsync(secondUserId, "Malone Cole", new DateTime(1994, 07, 15));

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task SetClassIdToNullByHomeroomTeacherIdAsync_ShouldSetClassIdToNull()
        {
            // Arrange
            var mockRepo = await this.GetMockTeacherRepositoryAsync("TestDb_SetClassIdToNullByHomeroomTeacherIdAsync");
            var teacherService = new TeacherService(mockRepo, this.userManagerMock.Object, this.signInManagerMock.Object);

            // Act
            await teacherService.SetClassIdToNullByTeacherIdAsync(this.firstTeacherId);

            // Assert
            var updatedClass = mockRepo.All().FirstOrDefault(x => x.Id == this.firstTeacherId);
            Assert.NotNull(updatedClass);
            Assert.Null(updatedClass.ClassId);
        }

        [Fact]
        public async Task SetClassIdToNullByHomeroomTeacherIdAsync_ShouldThrowArgumentExceptionWhenTeacherDoesNotExist()
        {
            // Arrange
            var mockRepo = await this.GetMockTeacherRepositoryAsync("TestDb_SetClassIdToNullByHomeroomTeacherIdAsync_Exception");
            var teacherService = new TeacherService(mockRepo, this.userManagerMock.Object, this.signInManagerMock.Object);
            var invalidTeacherId = "3";

            // Act & Assert
            var exception = await Assert.ThrowsAsync<ArgumentException>(() =>
                teacherService.SetClassIdToNullByTeacherIdAsync(invalidTeacherId));
            Assert.Equal("There is no such teacher.", exception.Message);
        }

        [Fact]
        public async Task AddTeacherAsync_ShouldAddNewTeacher()
        {
            // Arrange
            var teacherRepo = await this.GetMockTeacherRepositoryAsync("TestDb_AddTeacherAsync");
            var service = new TeacherService(teacherRepo, this.userManagerMock.Object, this.signInManagerMock.Object);
            var model = new TeacherFormModel { FullName = "New Teacher", BirthDate = DateTime.Now, SubjectId = 1, SchoolId = this.schoolId };

            // Act
            var teacherId = await service.AddTeacherAsync(model);

            // Assert
            Assert.NotNull(teacherId);
            Assert.NotNull(teacherRepo.All().FirstOrDefault(x => x.Id == teacherId));
            Assert.Equal(3, teacherRepo.All().ToList().Count);
        }

        [Fact]
        public async Task EditTeacherAsync_ShouldUpdateTeacherDetails()
        {
            // Arrange
            var mockRepo = await this.GetMockTeacherRepositoryAsync("TestDb_EditTeacherAsync");
            var service = new TeacherService(mockRepo, this.userManagerMock.Object, this.signInManagerMock.Object);

            var formModel = new TeacherFormModel { FullName = "New Name", BirthDate = new DateTime(1999, 06, 07), SchoolId = this.schoolId, ClassId = this.classId };

            // Act
            await service.EditTeacherAsync(this.firstTeacherId, formModel);

            // Assert
            var updatedTeacher = mockRepo.All().First(x => x.Id == this.firstTeacherId);
            Assert.Equal("New Name", updatedTeacher.FullName);
        }

        [Fact]
        public async Task EditTeacherAsync_ShouldThrowAnException()
        {
            // Arrange
            var mockRepo = await this.GetMockTeacherRepositoryAsync("TestDb_EditTeacherAsyncException");
            var service = new TeacherService(mockRepo, this.userManagerMock.Object, this.signInManagerMock.Object);
            var thirdTeacherId = "3";

            // Act
            var result = service.EditTeacherAsync(thirdTeacherId, null);

            // Assert
            Assert.True(result.IsFaulted);
        }

        [Fact]
        public async Task DeleteTeacherAsync_ShouldMarkTeacherAsDeletedAndRemoveRole()
        {
            // Arrange
            var mockRepo = await this.GetMockTeacherRepositoryAsync("TestDb_DeleteTeacherAsync");
            var service = new TeacherService(mockRepo, this.userManagerMock.Object, this.signInManagerMock.Object);

            // Act
            await service.DeleteTeacherAsync(new DeleteTeacherViewModel { Id = this.firstTeacherId });

            // Assert
            var teacher = mockRepo.AllAsNoTracking().FirstOrDefault(x => x.Id == this.firstTeacherId);
            Assert.Null(teacher);
            this.userManagerMock.Verify(u => u.RemoveFromRoleAsync(It.IsAny<ApplicationUser>(), GlobalConstants.TeacherRoleName), Times.Once);
        }

        [Fact]
        public async Task DeleteTeacherAsync_ShouldThrowAnException()
        {
            // Arrange
            var mockRepo = await this.GetMockTeacherRepositoryAsync("TestDb_DeleteTeacherAsyncException");
            var service = new TeacherService(mockRepo, this.userManagerMock.Object, this.signInManagerMock.Object);
            var thirdTeacherId = "3";

            // Act
            var result = service.DeleteTeacherAsync(new DeleteTeacherViewModel { Id = thirdTeacherId });

            // Assert
            Assert.True(result.IsFaulted);
            this.userManagerMock.Verify(u => u.RemoveFromRoleAsync(It.IsAny<ApplicationUser>(), GlobalConstants.TeacherRoleName), Times.Never);
        }

        private Mock<UserManager<ApplicationUser>> MockUserManager()
        {
            var store = new Mock<IUserStore<ApplicationUser>>();
            return new Mock<UserManager<ApplicationUser>>(
                store.Object, null, null, null, null, null, null, null, null);
        }

        private Mock<SignInManager<ApplicationUser>> MockSignInManager()
        {
            var userManagerMock = this.MockUserManager();

            return new Mock<SignInManager<ApplicationUser>>(
                userManagerMock.Object,
                new Mock<IHttpContextAccessor>().Object,
                new Mock<IUserClaimsPrincipalFactory<ApplicationUser>>().Object,
                new Mock<IOptions<IdentityOptions>>().Object,
                new Mock<ILogger<SignInManager<ApplicationUser>>>().Object,
                new Mock<IAuthenticationSchemeProvider>().Object);
        }

        private async Task<IDeletableEntityRepository<Teacher>> GetMockTeacherRepositoryAsync(string dbName)
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: dbName)
                .Options;

            var context = new ApplicationDbContext(options);

            // Seed data
            this.SeedData(context);

            await context.SaveChangesAsync();

            var mockRepo = new Mock<IDeletableEntityRepository<Teacher>>();
            mockRepo.Setup(r => r.All())
                .Returns(() => context.Teachers.Where(g => !g.IsDeleted).AsQueryable());

            mockRepo.Setup(r => r.AllAsNoTracking())
                .Returns(() => context.Teachers.Where(g => !g.IsDeleted).AsNoTracking().AsQueryable());

            mockRepo.Setup(r => r.AddAsync(It.IsAny<Teacher>()))
                .Callback((Teacher teacher) =>
                {
                    context.Teachers.Add(teacher);
                });

            mockRepo.Setup(r => r.SaveChangesAsync())
                .Callback(async () => await context.SaveChangesAsync());

            return mockRepo.Object;
        }

        private void SeedData(ApplicationDbContext context)
        {
            context.Subjects.AddRange(new[]
            {
                new Subject { Name = "Math", Description = "Mathematic teaches problem-solving and analytical skills through numbers, equations, and various mathematical concepts." },
                new Subject { Name = "History", Description = "Studies past events, civilizations, and historical figures to understand human development and cultural heritage." },
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

            context.Users.Add(new ApplicationUser
            {
                Id = this.userId,
                UserName = this.userId,
                Email = "user123@gmail.com",
            });

            context.Roles.Add(new ApplicationRole
            {
                Id = "1",
                Name = GlobalConstants.TeacherRoleName,
            });

            context.Teachers.AddRange(new Teacher[]
            {
                new Teacher
                {
                    Id = this.firstTeacherId,
                    FullName = "Jane Taylor",
                    BirthDate = new DateTime(1980, 08, 20),
                    SchoolId = this.schoolId,
                    UserId = this.userId,
                    SubjectId = 1,
                },
                new Teacher
                {
                    Id = this.secondTeacherId,
                    FullName = "Malone Cole",
                    BirthDate = new DateTime(1994, 07, 15),
                    SchoolId = this.schoolId,
                    SubjectId = 2,
                },
            });
        }
    }
}
