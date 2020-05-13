using BanallyMe.ValidationAdapter.AspNetCore.Adapters;
using BanallyMe.ValidationAdapter.ValidationResults;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Moq;
using System.Linq;
using Xunit;

namespace BanallyMe.ValidationAdapter.AspNetCore.UnitTests.Adapters
{
    public class AspNetCoreValidationAdapterTests
    {
        private readonly Mock<IActionContextAccessor> contextAccessorMock;
        private readonly AspNetCoreValidationAdapter testedAdapter;

        public AspNetCoreValidationAdapterTests()
        {
            contextAccessorMock = new Mock<IActionContextAccessor>();
            testedAdapter = new AspNetCoreValidationAdapter(contextAccessorMock.Object);
        }

        [Fact]
        public void HasValidationError_ReturnsFalseIfModelStateHasNoErrors()
        {
            AddActionContextWithoutErrorsToAccessor();

            testedAdapter.HasValidationErrors().Should().BeFalse();
        }

        [Fact]
        public void HasValidationError_ReturnsTrueIfModelStateHasErrors()
        {
            AddActionContextWithErrorsToAccessor();

            testedAdapter.HasValidationErrors().Should().BeTrue();
        }

        [Fact]
        public void GetAllValidationErrors_ReturnsEmptyEnumerableIfNoErrorsPresent()
        {
            AddActionContextWithoutErrorsToAccessor();

            var foundErrors = testedAdapter.GetAllValidationErrors();

            foundErrors.Should().BeEmpty();
        }

        [Fact]
        public void GetAllValidationErrors_ReturnsAllErrorsFromActionContext()
        {
            AddActionContextWithErrorsToAccessor();
            var expectedErrors = FakeValidationErrors.Select(CreateValidationErrorFromFakeError);

            var foundErrors = testedAdapter.GetAllValidationErrors();

            foundErrors.Should().BeEquivalentTo(expectedErrors);
        }

        [Fact]
        public void GetValidationErrorsAtPath_ReturnsEmptyEnumerableIfNoErrorsPresentAtAll()
        {
            AddActionContextWithoutErrorsToAccessor();

            var foundErrors = testedAdapter.GetValidationErrorsAtPath("");

            foundErrors.Should().BeEmpty();
        }

        [Fact]
        public void GetValidationErrorsAtPath_ReturnsEmptyEnumerableIfNoErrorsPresentAtPath()
        {
            AddActionContextWithoutErrorsToAccessor();

            var foundErrors = testedAdapter.GetValidationErrorsAtPath("nonExistingPath");

            foundErrors.Should().BeEmpty();
        }

        [Fact]
        public void GetValidationErrorsAtPath_ReturnsAllErrorsFromActionContext()
        {
            AddActionContextWithErrorsToAccessor();
            var testPath = FakeValidationErrors.Last().path;
            var expectedErrors = FakeValidationErrors.Where(error => error.path == testPath)
                .Select(CreateValidationErrorFromFakeError);

            var foundErrors = testedAdapter.GetValidationErrorsAtPath(testPath);

            foundErrors.Should().BeEquivalentTo(expectedErrors);
        }

        private (string path, string errorMessage)[] FakeValidationErrors => new[]
        {
            ("", "Global testerror"),
            ("path.1", "error at path 1"),
            ("path.2" , "First error at path 2"),
            ("path.2", "Second error at path 2")
        };

        private void AddActionContextWithErrorsToAccessor()
        {
            var fakeActionContext = new ActionContext();
            foreach (var (path, errorMessage) in FakeValidationErrors)
            {
                fakeActionContext.ModelState.AddModelError(path, errorMessage);
            }
            contextAccessorMock.Setup(accessor => accessor.ActionContext).Returns(fakeActionContext);
        }

        private void AddActionContextWithoutErrorsToAccessor()
        {
            var fakeActionContext = new ActionContext();
            contextAccessorMock.Setup(accessor => accessor.ActionContext).Returns(fakeActionContext);
        }

        private ValidationError CreateValidationErrorFromFakeError((string path, string errorMessage) fakeError)
            => ValidationError.CreateErrorAtPath(fakeError.errorMessage, fakeError.path);
    }
}
