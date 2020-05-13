using BanallyMe.ValidationAdapter.Adapters;
using BanallyMe.ValidationAdapter.AspNetCore.Adapters;
using BanallyMe.ValidationAdapter.AspNetCore.DependencyInjection;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.Extensions.DependencyInjection;
using System.Linq;
using Xunit;

namespace BanallyMe.ValidationAdapter.AspNetCore.UnitTests.DependencyInjection
{
    public class ValidationAdapterLoaderTests
    {
        private readonly ServiceCollection fakeServiceCollection;

        public ValidationAdapterLoaderTests()
        {
            fakeServiceCollection = new ServiceCollection();
        }

        [Fact]
        public void AddAspNetCoreValidationAdapter_AddsActionContextAccessorIfNotContainedInServiceCollectionYet()
        {
            fakeServiceCollection.AddAspNetCoreValidationAdapter();

            fakeServiceCollection.Where(ServiceIsActionContextAccessor).Should().HaveCount(1);
        }

        [Fact]
        public void AddAspNetCoreValidationAdapter_DoesntAddActionContextAccessorIfAlreadyContainedInServiceCollection()
        {
            fakeServiceCollection.AddTransient<IActionContextAccessor, ActionContextAccessor>();

            fakeServiceCollection.AddAspNetCoreValidationAdapter();

            fakeServiceCollection.Where(ServiceIsActionContextAccessor).Should().HaveCount(1);
        }

        [Fact]
        public void AddAspNetCoreValidationAdapter_AddsAspNetCoreValidationAdapter()
        {
            fakeServiceCollection.AddAspNetCoreValidationAdapter();

            fakeServiceCollection.Where(ServiceIsAspNetCoreValidationAdapter).Should().HaveCount(1);
        }

        [Fact]
        public void AddAspNetCoreValidationAdapter_AddsAspNetCoreValidationAdapterOnlyOnceIfCalledMultipleTimes()
        {
            fakeServiceCollection.AddAspNetCoreValidationAdapter();
            fakeServiceCollection.AddAspNetCoreValidationAdapter();
            fakeServiceCollection.AddAspNetCoreValidationAdapter();

            fakeServiceCollection.Where(ServiceIsAspNetCoreValidationAdapter).Should().HaveCount(1);
        }

        private static bool ServiceIsActionContextAccessor(ServiceDescriptor serviceDescriptor)
            => serviceDescriptor.ServiceType == typeof(IActionContextAccessor)
                && serviceDescriptor.ImplementationType == typeof(ActionContextAccessor)
                && serviceDescriptor.Lifetime == ServiceLifetime.Transient;

        private static bool ServiceIsAspNetCoreValidationAdapter(ServiceDescriptor serviceDescriptor)
            => serviceDescriptor.ServiceType == typeof(IValidationAdapter)
                && serviceDescriptor.ImplementationType == typeof(AspNetCoreValidationAdapter)
                && serviceDescriptor.Lifetime == ServiceLifetime.Transient;
    }
}
