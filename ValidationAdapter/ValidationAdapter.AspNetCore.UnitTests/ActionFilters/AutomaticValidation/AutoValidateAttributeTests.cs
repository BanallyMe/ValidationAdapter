using BanallyMe.ValidationAdapter.Adapters;
using BanallyMe.ValidationAdapter.AspNetCore.ActionFilters.AutomaticValidation;
using Moq;
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
            testedAttribute.OnActionExecuting(null);

            validationAdapterMock.Verify(adapter => adapter.HasValidationErrors(), Times.Once);
        }
    }
}
