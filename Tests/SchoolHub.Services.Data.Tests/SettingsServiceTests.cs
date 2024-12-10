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
    using SchoolHub.Data.Repositories;
    using SchoolHub.Services.Mapping;
    using SchoolHub.Web.ViewModels.Settings;
    using Xunit;

    public class SettingsServiceTests
    {
        public SettingsServiceTests()
        {
            AutoMapperConfig.RegisterMappings(typeof(SettingViewModel).Assembly);
        }

        [Fact]
        public void GetCountShouldReturnCorrectNumber()
        {
            var repository = new Mock<IDeletableEntityRepository<Setting>>();
            repository.Setup(r => r.AllAsNoTracking()).Returns(new List<Setting>
                                                        {
                                                            new Setting(),
                                                            new Setting(),
                                                            new Setting(),
                                                        }.AsQueryable());
            var service = new SettingsService(repository.Object);
            Assert.Equal(3, service.GetCount());
            repository.Verify(x => x.AllAsNoTracking(), Times.Once);
        }

        [Fact]
        public async Task GetCountShouldReturnCorrectNumberUsingDbContext()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: "SettingsTestDb").Options;
            using var dbContext = new ApplicationDbContext(options);
            dbContext.Settings.Add(new Setting());
            dbContext.Settings.Add(new Setting());
            dbContext.Settings.Add(new Setting());
            await dbContext.SaveChangesAsync();

            using var repository = new EfDeletableEntityRepository<Setting>(dbContext);
            var service = new SettingsService(repository);
            Assert.Equal(3, service.GetCount());
        }

        [Fact]
        public void GetAll_ShouldReturnAllItemsAsMappedViewModels()
        {
            // Arrange
            var mockRepository = new Mock<IDeletableEntityRepository<Setting>>();
            var settings = new List<Setting>
            {
                new Setting { Name = "Setting1", Value = "Value1" },
                new Setting { Name = "Setting2", Value = "Value2" },
            };

            mockRepository
                .Setup(r => r.All())
                .Returns(settings.AsQueryable());

            var service = new SettingsService(mockRepository.Object);

            AutoMapperConfig.RegisterMappings(typeof(SettingViewModel).Assembly);

            // Act
            var result = service.GetAll<SettingViewModel>();

            // Assert
            Assert.Equal(2, result.Count());
            Assert.Contains(result, x => x.Name == "Setting1" && x.Value == "Value1");
            Assert.Contains(result, x => x.Name == "Setting2" && x.Value == "Value2");
            mockRepository.Verify(r => r.All(), Times.Once);
        }

        [Fact]
        public async Task GetAll_ShouldReturnAllItemsUsingDbContext()
        {
            // Arrange
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: "SettingsTestDb_GetAll")
                .Options;

            using var dbContext = new ApplicationDbContext(options);
            dbContext.Settings.AddRange(
                new Setting { Name = "Setting1", Value = "Value1" },
                new Setting { Name = "Setting2", Value = "Value2" });
            await dbContext.SaveChangesAsync();

            using var repository = new EfDeletableEntityRepository<Setting>(dbContext);
            var service = new SettingsService(repository);

            // Act
            var result = service.GetAll<SettingViewModel>();

            // Assert
            Assert.Equal(2, result.Count());
            Assert.Contains(result, x => x.Name == "Setting1" && x.Value == "Value1");
            Assert.Contains(result, x => x.Name == "Setting2" && x.Value == "Value2");
        }
    }
}
