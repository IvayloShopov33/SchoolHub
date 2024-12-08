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

    using Xunit;

    public class StudentServiceTests
    {
        private readonly Mock<UserManager<ApplicationUser>> userManagerMock;
        private readonly Mock<SignInManager<ApplicationUser>> signInManagerMock;

        private readonly string schoolId = Guid.NewGuid().ToString();
        private readonly string classId = Guid.NewGuid().ToString();

        private readonly string userId = Guid.NewGuid().ToString();
        private readonly string teacherId = Guid.NewGuid().ToString();

        private readonly string firstStudentId = Guid.NewGuid().ToString();
        private readonly string secondStudentId = Guid.NewGuid().ToString();

        private readonly string firstGradeId = Guid.NewGuid().ToString();
        private readonly string secondGradeId = Guid.NewGuid().ToString();
        private readonly string thirdGradeId = Guid.NewGuid().ToString();

        private readonly string firstAbsenceId = Guid.NewGuid().ToString();
        private readonly string secondAbsenceId = Guid.NewGuid().ToString();
        private readonly string thirdAbsenceId = Guid.NewGuid().ToString();

        private readonly string firstRemarkId = Guid.NewGuid().ToString();
        private readonly string secondRemarkId = Guid.NewGuid().ToString();
        private readonly string thirdRemarkId = Guid.NewGuid().ToString();

        public StudentServiceTests()
        {
            this.userManagerMock = this.MockUserManager();
            this.signInManagerMock = this.MockSignInManager();

            AutoMapperConfig.RegisterMappings(
                typeof(Student).Assembly,
                typeof(IndexStudentViewModel).Assembly);
        }

        [Fact]
        public async Task IsStudentAsync_ShouldReturnTrueIfUserIsStudent()
        {
            // Arrange
            var mockRepo = await this.GetMockStudentRepositoryAsync("TestDb_IsStudentAsync");
            var studentService = new StudentService(mockRepo, this.userManagerMock.Object, this.signInManagerMock.Object);

            // Act
            var result = await studentService.IsStudentAsync(this.userId);

            // Assert
            Assert.True(result);
        }

        [Fact]
        public async Task IsStudentAsync_ShouldReturnFalseForInvalidUserId()
        {
            // Arrange
            var mockRepo = await this.GetMockStudentRepositoryAsync("TestDb_IsStudentAsyncInvalidUserId");
            var studentService = new StudentService(mockRepo, this.userManagerMock.Object, this.signInManagerMock.Object);

            // Act
            var result = await studentService.IsStudentAsync("invalidUserId");

            // Assert
            Assert.False(result);
        }

        [Fact]
        public async Task GetTotalCountOfStudentsAsync_ShouldReturnAvailableStudents()
        {
            // Arrange
            var mockRepo = await this.GetMockStudentRepositoryAsync("TestDb_GetTotalCountOfStudentsAsync");
            var studentService = new StudentService(mockRepo, this.userManagerMock.Object, this.signInManagerMock.Object);

            // Act
            var result = await studentService.GetTotalCountOfStudentsAsync();

            // Assert
            Assert.Equal(2, result);
        }

        [Fact]
        public async Task GetStudentByIdAsync_ShouldReturnCorrectStudent()
        {
            // Arrange
            var mockRepo = await this.GetMockStudentRepositoryAsync("TestDb_GetStudentByIdAsync");
            var studentService = new StudentService(mockRepo, this.userManagerMock.Object, this.signInManagerMock.Object);

            // Act
            var result = await studentService.GetStudentByIdAsync(this.firstStudentId);

            // Assert
            Assert.NotNull(result);
            Assert.Equal("Alice Johnson", result.FullName);
        }

        [Fact]
        public async Task GetStudentByUserIdAsync_ShouldReturnCorrectStudent()
        {
            // Arrange
            var mockRepo = await this.GetMockStudentRepositoryAsync("TestDb_GetStudentByUserIdAsync");
            var studentService = new StudentService(mockRepo, this.userManagerMock.Object, this.signInManagerMock.Object);

            // Act
            var result = await studentService.GetStudentByUserIdAsync(this.userId);

            // Assert
            Assert.NotNull(result);
            Assert.Equal("Alice Johnson", result.FullName);
        }

        [Fact]
        public async Task GetStudentIdByUserIdAsync_ShouldReturnCorrectStudentId()
        {
            // Arrange
            var mockRepo = await this.GetMockStudentRepositoryAsync("TestDb_GetStudentIdByUserIdAsync");
            var studentService = new StudentService(mockRepo, this.userManagerMock.Object, this.signInManagerMock.Object);

            // Act
            var result = await studentService.GetStudentIdByUserIdAsync(this.userId);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(this.firstStudentId, result);
        }

        [Fact]
        public async Task GetStudentDetailsByIdAsync_ShouldReturnStudentWithDetails()
        {
            // Arrange
            var mockRepo = await this.GetMockStudentRepositoryAsync("TestDb_GetStudentDetailsByIdAsync");
            var studentService = new StudentService(mockRepo, this.userManagerMock.Object, this.signInManagerMock.Object);

            // Act
            var result = await studentService.GetStudentDetailsByIdAsync(this.firstStudentId);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(this.firstStudentId, result.Id);
            Assert.Equal(2, result.Grades.Count);
            Assert.Equal(2, result.Absences.Count);
            Assert.Equal(2, result.Remarks.Count);
        }

        [Fact]
        public async Task GetStudentGradesGroupBySubject_ShouldGroupGradesBySubject()
        {
            // Arrange
            var mockRepo = await this.GetMockStudentRepositoryAsync("TestDb_GetStudentGradesGroupBySubject");
            var studentService = new StudentService(mockRepo, this.userManagerMock.Object, this.signInManagerMock.Object);

            // Act
            var student = await studentService.GetStudentDetailsByIdAsync(this.firstStudentId);
            var result = studentService.GetStudentGradesGroupBySubject(student);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(2, result.SelectMany(x => x.Grades).Count());
            Assert.Equal("History", result.First().SubjectName);
        }

        [Fact]
        public async Task GetStudentStatisticsAsync_ShouldCalculateCorrectStatistics()
        {
            // Arrange
            var mockRepo = await this.GetMockStudentRepositoryAsync("TestDb_GetStudentStatisticsAsync");
            var studentService = new StudentService(mockRepo, this.userManagerMock.Object, this.signInManagerMock.Object);

            // Act
            var firstResult = await studentService.GetStudentStatisticsAsync(this.firstStudentId);
            var secondResult = await studentService.GetStudentStatisticsAsync(this.secondStudentId);

            // Assert
            Assert.NotNull(firstResult);
            Assert.Equal(5.5, firstResult.AverageGPA);
            Assert.Equal(1.5, firstResult.TotalAbsences);
            Assert.Equal(2, firstResult.PraisingRemarksCount);
            Assert.Equal(0, firstResult.NegativeRemarksCount);

            Assert.NotNull(secondResult);
            Assert.Equal(0, secondResult.AverageGPA);
            Assert.Equal(1, secondResult.TotalAbsences);
            Assert.Equal(0, secondResult.PraisingRemarksCount);
            Assert.Equal(1, secondResult.NegativeRemarksCount);
        }

        [Fact]
        public async Task GetStudentStatisticsAsync_ShouldThrowAnException()
        {
            // Arrange
            var mockRepo = await this.GetMockStudentRepositoryAsync("TestDb_GetStudentStatisticsAsyncException");
            var studentService = new StudentService(mockRepo, this.userManagerMock.Object, this.signInManagerMock.Object);
            var thirdStudentId = "3";

            // Act
            var result = studentService.GetStudentStatisticsAsync(thirdStudentId);

            // Assert
            Assert.True(result.IsFaulted);
        }

        [Fact]
        public async Task SetStudentUserByFullNameAndBirthDateAsync_ShouldSetUserAndAddRole()
        {
            // Arrange
            var mockRepo = await this.GetMockStudentRepositoryAsync("TestDb_SetStudentUserByFullNameAndBirthDateAsync");
            var studentService = new StudentService(mockRepo, this.userManagerMock.Object, this.signInManagerMock.Object);

            // Act
            var student = await studentService.GetStudentDetailsByIdAsync(this.secondStudentId);
            var result = await studentService.SetStudentUserByFullNameAndBirthDateAsync(this.userId, student.FullName, student.BirthDate);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(this.userId, result);
            this.userManagerMock.Verify(u => u.AddToRoleAsync(student.User, GlobalConstants.StudentRoleName), Times.Once);
        }

        [Fact]
        public async Task SetStudentUserByFullNameAndBirthDateAsync_ShouldReturnNullBecauseOfInvalidStudent()
        {
            // Arrange
            var mockRepo = await this.GetMockStudentRepositoryAsync("TestDb_SetStudentUserByFullNameAndBirthDateAsyncInvalidStudent");
            var studentService = new StudentService(mockRepo, this.userManagerMock.Object, this.signInManagerMock.Object);

            // Act
            var result = await studentService.SetStudentUserByFullNameAndBirthDateAsync(this.userId, "Elton Taylor", new DateTime(2000, 05, 05));

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task SetStudentUserByFullNameAndBirthDateAsync_ShouldReturnNullBecauseOfInvalidUserId()
        {
            // Arrange
            var mockRepo = await this.GetMockStudentRepositoryAsync("TestDb_SetStudentUserByFullNameAndBirthDateAsyncInvalidUserId");
            var studentService = new StudentService(mockRepo, this.userManagerMock.Object, this.signInManagerMock.Object);
            var secondUserId = "2";

            // Act
            var student = await studentService.GetStudentDetailsByIdAsync(this.firstStudentId);
            var result = await studentService.SetStudentUserByFullNameAndBirthDateAsync(secondUserId, student.FullName, student.BirthDate);

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task AddStudentAsync_ShouldAddStudentToRepository()
        {
            // Arrange
            var mockRepo = await this.GetMockStudentRepositoryAsync("TestDb_AddStudentAsync");
            var studentService = new StudentService(mockRepo, this.userManagerMock.Object, this.signInManagerMock.Object);
            var formModel = new StudentFormModel { FullName = "New Student", BirthDate = new DateTime(1999, 06, 07), SchoolId = this.schoolId, ClassId = this.classId };

            // Act
            await studentService.AddStudentAsync(formModel);

            // Assert
            var students = mockRepo.All().ToList();
            Assert.Equal(3, students.Count);
        }

        [Fact]
        public async Task EditStudentAsync_ShouldUpdateStudentDetails()
        {
            // Arrange
            var mockRepo = await this.GetMockStudentRepositoryAsync("TestDb_EditStudentAsync");
            var studentService = new StudentService(mockRepo, this.userManagerMock.Object, this.signInManagerMock.Object);

            var formModel = new StudentFormModel { FullName = "New Name", BirthDate = new DateTime(1999, 06, 07), SchoolId = this.schoolId, ClassId = this.classId };

            // Act
            await studentService.EditStudentAsync(this.firstStudentId, formModel);

            // Assert
            var updatedStudent = mockRepo.All().First(x => x.Id == this.firstStudentId);
            Assert.Equal("New Name", updatedStudent.FullName);
        }

        [Fact]
        public async Task EditStudentAsync_ShouldThrowAnException()
        {
            // Arrange
            var mockRepo = await this.GetMockStudentRepositoryAsync("TestDb_EditStudentAsyncException");
            var studentService = new StudentService(mockRepo, this.userManagerMock.Object, this.signInManagerMock.Object);
            var thirdStudentId = "3";

            // Act
            var result = studentService.EditStudentAsync(thirdStudentId, null);

            // Assert
            Assert.True(result.IsFaulted);
        }

        [Fact]
        public async Task DeleteStudentAsync_ShouldMarkStudentAsDeletedAndRemoveRole()
        {
            // Arrange
            var mockRepo = await this.GetMockStudentRepositoryAsync("TestDb_DeleteStudentAsync");
            var studentService = new StudentService(mockRepo, this.userManagerMock.Object, this.signInManagerMock.Object);

            // Act
            await studentService.DeleteStudentAsync(new DeleteStudentViewModel { Id = this.firstStudentId });

            // Assert
            var student = mockRepo.AllAsNoTracking().FirstOrDefault(x => x.Id == this.firstStudentId);
            Assert.Null(student);
            this.userManagerMock.Verify(u => u.RemoveFromRoleAsync(It.IsAny<ApplicationUser>(), GlobalConstants.StudentRoleName), Times.Once);
        }

        [Fact]
        public async Task DeleteStudentAsync_ShouldThrowAnException()
        {
            // Arrange
            var mockRepo = await this.GetMockStudentRepositoryAsync("TestDb_DeleteStudentAsyncException");
            var studentService = new StudentService(mockRepo, this.userManagerMock.Object, this.signInManagerMock.Object);
            var thirdStudentId = "3";

            // Act
            var result = studentService.DeleteStudentAsync(new DeleteStudentViewModel { Id = thirdStudentId });

            // Assert
            Assert.True(result.IsFaulted);
            this.userManagerMock.Verify(u => u.RemoveFromRoleAsync(It.IsAny<ApplicationUser>(), GlobalConstants.StudentRoleName), Times.Never);
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

        private async Task<IDeletableEntityRepository<Student>> GetMockStudentRepositoryAsync(string dbName)
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: dbName)
                .Options;

            var context = new ApplicationDbContext(options);

            // Seed data
            this.SeedData(context);

            await context.SaveChangesAsync();

            var mockRepo = new Mock<IDeletableEntityRepository<Student>>();
            mockRepo.Setup(r => r.All())
                .Returns(() => context.Students.Where(g => !g.IsDeleted).AsQueryable());

            mockRepo.Setup(r => r.AllAsNoTracking())
                .Returns(() => context.Students.Where(g => !g.IsDeleted).AsNoTracking().AsQueryable());

            mockRepo.Setup(r => r.AddAsync(It.IsAny<Student>()))
                .Callback((Student student) =>
                {
                    context.Students.Add(student);
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

            context.Categories.AddRange(new[]
            {
                new Category { Name = "Absent" },
                new Category { Name = "Late" },
                new Category { Name = "Exam" },
                new Category { Name = "Homework" },
                new Category { Name = "Term test" },
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
                Name = GlobalConstants.StudentRoleName,
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
                    UserId = this.userId,
                },
                new Student
                {
                    Id = this.secondStudentId,
                    FullName = "Mark Ten",
                    BirthDate = new DateTime(2005, 9, 19),
                    ClassId = this.classId,
                    SchoolId = this.schoolId,
                },
            });

            context.Teachers.AddRange(new Teacher
            {
                Id = this.teacherId,
                FullName = "Jane Taylor",
                BirthDate = new DateTime(1980, 8, 20),
                SchoolId = this.schoolId,
                SubjectId = 2,
            });

            context.Grades.AddRange(new[]
            {
                new Grade { Id = this.firstGradeId, Score = 6, TeacherId = this.teacherId, StudentId = this.firstStudentId, SubjectId = 2, CategoryId = 3 },
                new Grade { Id = this.thirdGradeId, Score = 5, TeacherId = this.teacherId, StudentId = this.firstStudentId, SubjectId = 2, CategoryId = 3 },
            });

            context.Absences.AddRange(new Absence[]
            {
                new Absence
                {
                    Id = this.firstAbsenceId,
                    CategoryId = 1,
                    SubjectId = 2,
                    StudentId = this.firstStudentId,
                    TeacherId = this.teacherId,
                    Date = new DateTime(2024, 03, 04),
                },
                new Absence
                {
                    Id = this.secondAbsenceId,
                    CategoryId = 2,
                    SubjectId = 2,
                    StudentId = this.firstStudentId,
                    TeacherId = this.teacherId,
                    Date = new DateTime(2024, 05, 07),
                },
                new Absence
                {
                    Id = this.thirdAbsenceId,
                    CategoryId = 1,
                    SubjectId = 2,
                    StudentId = this.secondStudentId,
                    TeacherId = this.teacherId,
                    Date = new DateTime(2024, 01, 29),
                },
            });

            context.Remarks.AddRange(new Remark[]
            {
                new Remark
                {
                    Id = this.firstRemarkId,
                    Comment = "Amazing student with excellent behavior",
                    IsPraise = true,
                    SubjectId = 2,
                    StudentId = this.firstStudentId,
                    TeacherId = this.teacherId,
                    Date = new DateTime(2024, 04, 01),
                },
                new Remark
                {
                    Id = this.secondRemarkId,
                    Comment = "Awful student with bad behavior",
                    IsPraise = false,
                    SubjectId = 2,
                    StudentId = this.secondStudentId,
                    TeacherId = this.teacherId,
                    Date = new DateTime(2024, 09, 04),
                },
                new Remark
                {
                    Id = this.thirdRemarkId,
                    Comment = "Active participation in class",
                    IsPraise = true,
                    SubjectId = 2,
                    StudentId = this.firstStudentId,
                    TeacherId = this.teacherId,
                    Date = new DateTime(2024, 05, 22),
                },
            });
        }
    }
}
