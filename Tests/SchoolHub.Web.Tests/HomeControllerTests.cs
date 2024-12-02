namespace SchoolHub.Web.Tests
{
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Security.Claims;
    using System.Threading.Tasks;

    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Logging;

    using Moq;

    using SchoolHub.Services;
    using SchoolHub.Web.Controllers;
    using SchoolHub.Web.ViewModels;

    using Xunit;

    public class HomeControllerTests
    {
        private readonly Mock<ILogger<HomeController>> mockLogger;
        private readonly Mock<IStudentService> mockStudentService;
        private readonly HomeController controller;

        public HomeControllerTests()
        {
            this.mockLogger = new Mock<ILogger<HomeController>>();
            this.mockStudentService = new Mock<IStudentService>();
            this.controller = new HomeController(this.mockLogger.Object, this.mockStudentService.Object);
        }

        [Fact]
        public async Task Index_ShouldRedirectToStudentStatistics_WhenUserIsStudent()
        {
            // Arrange
            var userId = "123";
            var studentId = "456";
            var user = this.CreateClaimsPrincipal(userId, isStudent: true);
            this.controller.ControllerContext = this.CreateControllerContext(user);

            this.mockStudentService
                .Setup(s => s.GetStudentIdByUserIdAsync(userId))
                .ReturnsAsync(studentId);

            // Act
            var result = await this.controller.Index();

            // Assert
            var redirectResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Statistics", redirectResult.ActionName);
            Assert.Equal("Student", redirectResult.ControllerName);
            Assert.Equal(studentId, redirectResult.RouteValues["studentId"]);
        }

        [Fact]
        public async Task Index_ShouldRedirectToSchoolIndex_WhenUserIsAuthenticatedButNotStudent()
        {
            // Arrange
            var userId = "123";
            var user = this.CreateClaimsPrincipal(userId, isStudent: false);
            this.controller.ControllerContext = this.CreateControllerContext(user);

            // Act
            var result = await this.controller.Index();

            // Assert
            var redirectResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Index", redirectResult.ActionName);
            Assert.Equal("School", redirectResult.ControllerName);
        }

        [Fact]
        public async Task Index_ShouldReturnView_WhenUserIsNotAuthenticated()
        {
            // Arrange
            var user = this.CreateClaimsPrincipal(userId: null);
            this.controller.ControllerContext = this.CreateControllerContext(user);

            // Act
            var result = await this.controller.Index();

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.Null(viewResult.Model);
        }

        [Fact]
        public void Error_ShouldReturnErrorViewModel()
        {
            // Arrange
            var traceId = Activity.Current?.Id ?? "test-trace-id";
            var httpContext = new DefaultHttpContext();
            httpContext.TraceIdentifier = traceId;

            this.controller.ControllerContext = new ControllerContext
            {
                HttpContext = httpContext,
            };

            // Act
            var result = this.controller.Error();

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsType<ErrorViewModel>(viewResult.Model);
            Assert.Equal(traceId, model.RequestId);
        }

        [Fact]
        public void Status_ShouldRedirectToIndex_WhenCodeIsNull()
        {
            // Act
            var result = this.controller.Status(null);

            // Assert
            var redirectResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Index", redirectResult.ActionName);
        }

        [Theory]
        [InlineData(400, "BadRequest")]
        [InlineData(401, "Unauthorized")]
        [InlineData(404, "NotFound")]
        [InlineData(500, "ServerError")]
        [InlineData(403, "Error")]
        public void Status_ShouldReturnCorrectView_ForSpecificStatusCodes(int statusCode, string expectedViewName)
        {
            // Act
            var result = this.controller.Status(statusCode);

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.Equal(expectedViewName, viewResult.ViewName);
        }

        private ClaimsPrincipal CreateClaimsPrincipal(
            string userId,
            bool isStudent = false,
            bool isAdmin = false,
            bool isTeacher = false)
        {
            var claims = new List<Claim>();

            if (!string.IsNullOrEmpty(userId))
            {
                claims.Add(new Claim(ClaimTypes.NameIdentifier, userId));
            }

            if (isStudent)
            {
                claims.Add(new Claim(ClaimTypes.Role, SchoolHub.Common.GlobalConstants.StudentRoleName));
            }

            if (isAdmin)
            {
                claims.Add(new Claim(ClaimTypes.Role, SchoolHub.Common.GlobalConstants.AdministratorRoleName));
            }

            if (isTeacher)
            {
                claims.Add(new Claim(ClaimTypes.Role, SchoolHub.Common.GlobalConstants.TeacherRoleName));
            }

            return new ClaimsPrincipal(new ClaimsIdentity(claims, "TestAuthentication"));
        }

        private ControllerContext CreateControllerContext(ClaimsPrincipal user)
        {
            var httpContext = new DefaultHttpContext
            {
                User = user,
            };

            return new ControllerContext
            {
                HttpContext = httpContext,
            };
        }
    }
}
