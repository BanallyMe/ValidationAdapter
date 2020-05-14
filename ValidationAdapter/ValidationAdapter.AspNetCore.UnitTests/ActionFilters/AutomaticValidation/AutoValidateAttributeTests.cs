using BanallyMe.ValidationAdapter.Adapters;
using BanallyMe.ValidationAdapter.AspNetCore.ActionFilters.AutomaticValidation;
using BanallyMe.ValidationAdapter.AspNetCore.ControllerOutput;
using BanallyMe.ValidationAdapter.ValidationResults;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Routing;
using Moq;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace BanallyMe.ValidationAdapter.AspNetCore.UnitTests.ActionFilters.AutomaticValidation
{
    public class AutoValidateAttributeTests
    {
        private readonly Mock<IValidationAdapter> validationAdapterMock;
        private readonly AutoValidateAttribute testedAttribute;

        public AutoValidateAttributeTests()
        {
            validationAdapterMock = new Mock<IValidationAdapter>();
            testedAttribute = new AutoValidateAttribute(validationAdapterMock.Object);
        }

        [Fact]
        public void OnActionExecuting_ChecksIfModelIsValid()
        {
            var fakeContext = CreateFakeExecutingContext();

            testedAttribute.OnActionExecuting(fakeContext);

            validationAdapterMock.Verify(adapter => adapter.HasValidationErrors(), Times.Once);
        }

        [Fact]
        public void OnActionExecuting_DoesntValidateIfExecutingContextIsNull()
        {
            testedAttribute.OnActionExecuting(null);

            validationAdapterMock.Verify(adapter => adapter.HasValidationErrors(), Times.Never);
            validationAdapterMock.Verify(adapter => adapter.Validate(), Times.Never);
        }

        [Fact]
        public void OnActionExecuting_DoesntSetResultIfModelIsValid()
        {
            var fakeContext = CreateFakeExecutingContext();

            testedAttribute.OnActionExecuting(fakeContext);

            fakeContext.Result.Should().BeNull();
        }

        [Fact]
        public void OnActionExecuting_SetsResultToErrorObjectIfModelIsInvalid()
        {
            SetupValidationAdapterToContainErrors();
            var fakeContext = CreateFakeExecutingContext();
            var expectedControllerOutput = FakeErrorControllerOutput;

            testedAttribute.OnActionExecuting(fakeContext);

            fakeContext.Result.Should().BeOfType<UnprocessableEntityObjectResult>()
                .Which.Value.Should().BeEquivalentTo(expectedControllerOutput);
        }

        private void SetupValidationAdapterToContainErrors()
        {
            var fakeValidationResult = ValidationResult.CreateInvalidResultFromValidationErrors(FakeErrors);
            validationAdapterMock.Setup(adapter => adapter.Validate()).Returns(fakeValidationResult);
            validationAdapterMock.Setup(adapter => adapter.HasValidationErrors()).Returns(true);
        }

        private IEnumerable<ValidationError> FakeErrors => new[]
        {
            ValidationError.CreateErrorAtPath("", "Global Error"),
            ValidationError.CreateErrorAtPath("path1", "Error for Path 1"),
            ValidationError.CreateErrorAtPath("path2", "First Error for Path 2"),
            ValidationError.CreateErrorAtPath("path2", "Second Error for Path 2"),
        };

        private IEnumerable<SerializableValidationResult> FakeErrorControllerOutput => FakeErrors.GroupBy(error => error.ErrorPath)
                .Select(errorGroup => new SerializableValidationResult { Path = errorGroup.Key, ErrorMessages = errorGroup.Select(error => error.ErrorMessage) });

        private static ActionExecutingContext CreateFakeExecutingContext()
        {
            var fakeActionContext = new ActionContext(Mock.Of<HttpContext>(), Mock.Of<RouteData>(), Mock.Of<ActionDescriptor>());
            var fakeContext = new ActionExecutingContext(fakeActionContext, new List<IFilterMetadata>(), new Dictionary<string, object>(), null);

            return fakeContext;
        }
    }
}
