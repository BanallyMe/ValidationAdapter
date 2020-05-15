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
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace BanallyMe.ValidationAdapter.AspNetCore.UnitTests.ActionFilters.AutomaticValidation
{
    public class AutoValidateFilterTests
    {
        private readonly Mock<IValidationAdapter> validationAdapterMock;
        private readonly AutoValidateFilter testedFilter;

        public AutoValidateFilterTests()
        {
            validationAdapterMock = new Mock<IValidationAdapter>();
            testedFilter = new AutoValidateFilter(validationAdapterMock.Object);
        }

        [Fact]
        public void OnActionExecuting_ChecksIfModelIsValid()
        {
            var fakeContext = CreateFakeExecutingContextWithAutoValidateFilter();

            testedFilter.OnActionExecuting(fakeContext);

            validationAdapterMock.Verify(adapter => adapter.HasValidationErrors(), Times.Once);
        }

        [Fact]
        public void OnActionExecuting_DoesntValidateIfExecutingContextIsNull()
        {
            testedFilter.OnActionExecuting(null);

            validationAdapterMock.Verify(adapter => adapter.HasValidationErrors(), Times.Never);
            validationAdapterMock.Verify(adapter => adapter.Validate(), Times.Never);
        }

        [Fact]
        public void OnActionExecuting_DoesntSetResultIfModelIsValid()
        {
            var fakeContext = CreateFakeExecutingContextWithAutoValidateFilter();

            testedFilter.OnActionExecuting(fakeContext);

            fakeContext.Result.Should().BeNull();
        }

        [Fact]
        public void OnActionExecuting_SetsResultToErrorObjectIfModelIsInvalid()
        {
            SetupValidationAdapterToContainErrors();
            var fakeContext = CreateFakeExecutingContextWithAutoValidateFilter();
            var expectedControllerOutput = FakeErrorControllerOutput;

            testedFilter.OnActionExecuting(fakeContext);

            fakeContext.Result.Should().BeOfType<UnprocessableEntityObjectResult>();
            var result = (UnprocessableEntityObjectResult)fakeContext.Result;
            var resultBody = JsonConvert.DeserializeObject<IEnumerable<SerializableValidationResult>>((string)result.Value);
            resultBody.Should().BeEquivalentTo(expectedControllerOutput);
        }

        [Fact]
        public void OnActionExecuting_DoesntSetResultIfActionIsNotDecoratedWithAutoValidate()
        {
            SetupValidationAdapterToContainErrors();
            var fakeContext = CreateFakeExecutingContextWithoutFilters();

            testedFilter.OnActionExecuting(fakeContext);

            fakeContext.Result.Should().BeNull();
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

        private static ActionExecutingContext CreateFakeExecutingContextWithAutoValidateFilter()
        {
            var fakeContext = CreateFakeExecutingContextWithoutFilters();
            var autoValidateFilterDescriptor = new FilterDescriptor(new AutoValidateAttribute(), 0);
            fakeContext.ActionDescriptor.FilterDescriptors.Add(autoValidateFilterDescriptor);

            return fakeContext;
        }

        private static ActionExecutingContext CreateFakeExecutingContextWithoutFilters()
        {
            var fakeActionDescriptor = new ActionDescriptor { FilterDescriptors = new List<FilterDescriptor>() };
            var fakeActionContext = new ActionContext(Mock.Of<HttpContext>(), Mock.Of<RouteData>(), fakeActionDescriptor);
            var fakeContext = new ActionExecutingContext(fakeActionContext, new List<IFilterMetadata>(), new Dictionary<string, object>(), null);

            return fakeContext;
        }
    }
}
