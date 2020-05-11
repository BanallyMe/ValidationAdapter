using BanallyMe.ValidationAdapter.ValidationErrors;
using FluentAssertions;
using System;
using Xunit;

namespace BanallyMe.ValidationAdapter.UnitTests.ValidationErrors
{
    public class ValidationErrorTests
    {
        [Theory]
        [InlineData("")]
        [InlineData("testmessage")]
        public void CreateGlobalError_SetsErrorMessageCorrectly(string testErrorMessage)
        {
            var testError = ValidationError.CreateGlobalError(testErrorMessage);

            testError.ErrorMessage.Should().Be(testErrorMessage);
        }

        [Fact]
        public void CreateGlobalError_SetsEmptyPath()
        {
            var testError = ValidationError.CreateGlobalError("any Message");

            testError.ErrorPath.Should().BeEmpty();
        }

        [Fact]
        public void CreateGlobalError_ThrowsExceptionIfErrorMessageIsNull()
        {
            Action errorCreation = () => ValidationError.CreateGlobalError(null);

            errorCreation.Should().ThrowExactly<ArgumentNullException>().Which.ParamName.Should().Be("errorMessage");
        }

        [Theory]
        [InlineData("")]
        [InlineData("testmessage")]
        public void CreateErrorAtPath_SetsErrorMessageCorrectly(string testErrorMessage)
        {
            var testError = ValidationError.CreateErrorAtPath(testErrorMessage, "any Path");

            testError.ErrorMessage.Should().Be(testErrorMessage);
        }

        [Theory]
        [InlineData("")]
        [InlineData("testPath")]
        public void CreateErrorAtPath_SetsErrorPathCorrectly(string testPath)
        {
            var testError = ValidationError.CreateErrorAtPath("any error message", testPath);

            testError.ErrorPath.Should().Be(testPath);
        }

        [Fact]
        public void CreateErrorAtPath_ThrowsExceptionIfErrorMessageIsNull()
        {
            Action errorCreation = () => ValidationError.CreateErrorAtPath(null, "any Path");

            errorCreation.Should().ThrowExactly<ArgumentNullException>().Which.ParamName.Should().Be("errorMessage");
        }

        [Fact]
        public void CreateErrorAtPath_ConvertsNullPathToEmptyPath()
        {
            var testError = ValidationError.CreateErrorAtPath("any Message", null);

            testError.ErrorPath.Should().BeEmpty();
        }

        [Fact]
        public void IsGlobal_ReturnsTrueIfPathIsEmpty()
        {
            var testError = ValidationError.CreateErrorAtPath("any Message", "");

            testError.IsGlobal.Should().BeTrue();
        }

        [Theory]
        [InlineData(" ")]
        [InlineData("dfgdf")]
        public void IsGlobal_ReturnsFalseIfPathIsSet(string testErrorPath)
        {
            var testError = ValidationError.CreateErrorAtPath("any Message", testErrorPath);

            testError.IsGlobal.Should().BeFalse();
        }
    }
}
