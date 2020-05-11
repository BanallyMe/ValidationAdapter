using BanallyMe.ValidationAdapter.ValidationResults;
using FluentAssertions;
using System;
using Xunit;

namespace BanallyMe.ValidationAdapter.UnitTests.ValidationResults
{
    public class ValidationResultTests
    {
        [Fact]
        public void CreateValidResult_CreatesResultWithoutErrors()
        {
            var testResult = ValidationResult.CreateValidResult();

            testResult.ValidationErrors.Should().BeEmpty();
        }

        [Fact]
        public void CreateInvalidResultFromValidationErrors_ThrowsExceptionIfPassedErrorsAreNull()
        {
            Action resultCreation = () => ValidationResult.CreateInvalidResultFromValidationErrors(null);

            resultCreation.Should().ThrowExactly<ArgumentNullException>()
                .Which.ParamName.Should().Be("validationErrors");
        }

        [Fact]
        public void CreateInvalidResultFromValidationErrors_CreatesResultWithCorrectErrors()
        {
            var testResult = ValidationResult.CreateInvalidResultFromValidationErrors(testRawErrors);

            testResult.ValidationErrors.Should().BeEquivalentTo(testResultErrors);
        }

        [Fact]
        public void CountErrors_ReturnsZeroErrorCountWhenNoErrorsPresent()
        {
            var testResult = ValidationResult.CreateValidResult();

            testResult.CountErrors.Should().Be(0);
        }

        [Fact]
        public void CountErrors_ReturnsCorrectErrorCountWhenErrorsPresent()
        {
            var expectedErrorCount = testRawErrors.Length;
            
            var testResult = ValidationResult.CreateInvalidResultFromValidationErrors(testRawErrors);

            testResult.CountErrors.Should().Be(expectedErrorCount);
        }

        [Fact]
        public void IsValid_ReturnsTrueIfNoErrorsPresent()
        {
            var testResult = ValidationResult.CreateValidResult();

            testResult.IsValid.Should().BeTrue();
        }

        [Fact]
        public void IsValid_ReturnsFalseIfNoErrorsPresent()
        {
            var testResult = ValidationResult.CreateInvalidResultFromValidationErrors(testRawErrors);

            testResult.IsValid.Should().BeFalse();
        }

        public ValidationError[] testRawErrors =>
            new[]
            {
                ValidationError.CreateErrorAtPath("Error 1", "path1"),
                ValidationError.CreateGlobalError("Error 2"),
                ValidationError.CreateErrorAtPath("Error 3", "path2"),
                ValidationError.CreateErrorAtPath("Error 4", "path2")
            };

        public PathValidationErrorsCollection[] testResultErrors =>
            new[]
            {
                PathValidationErrorsCollection.CreateWithErrorsAtPath(new [] { "Error 1" }, "path1"),
                PathValidationErrorsCollection.CreateWithErrorsAtPath(new [] { "Error 2" }, ""),
                PathValidationErrorsCollection.CreateWithErrorsAtPath(new [] { "Error 3", "Error 4"}, "path2")
            };
    }

}
