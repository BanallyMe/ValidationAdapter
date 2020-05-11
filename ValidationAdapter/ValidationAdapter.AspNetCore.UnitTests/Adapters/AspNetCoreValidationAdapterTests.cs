using BanallyMe.ValidationAdapter.AspNetCore.Adapters;
using Microsoft.AspNetCore.Http;
using Moq;

namespace BanallyMe.ValidationAdapter.AspNetCore.UnitTests.Adapters
{
    public class AspNetCoreValidationAdapterTests
    {
        private readonly Mock<IHttpContextAccessor> contextAccessorMock;
        private readonly AspNetCoreValidationAdapter testedAdapter;

        public AspNetCoreValidationAdapterTests()
        {
            contextAccessorMock = new Mock<IHttpContextAccessor>();
            testedAdapter = new AspNetCoreValidationAdapter(contextAccessorMock.Object);
        }
    }
}
