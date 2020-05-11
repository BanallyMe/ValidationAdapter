using BanallyMe.ValidationAdapter.ValidationResults;
using FluentAssertions;
using System;
using Xunit;

namespace BanallyMe.ValidationAdapter.UnitTests.ValidationResults
{
    public class PathValidationErrorsCollectionTests
    {
        [Fact]
        public void CreateWithErrorsAtPath_CreatesCollectionWithCorrectPath()
        {
            var testPath = "testpath";
            
            var testCollection = PathValidationErrorsCollection.CreateWithErrorsAtPath(new[] { "any error" }, testPath);

            testCollection.Path.Should().Be(testPath);
        }

        [Fact]
        public void CreateWithErrorsAtPath_CreatesCollectionWithAllPassedErrors()
        {
            var testCollection = PathValidationErrorsCollection.CreateWithErrorsAtPath(testErrors, "any Path");

            testCollection.Should().BeEquivalentTo(testErrors);
        }

        [Fact]
        public void Count_ReturnsCorrectErrorCount()
        {
            var expectedErrorCount = testErrors.Length;
            
            var testCollection = PathValidationErrorsCollection.CreateWithErrorsAtPath(testErrors, "any Path");

            testCollection.Count.Should().Be(expectedErrorCount);
        }

        [Fact]
        public void IndexedAccess_ReturnsCorrectErrorMessage()
        {
            var testedIndex = testErrors.Length - 1;
            
            var testErrorCollection = PathValidationErrorsCollection.CreateWithErrorsAtPath(testErrors, "any Path");

            testErrorCollection[testedIndex].Should().Be(testErrors[testedIndex]);
        }

        [Fact]
        public void IndexedAccess_ThrowsExceptionIfIndexOutOfRange()
        {
            var testedIndex = testErrors.Length;
            var testErrorCollection = PathValidationErrorsCollection.CreateWithErrorsAtPath(testErrors, "any Path");

            Action indexedAccess = () => _ = testErrorCollection[testedIndex];

            indexedAccess.Should().ThrowExactly<IndexOutOfRangeException>();
        }

        private string[] testErrors => new[] { "testError 1", "testError 2", "testError 3" };
    }
}
