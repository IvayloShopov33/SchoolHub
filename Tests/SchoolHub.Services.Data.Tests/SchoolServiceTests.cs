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
    using SchoolHub.Web.ViewModels.School;

    using Xunit;

    public class SchoolServiceTests
    {
        private readonly string firstSchoolId = Guid.NewGuid().ToString();
        private readonly string secondSchoolId = Guid.NewGuid().ToString();

        public SchoolServiceTests()
        {
            AutoMapperConfig.RegisterMappings(
                typeof(School).Assembly,
                typeof(IndexSchoolViewModel).Assembly);
        }

        [Fact]
        public async Task AllAsync_ShouldReturnAllSchools()
        {
            // Arrange
            var mockRepo = await this.GetMockSchoolRepositoryAsync("TestDb_AllAsync");
            var service = new SchoolService(mockRepo);

            // Act
            var result = await service.AllAsync();

            // Assert
            Assert.Equal(2, result.Count);
            Assert.Contains(result, s => s.Name == "Alpha School");
            Assert.Contains(result, s => s.Name == "Beta School");
        }

        [Fact]
        public async Task SearchAsync_ShouldReturnMatchingSchools()
        {
            // Arrange
            var mockRepo = await this.GetMockSchoolRepositoryAsync("TestDb_SearchAsync");
            var service = new SchoolService(mockRepo);

            // Act
            var result = await service.SearchAsync("Alpha");

            // Assert
            Assert.Single(result);
            Assert.Contains(result, s => s.Name == "Alpha School");
        }

        [Fact]
        public async Task GetSchoolDetailsByIdAsync_ShouldReturnCorrectSchool()
        {
            // Arrange
            var mockRepo = await this.GetMockSchoolRepositoryAsync("TestDb_GetSchoolDetails");
            var service = new SchoolService(mockRepo);

            // Act
            var result = await service.GetSchoolDetailsByIdAsync(this.firstSchoolId);

            // Assert
            Assert.NotNull(result);
            Assert.Equal("Alpha School", result.Name);
        }

        [Fact]
        public async Task AddSchoolAsync_ShouldAddNewSchool()
        {
            // Arrange
            var mockRepo = await this.GetMockSchoolRepositoryAsync("TestDb_AddSchoolAsync");
            var service = new SchoolService(mockRepo);
            var newSchool = new SchoolFormModel
            {
                Name = "Gamma School",
                Address = "Gamma Ln",
                WebsiteUrl = "https://gamma.edu",
            };

            // Act
            await service.AddSchoolAsync(newSchool);
            var result = await service.AllAsync();

            // Assert
            Assert.Equal(3, result.Count);
            Assert.Contains(result, s => s.Name == "Gamma School");
        }

        [Fact]
        public async Task EditSchoolAsync_ShouldUpdateSchoolDetails()
        {
            // Arrange
            var mockRepo = await this.GetMockSchoolRepositoryAsync("TestDb_EditSchoolAsync");
            var service = new SchoolService(mockRepo);
            var updatedSchool = new SchoolFormModel
            {
                Name = "Alpha Academy",
                Address = "New Alpha St",
                WebsiteUrl = "https://newalpha.edu",
            };

            // Act
            await service.EditSchoolAsync(this.firstSchoolId, updatedSchool);
            var result = await service.GetSchoolDetailsByIdAsync(this.firstSchoolId);

            // Assert
            Assert.NotNull(result);
            Assert.Equal("Alpha Academy", result.Name);
            Assert.Equal("New Alpha St", result.Address);
            Assert.Equal("https://newalpha.edu", result.WebsiteUrl);
        }

        [Fact]
        public async Task DeleteSchoolAsync_ShouldMarkSchoolAsDeleted()
        {
            // Arrange
            var mockRepo = await this.GetMockSchoolRepositoryAsync("TestDb_DeleteSchoolAsync");
            var service = new SchoolService(mockRepo);
            var deleteModel = new DeleteSchoolViewModel { Id = this.firstSchoolId };

            // Act
            await service.DeleteSchoolAsync(deleteModel);
            var schools = await service.AllAsync();

            // Assert
            Assert.Single(schools);
            Assert.DoesNotContain(schools, s => s.Id == this.firstSchoolId);
        }

        private async Task<IDeletableEntityRepository<School>> GetMockSchoolRepositoryAsync(string dbName)
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: dbName)
                .Options;
            var context = new ApplicationDbContext(options);

            // Seed data
            context.Schools.Add(new School
            {
                Id = this.firstSchoolId,
                Name = "Alpha School",
                Address = "Alpha St",
                WebsiteUrl = "https://alpha.edu",
            });
            context.Schools.Add(new School
            {
                Id = this.secondSchoolId,
                Name = "Beta School",
                Address = "Beta Rd",
                WebsiteUrl = "https://beta.edu",
            });
            await context.SaveChangesAsync();

            var mockRepo = new Mock<IDeletableEntityRepository<School>>();
            mockRepo.Setup(r => r.All())
            .Returns(() => context.Schools.Where(s => !s.IsDeleted).AsQueryable());

            mockRepo.Setup(r => r.AllAsNoTracking())
                    .Returns(() => context.Schools.Where(s => !s.IsDeleted).AsQueryable());

            mockRepo.Setup(r => r.AddAsync(It.IsAny<School>()))
                    .Callback((School school) =>
                    {
                        context.Schools.Add(school);
                    });

            mockRepo.Setup(r => r.Delete(It.IsAny<School>()))
                    .Callback((School school) =>
                    {
                        var entity = context.Schools.FirstOrDefault(s => s.Id == school.Id);
                        if (entity != null)
                        {
                            entity.IsDeleted = true;
                            entity.DeletedOn = DateTime.UtcNow;
                        }
                    });

            mockRepo.Setup(r => r.SaveChangesAsync())
                    .Callback(async () => await context.SaveChangesAsync());

            return mockRepo.Object;
        }
    }
}
