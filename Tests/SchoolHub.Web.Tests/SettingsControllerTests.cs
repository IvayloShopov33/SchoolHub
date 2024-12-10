namespace SchoolHub.Web.Tests
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    using Microsoft.AspNetCore.Mvc;
    using Moq;

    using SchoolHub.Data.Common.Repositories;
    using SchoolHub.Data.Models;
    using SchoolHub.Services.Data;
    using SchoolHub.Web.Controllers;
    using SchoolHub.Web.ViewModels.Settings;

    using Xunit;

    public class SettingsControllerTests
    {
        [Fact]
        public void Index_ShouldReturnViewWithCorrectModel()
        {
            // Arrange
            var mockSettingsService = new Mock<ISettingsService>();
            var mockRepository = new Mock<IDeletableEntityRepository<Setting>>();

            var mockSettings = new List<SettingViewModel>
            {
                new SettingViewModel { Name = "Setting1", Value = "Value1" },
                new SettingViewModel { Name = "Setting2", Value = "Value2" },
            };

            mockSettingsService
                .Setup(s => s.GetAll<SettingViewModel>())
                .Returns(mockSettings);

            var controller = new SettingsController(mockSettingsService.Object, mockRepository.Object);

            // Act
            var result = controller.Index();

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsType<SettingsListViewModel>(viewResult.Model);
            Assert.Equal(2, model.Settings.Count());
            Assert.Equal("Setting1", model.Settings.First().Name);
            mockSettingsService.Verify(s => s.GetAll<SettingViewModel>(), Times.Once);
        }

        [Fact]
        public async Task InsertSetting_ShouldAddRandomSettingAndRedirectToIndex()
        {
            // Arrange
            var mockSettingsService = new Mock<ISettingsService>();
            var mockRepository = new Mock<IDeletableEntityRepository<Setting>>();

            var settingsList = new List<Setting>();
            mockRepository
                .Setup(r => r.AddAsync(It.IsAny<Setting>()))
                .Callback<Setting>(s => settingsList.Add(s))
                .Returns(Task.CompletedTask);

            mockRepository
                .Setup(r => r.SaveChangesAsync())
                .Returns(Task.FromResult(0));

            var controller = new SettingsController(mockSettingsService.Object, mockRepository.Object);

            // Act
            var result = await controller.InsertSetting();

            // Assert
            Assert.Single(settingsList);
            Assert.StartsWith("Name_", settingsList.First().Name);
            Assert.StartsWith("Value_", settingsList.First().Value);

            var redirectResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Index", redirectResult.ActionName);

            mockRepository.Verify(r => r.AddAsync(It.IsAny<Setting>()), Times.Once);
            mockRepository.Verify(r => r.SaveChangesAsync(), Times.Once);
        }

        [Fact]
        public void Index_ShouldReturnEmptyModelWhenNoSettingsExist()
        {
            // Arrange
            var mockSettingsService = new Mock<ISettingsService>();
            var mockRepository = new Mock<IDeletableEntityRepository<Setting>>();

            mockSettingsService
                .Setup(s => s.GetAll<SettingViewModel>())
                .Returns(new List<SettingViewModel>());

            var controller = new SettingsController(mockSettingsService.Object, mockRepository.Object);

            // Act
            var result = controller.Index();

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsType<SettingsListViewModel>(viewResult.Model);
            Assert.Empty(model.Settings);
            mockSettingsService.Verify(s => s.GetAll<SettingViewModel>(), Times.Once);
        }

        [Fact]
        public async Task InsertSetting_ShouldCallRepositoryOnce()
        {
            // Arrange
            var mockSettingsService = new Mock<ISettingsService>();
            var mockRepository = new Mock<IDeletableEntityRepository<Setting>>();

            var controller = new SettingsController(mockSettingsService.Object, mockRepository.Object);

            // Act
            await controller.InsertSetting();

            // Assert
            mockRepository.Verify(r => r.AddAsync(It.IsAny<Setting>()), Times.Once);
            mockRepository.Verify(r => r.SaveChangesAsync(), Times.Once);
        }
    }
}
