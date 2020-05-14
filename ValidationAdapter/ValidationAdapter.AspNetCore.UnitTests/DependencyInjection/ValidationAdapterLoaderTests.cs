using BanallyMe.ValidationAdapter.Adapters;
using BanallyMe.ValidationAdapter.AspNetCore.ActionFilters.AutomaticValidation;
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

        [Fact]
        public void AddAspNetCoreValidationAdapter_AddsAutoValidateFilter()
        {
            fakeServiceCollection.AddAspNetCoreValidationAdapter();

            fakeServiceCollection.Where(ServiceIsAutoValidateFilter).Should().HaveCount(1);
        }

        [Fact]
        public void AddAspNetCoreValidationAdapter_AddsAutoValidateFilterOnlyOnceIfCalledMultipleTimes()
        {
            fakeServiceCollection.AddAspNetCoreValidationAdapter();
            fakeServiceCollection.AddAspNetCoreValidationAdapter();
            fakeServiceCollection.AddAspNetCoreValidationAdapter();

            fakeServiceCollection.Where(ServiceIsAutoValidateFilter).Should().HaveCount(1);
        }

        private static bool ServiceIsActionContextAccessor(ServiceDescriptor serviceDescriptor)
            => ServiceIsOfImplementationType<IActionContextAccessor, ActionContextAccessor>(serviceDescriptor);

        private static bool ServiceIsAspNetCoreValidationAdapter(ServiceDescriptor serviceDescriptor)
            => ServiceIsOfImplementationType<IValidationAdapter, AspNetCoreValidationAdapter>(serviceDescriptor);

        private static bool ServiceIsAutoValidateFilter(ServiceDescriptor serviceDescriptor)
            => ServiceIsOfImplementationType<AutoValidateAttribute, AutoValidateAttribute>(serviceDescriptor);

        private static bool ServiceIsOfImplementationType<TServiceType, TImplementationType>(ServiceDescriptor serviceDescriptor)
            => serviceDescriptor.ServiceType == typeof(TServiceType)
                && serviceDescriptor.ImplementationType == typeof(TImplementationType)
                && serviceDescriptor.Lifetime == ServiceLifetime.Transient;
    }
}
