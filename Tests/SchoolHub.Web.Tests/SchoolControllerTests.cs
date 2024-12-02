namespace SchoolHub.Web.Tests
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    using Microsoft.AspNetCore.Mvc;
    using Moq;
    using SchoolHub.Data.Models;
    using SchoolHub.Services;
    using SchoolHub.Services.Mapping;
    using SchoolHub.Web.Controllers;
    using SchoolHub.Web.ViewModels.School;

    using Xunit;

    public class SchoolControllerTests
    {
        private readonly Mock<ISchoolService> mockSchoolService;
        private readonly SchoolController controller;

        private readonly string firstSchoolId = Guid.NewGuid().ToString();
        private readonly string secondSchoolId = Guid.NewGuid().ToString();

        public SchoolControllerTests()
        {
            this.mockSchoolService = new Mock<ISchoolService>();
            this.controller = new SchoolController(this.mockSchoolService.Object);

            AutoMapperConfig.RegisterMappings(
                typeof(School).Assembly,
                typeof(IndexSchoolViewModel).Assembly);
        }

        [Fact]
        public async Task Index_ShouldReturnPaginatedSchools()
        {
            // Arrange
            var mockService = new Mock<ISchoolService>();
            mockService.Setup(s => s.AllAsync()).ReturnsAsync(this.GetTestSchools());
            var controller = new SchoolController(mockService.Object);

            // Act
            var result = await controller.Index(null, 1, 10);

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsAssignableFrom<PaginatedIndexSchoolViewModel>(viewResult.Model);

            Assert.Equal(2, model.Schools.Count());
            Assert.Contains(model.Schools, s => s.Name == "Alpha School");
        }

        [Fact]
        public async Task Index_ShouldReturnFilteredSchools_WhenSearchQueryIsNotNull()
        {
            // Arrange
            var searchQuery = "Alpha";
            this.mockSchoolService
                .Setup(s => s.SearchAsync(searchQuery))
                .ReturnsAsync(this.GetTestSchools().Where(s => s.Name.Contains(searchQuery)).ToList());

            var controller = new SchoolController(this.mockSchoolService.Object);

            // Act
            var result = await controller.Index(searchQuery, 1, 10);

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsAssignableFrom<PaginatedIndexSchoolViewModel>(viewResult.Model);

            Assert.Single(model.Schools);
            Assert.Equal("Alpha School", model.Schools.First().Name);

            Assert.Equal(1, model.CurrentPage);
            Assert.Equal(10, model.PageSize);
            Assert.Equal(searchQuery, model.SearchQuery);
            Assert.Equal(1, model.TotalCount);
        }

        [Fact]
        public async Task Details_ShouldReturnCorrectSchool()
        {
            // Arrange
            var mockService = new Mock<ISchoolService>();
            var schoolDetails = new DetailsSchoolViewModel
            {
                Id = this.firstSchoolId,
                Name = "Alpha School",
                WebsiteUrl = "https://alpha.edu",
            };

            mockService.Setup(s => s.GetSchoolDetailsByIdAsync(this.firstSchoolId)).ReturnsAsync(schoolDetails);
            var controller = new SchoolController(mockService.Object);

            // Act
            var result = await controller.Details(this.firstSchoolId);

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsType<DetailsSchoolViewModel>(viewResult.Model);

            Assert.Equal("Alpha School", model.Name);
        }

        [Fact]
        public async Task Add_ShouldRedirectToIndex_WhenModelIsValid()
        {
            // Arrange
            var mockService = new Mock<ISchoolService>();
            var controller = new SchoolController(mockService.Object);

            var formModel = new SchoolFormModel
            {
                Name = "Gamma School",
                Address = "Gamma Ln",
                WebsiteUrl = "https://gamma.edu",
            };

            // Act
            var result = await controller.Add(formModel);

            // Assert
            var redirectResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Index", redirectResult.ActionName);
        }

        [Fact]
        public void Add_Get_ReturnsView()
        {
            // Act
            var result = this.controller.Add() as ViewResult;

            // Assert
            Assert.NotNull(result);
        }

        [Fact]
        public async Task Add_Post_RedirectsToIndex_WhenModelStateIsValid()
        {
            // Arrange
            var formModel = new SchoolFormModel { Name = "New School" };

            // Act
            var result = await this.controller.Add(formModel) as RedirectToActionResult;

            // Assert
            Assert.NotNull(result);
            Assert.Equal("Index", result.ActionName);
        }

        [Fact]
        public async Task Add_Post_ReturnsView_WhenModelStateIsInvalid()
        {
            // Arrange
            var formModel = new SchoolFormModel { Name = "Invalid School" };
            this.controller.ModelState.AddModelError("Address", "Required");

            // Act
            var result = await this.controller.Add(formModel) as ViewResult;

            // Assert
            Assert.NotNull(result);
            Assert.Equal(formModel, result.Model);
        }

        [Fact]
        public async Task Edit_Get_ReturnsViewWithSchool()
        {
            // Arrange
            var schoolDetails = new DetailsSchoolViewModel { Id = this.firstSchoolId, Name = "School 1" };
            this.mockSchoolService.Setup(s => s.GetSchoolDetailsByIdAsync(this.firstSchoolId)).ReturnsAsync(schoolDetails);

            // Act
            var result = await this.controller.Edit(this.firstSchoolId) as ViewResult;
            var model = result.Model as SchoolFormModel;

            // Assert
            Assert.NotNull(result);
            Assert.NotNull(model);
            Assert.Equal("School 1", model.Name);
        }

        [Fact]
        public async Task Edit_Post_RedirectsToDetails_WhenModelStateIsValid()
        {
            // Arrange
            var formModel = new SchoolFormModel { Name = "Updated School" };

            // Act
            var result = await this.controller.Edit(this.firstSchoolId, formModel) as RedirectToActionResult;

            // Assert
            Assert.NotNull(result);
            Assert.Equal("Details", result.ActionName);
            Assert.Equal(this.firstSchoolId, result.RouteValues["id"]);
        }

        [Fact]
        public async Task Edit_Post_ReturnsView_WhenModelStateIsInvalid()
        {
            // Arrange
            var formModel = new SchoolFormModel { Name = "Invalid School" };
            this.controller.ModelState.AddModelError("Address", "Required");

            // Act
            var result = await this.controller.Edit(this.firstSchoolId, formModel) as ViewResult;

            // Assert
            Assert.NotNull(result);
            Assert.Equal(formModel, result.Model);
        }

        [Fact]
        public async Task Delete_Get_ReturnsViewWithSchool()
        {
            // Arrange
            var schoolDetails = new DetailsSchoolViewModel { Id = this.firstSchoolId, Name = "School 1" };
            this.mockSchoolService.Setup(s => s.GetSchoolDetailsByIdAsync(this.firstSchoolId)).ReturnsAsync(schoolDetails);

            // Act
            var result = await this.controller.Delete(this.firstSchoolId) as ViewResult;
            var model = result.Model as DeleteSchoolViewModel;

            // Assert
            Assert.NotNull(result);
            Assert.NotNull(model);
            Assert.Equal(this.firstSchoolId, model.Id);
        }

        [Fact]
        public async Task DeleteConfirmed_Post_RedirectsToIndex()
        {
            // Arrange
            var formModel = new DeleteSchoolViewModel { Id = this.firstSchoolId };

            // Act
            var result = await this.controller.DeleteConfirmed(formModel) as RedirectToActionResult;

            // Assert
            Assert.NotNull(result);
            Assert.Equal("Index", result.ActionName);
            this.mockSchoolService.Verify(s => s.DeleteSchoolAsync(formModel), Times.Once);
        }

        private List<IndexSchoolViewModel> GetTestSchools()
        {
            return new List<IndexSchoolViewModel>
            {
                new IndexSchoolViewModel { Id = this.firstSchoolId, Name = "Alpha School", WebsiteUrl = "https://alpha.edu" },
                new IndexSchoolViewModel { Id = this.secondSchoolId, Name = "Beta School", WebsiteUrl = "https://beta.edu" },
            };
        }
    }
}
