namespace SchoolHub.Web.Tests
{
    using System.Linq;

    using Microsoft.AspNetCore.Mvc;
    using Moq;

    using SchoolHub.Services.Data;
    using SchoolHub.Web.Areas.Administration.Controllers;
    using SchoolHub.Web.ViewModels.Administration.Dashboard;

    using Xunit;

    public class DashboardControllerTests
    {
        [Fact]
        public void Index_ShouldReturnViewWithCorrectViewModel()
        {
            // Arrange
            var mockSettingsService = new Mock<ISettingsService>();
            mockSettingsService.Setup(s => s.GetCount()).Returns(5);
            var controller = new DashboardController(mockSettingsService.Object);

            // Act
            var result = controller.Index();

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsType<IndexViewModel>(viewResult.Model);
            Assert.Equal(5, model.SettingsCount);
            mockSettingsService.Verify(s => s.GetCount(), Times.Once);
        }

        [Fact]
        public void Index_ShouldUseAuthorizeAttribute()
        {
            // Arrange
            var controllerType = typeof(DashboardController);

            // Act
            var authorizeAttribute = controllerType
                .GetCustomAttributes(typeof(Microsoft.AspNetCore.Authorization.AuthorizeAttribute), true)
                .FirstOrDefault();

            // Assert
            Assert.NotNull(authorizeAttribute);
            Assert.IsType<Microsoft.AspNetCore.Authorization.AuthorizeAttribute>(authorizeAttribute);
            Assert.Equal("Administrator", ((Microsoft.AspNetCore.Authorization.AuthorizeAttribute)authorizeAttribute).Roles);
        }

        [Fact]
        public void Index_ShouldHaveAreaAttribute()
        {
            // Arrange
            var controllerType = typeof(DashboardController);

            // Act
            var areaAttribute = controllerType
                .GetCustomAttributes(typeof(Microsoft.AspNetCore.Mvc.AreaAttribute), true)
                .FirstOrDefault();

            // Assert
            Assert.NotNull(areaAttribute);
            Assert.IsType<Microsoft.AspNetCore.Mvc.AreaAttribute>(areaAttribute);
            Assert.Equal("Administration", ((Microsoft.AspNetCore.Mvc.AreaAttribute)areaAttribute).RouteValue);
        }
    }
}
