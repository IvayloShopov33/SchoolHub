namespace SchoolHub.Web.Tests
{
    using System;
    using System.Security.Claims;

    using SchoolHub.Common;
    using SchoolHub.Web.Infrastructure;

    using Xunit;

    public class ClaimsPrincipalExtensionsTests
    {
        [Fact]
        public void GetId_ShouldReturnId_WhenClaimExists()
        {
            // Arrange
            var userId = Guid.NewGuid().ToString();
            var claims = new[] { new Claim(ClaimTypes.NameIdentifier, userId) };
            var claimsPrincipal = new ClaimsPrincipal(new ClaimsIdentity(claims));

            // Act
            var result = claimsPrincipal.GetId();

            // Assert
            Assert.Equal(userId, result);
        }

        [Fact]
        public void GetId_ShouldReturnNull_WhenClaimDoesNotExist()
        {
            // Arrange
            var claimsPrincipal = new ClaimsPrincipal(new ClaimsIdentity());

            // Act
            var result = claimsPrincipal.GetId();

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public void GetEmail_ShouldReturnEmail_WhenClaimExists()
        {
            // Arrange
            var email = "user@example.com";
            var claims = new[] { new Claim(ClaimTypes.Email, email) };
            var claimsPrincipal = new ClaimsPrincipal(new ClaimsIdentity(claims));

            // Act
            var result = claimsPrincipal.GetEmail();

            // Assert
            Assert.Equal(email, result);
        }

        [Fact]
        public void GetEmail_ShouldReturnNull_WhenClaimDoesNotExist()
        {
            // Arrange
            var claimsPrincipal = new ClaimsPrincipal(new ClaimsIdentity());

            // Act
            var result = claimsPrincipal.GetEmail();

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public void IsAdmin_ShouldReturnTrue_WhenUserIsInAdminRole()
        {
            // Arrange
            var claims = new[] { new Claim(ClaimTypes.Role, GlobalConstants.AdministratorRoleName) };
            var claimsPrincipal = new ClaimsPrincipal(new ClaimsIdentity(claims));

            // Act
            var result = claimsPrincipal.IsAdmin();

            // Assert
            Assert.True(result);
        }

        [Fact]
        public void IsAdmin_ShouldReturnFalse_WhenUserIsNotInAdminRole()
        {
            // Arrange
            var claims = new[] { new Claim(ClaimTypes.Role, GlobalConstants.StudentRoleName) };
            var claimsPrincipal = new ClaimsPrincipal(new ClaimsIdentity(claims));

            // Act
            var result = claimsPrincipal.IsAdmin();

            // Assert
            Assert.False(result);
        }

        [Fact]
        public void IsTeacher_ShouldReturnTrue_WhenUserIsInTeacherRole()
        {
            // Arrange
            var claims = new[] { new Claim(ClaimTypes.Role, GlobalConstants.TeacherRoleName) };
            var claimsPrincipal = new ClaimsPrincipal(new ClaimsIdentity(claims));

            // Act
            var result = claimsPrincipal.IsTeacher();

            // Assert
            Assert.True(result);
        }

        [Fact]
        public void IsTeacher_ShouldReturnFalse_WhenUserIsNotInTeacherRole()
        {
            // Arrange
            var claims = new[] { new Claim(ClaimTypes.Role, GlobalConstants.StudentRoleName) };
            var claimsPrincipal = new ClaimsPrincipal(new ClaimsIdentity(claims));

            // Act
            var result = claimsPrincipal.IsTeacher();

            // Assert
            Assert.False(result);
        }

        [Fact]
        public void IsStudent_ShouldReturnTrue_WhenUserIsInStudentRole()
        {
            // Arrange
            var claims = new[] { new Claim(ClaimTypes.Role, GlobalConstants.StudentRoleName) };
            var claimsPrincipal = new ClaimsPrincipal(new ClaimsIdentity(claims));

            // Act
            var result = claimsPrincipal.IsStudent();

            // Assert
            Assert.True(result);
        }

        [Fact]
        public void IsStudent_ShouldReturnFalse_WhenUserIsNotInStudentRole()
        {
            // Arrange
            var claims = new[] { new Claim(ClaimTypes.Role, GlobalConstants.TeacherRoleName) };
            var claimsPrincipal = new ClaimsPrincipal(new ClaimsIdentity(claims));

            // Act
            var result = claimsPrincipal.IsStudent();

            // Assert
            Assert.False(result);
        }
    }
}
