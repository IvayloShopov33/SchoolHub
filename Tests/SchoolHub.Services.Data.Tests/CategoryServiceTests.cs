namespace SchoolHub.Services.Data.Tests
{
    using System.Linq;
    using System.Threading.Tasks;

    using Microsoft.EntityFrameworkCore;
    using Moq;

    using SchoolHub.Data;
    using SchoolHub.Data.Common.Repositories;
    using SchoolHub.Data.Models;
    using SchoolHub.Services.Mapping;
    using SchoolHub.Web.ViewModels.Category;
    using Xunit;

    public class CategoryServiceTests
    {
        public CategoryServiceTests()
        {
            AutoMapperConfig.RegisterMappings(
                typeof(Category).Assembly,
                typeof(CategoryFormModel).Assembly);
        }

        [Fact]
        public async Task GetGradeCategoriesAsync_ShouldReturnCategoriesWithIdGreaterThanTwo()
        {
            // Arrange
            var mockRepo = await this.GetMockCategoryRepositoryAsync("TestDb_GetGradeCategories");
            var categoryService = new CategoryService(mockRepo);

            // Act
            var result = await categoryService.GetGradeCategoriesAsync();

            // Assert
            Assert.NotNull(result);
            Assert.Equal(3, result.Count);
            Assert.Contains(result, c => c.Name == "Exam");
            Assert.Contains(result, c => c.Name == "Homework");
            Assert.Contains(result, c => c.Name == "Term test");
        }

        [Fact]
        public async Task GetAbsenceCategoriesAsync_ShouldReturnCategoriesWithIdLessThanOrEqualToTwo()
        {
            // Arrange
            var mockRepo = await this.GetMockCategoryRepositoryAsync("TestDb_GetAbsenceCategories");
            var categoryService = new CategoryService(mockRepo);

            // Act
            var result = await categoryService.GetAbsenceCategoriesAsync();

            // Assert
            Assert.NotNull(result);
            Assert.Equal(2, result.Count);
            Assert.Contains(result, c => c.Name == "Absent");
            Assert.Contains(result, c => c.Name == "Late");
        }

        private async Task<IDeletableEntityRepository<Category>> GetMockCategoryRepositoryAsync(string dbName)
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: dbName)
                .Options;
            var context = new ApplicationDbContext(options);

            // Seed data
            context.Categories.AddRange(new Category[]
            {
                new Category { Name = "Absent" },
                new Category { Name = "Late" },
                new Category { Name = "Exam" },
                new Category { Name = "Homework" },
                new Category { Name = "Term test" },
            });
            await context.SaveChangesAsync();

            var mockRepo = new Mock<IDeletableEntityRepository<Category>>();
            mockRepo.Setup(r => r.All())
                .Returns(() => context.Categories.Where(s => !s.IsDeleted).AsQueryable());

            return mockRepo.Object;
        }
    }
}
