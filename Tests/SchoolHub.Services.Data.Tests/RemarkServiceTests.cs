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
    using SchoolHub.Web.ViewModels.Remark;

    using Xunit;

    public class RemarkServiceTests
    {
        private readonly string schoolId = Guid.NewGuid().ToString();
        private readonly string classId = Guid.NewGuid().ToString();

        private readonly string firstStudentId = Guid.NewGuid().ToString();
        private readonly string secondStudentId = Guid.NewGuid().ToString();

        private readonly string firstTeacherId = Guid.NewGuid().ToString();
        private readonly string secondTeacherId = Guid.NewGuid().ToString();

        private readonly string firstRemarkId = Guid.NewGuid().ToString();
        private readonly string secondRemarkId = Guid.NewGuid().ToString();
        private readonly string thirdRemarkId = Guid.NewGuid().ToString();

        public RemarkServiceTests()
        {
            AutoMapperConfig.RegisterMappings(
                typeof(Absence).Assembly,
                typeof(RemarkFormModel).Assembly);
        }

        [Fact]
        public async Task GetRemarkByIdAsync_ShouldReturnCorrectRemark()
        {
            // Arrange
            var repo = await this.GetMockRemarkRepositoryAsync("TestDb_GetRemarkByIdAsync");
            var service = new RemarkService(repo);

            // Act
            var result = await service.GetRemarkByIdAsync(this.firstRemarkId);

            // Assert
            Assert.NotNull(result);
            Assert.Equal("Amazing student with excellent behavior", result.Comment);
            Assert.True(result.IsPraise);
        }

        [Fact]
        public async Task GetRemarksByStudentIdAsync_ShouldReturnRemarksWithPagination()
        {
            // Arrange
            var repo = await this.GetMockRemarkRepositoryAsync("TestDb_GetRemarksByStudentIdAsync");
            var service = new RemarkService(repo);

            // Act
            var (remarks, totalCount) = await service.GetRemarksByStudentIdAsync(this.firstStudentId, 1, 10);

            // Assert
            Assert.Single(remarks);
            Assert.Equal(1, totalCount);
            Assert.Equal("Amazing student with excellent behavior", remarks.First().Comment);
        }

        [Fact]
        public async Task GetGroupedRemarksByStudentAsync_ShouldReturnGroupedRemarks()
        {
            // Arrange
            var repo = await this.GetMockRemarkRepositoryAsync("TestDb_GetGroupedRemarksByStudentAsync");
            var service = new RemarkService(repo);

            // Act
            var result = await service.GetGroupedRemarksByStudentAsync(this.secondStudentId);

            // Assert
            Assert.NotNull(result);
            Assert.Single(result);
            Assert.Equal("Mathematics", result.First().SubjectName);
            Assert.Equal("John Mark", result.First().TeacherName);
            Assert.Equal(1, result.First().PositiveRemarksCount);
            Assert.Equal(1, result.First().NegativeRemarksCount);
        }

        [Fact]
        public async Task AddRemarkAsync_ShouldAddRemarkSuccessfully()
        {
            // Arrange
            var repo = await this.GetMockRemarkRepositoryAsync("TestDb_AddRemarkAsync");
            var service = new RemarkService(repo);

            var formModel = new RemarkFormModel
            {
                Comment = "Needs improvement in all aspects of work",
                IsPraise = false,
                StudentId = this.secondStudentId,
                TeacherId = this.firstTeacherId,
                SubjectId = 1,
                Date = DateTime.Now,
            };

            // Act
            await service.AddRemarkAsync(formModel);

            // Assert
            var allRemarks = await repo.AllAsNoTracking().ToListAsync();
            var addedRemark = repo.All().FirstOrDefault(r => r.Comment == "Needs improvement in all aspects of work");

            Assert.NotNull(addedRemark);
            Assert.False(addedRemark.IsPraise);
            Assert.Equal(4, allRemarks.Count);
        }

        [Fact]
        public async Task EditRemarkAsync_ShouldUpdateRemarkDetails()
        {
            // Arrange
            var repo = await this.GetMockRemarkRepositoryAsync("TestDb_EditRemarkAsync");
            var service = new RemarkService(repo);

            var updatedDate = DateTime.Now.AddDays(-5);
            var formModel = new RemarkFormModel
            {
                Comment = "Updated remark",
                IsPraise = false,
                StudentId = this.firstStudentId,
                TeacherId = this.firstTeacherId,
                SubjectId = 1,
                Date = updatedDate,
            };

            // Act
            await service.EditRemarkAsync(this.firstRemarkId, formModel);

            // Assert
            var updatedRemark = repo.All().First(r => r.Id == this.firstRemarkId);
            Assert.Equal("Updated remark", updatedRemark.Comment);
            Assert.False(updatedRemark.IsPraise);
            Assert.Equal(updatedDate, updatedRemark.Date);
        }

        [Fact]
        public async Task EditRemarkByIdAsync_ShouldThrowAnException()
        {
            // Arrange
            var mockRepo = await this.GetMockRemarkRepositoryAsync("TestDb_EditRemarkByIdAsync");
            var remarkService = new RemarkService(mockRepo);
            var fourthRemarkId = "4";

            // Act
            var result = remarkService.EditRemarkAsync(fourthRemarkId, null);

            // Assert
            Assert.True(result.IsFaulted);
        }

        [Fact]
        public async Task DeleteRemarkAsync_ShouldMarkRemarkAsDeleted()
        {
            // Arrange
            var repo = await this.GetMockRemarkRepositoryAsync("TestDb_DeleteRemarkAsync");
            var service = new RemarkService(repo);

            // Act
            await service.DeleteRemarkAsync(this.firstRemarkId);

            // Assert
            var deletedRemark = repo.AllAsNoTracking().FirstOrDefault(r => r.Id == this.firstRemarkId);
            Assert.Null(deletedRemark);
        }

        [Fact]
        public async Task DeleteRemarkByIdAsync_ShouldThrowAnException()
        {
            // Arrange
            var mockRepo = await this.GetMockRemarkRepositoryAsync("TestDb_DeleteRemarkByIdAsync");
            var remarkService = new RemarkService(mockRepo);
            var fourthRemarkId = "4";

            // Act
            var result = remarkService.DeleteRemarkAsync(fourthRemarkId);

            // Assert
            Assert.True(result.IsFaulted);
        }

        private async Task<IDeletableEntityRepository<Remark>> GetMockRemarkRepositoryAsync(string dbName)
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: dbName)
                .Options;

            var context = new ApplicationDbContext(options);

            // Seed data
            this.SeedData(context);

            await context.SaveChangesAsync();

            var mockRepo = new Mock<IDeletableEntityRepository<Remark>>();
            mockRepo.Setup(r => r.All())
                .Returns(() => context.Remarks.Where(g => !g.IsDeleted).AsQueryable());

            mockRepo.Setup(r => r.AllAsNoTracking())
                .Returns(() => context.Remarks.Where(g => !g.IsDeleted).AsNoTracking().AsQueryable());

            mockRepo.Setup(r => r.AddAsync(It.IsAny<Remark>()))
                .Callback((Remark remark) =>
                {
                    context.Remarks.Add(remark);
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
                    FullName = "John Mark",
                    BirthDate = new DateTime(1965, 5, 10),
                    SchoolId = this.schoolId,
                    SubjectId = 1,
                },
                new Teacher
                {
                    Id = this.secondTeacherId,
                    FullName = "Jane Taylor",
                    BirthDate = new DateTime(1980, 8, 20),
                    SchoolId = this.schoolId,
                    SubjectId = 2,
                },
            });

            context.Remarks.AddRange(new Remark[]
            {
                new Remark
                {
                    Id = this.firstRemarkId,
                    Comment = "Amazing student with excellent behavior",
                    IsPraise = true,
                    SubjectId = 1,
                    StudentId = this.firstStudentId,
                    TeacherId = this.firstTeacherId,
                    Date = new DateTime(2024, 04, 01),
                },
                new Remark
                {
                    Id = this.secondRemarkId,
                    Comment = "Awful student with bad behavior",
                    IsPraise = false,
                    SubjectId = 1,
                    StudentId = this.secondStudentId,
                    TeacherId = this.firstTeacherId,
                    Date = new DateTime(2024, 09, 04),
                },
                new Remark
                {
                    Id = this.thirdRemarkId,
                    Comment = "Active participation in class",
                    IsPraise = true,
                    SubjectId = 1,
                    StudentId = this.secondStudentId,
                    TeacherId = this.firstTeacherId,
                    Date = new DateTime(2024, 05, 22),
                },
            });
        }
    }
}
