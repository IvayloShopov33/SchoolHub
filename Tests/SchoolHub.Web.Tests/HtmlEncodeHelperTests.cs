namespace SchoolHub.Web.Tests
{
    using SchoolHub.Web.Infrastructure;

    using Xunit;

    public class HtmlEncodeHelperTests
    {
        [Fact]
        public void EscapeHtmlSpecialCharacters_ShouldReturnEmptyString()
        {
            // Act
            var resultString = HtmlEncodeHelper.EscapeHtmlSpecialCharacters(null);

            // Assert
            Assert.Equal(string.Empty, resultString);
        }

        [Fact]
        public void EscapeHtmlSpecialCharacters_ShouldReturnCorrectString()
        {
            // Arrange
            var inputString = "<div>&\'";

            // Act
            var resultString = HtmlEncodeHelper.EscapeHtmlSpecialCharacters(inputString);

            // Assert
            Assert.Equal("&lt;div&gt;and&#39;", resultString);
        }
    }
}
