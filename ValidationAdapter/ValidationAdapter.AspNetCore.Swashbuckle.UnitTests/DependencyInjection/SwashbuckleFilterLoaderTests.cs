using BanallyMe.ValidationAdapter.AspNetCore.Swashbuckle.DependencyInjection;
using BanallyMe.ValidationAdapter.AspNetCore.Swashbuckle.OperationFilters;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Swashbuckle.AspNetCore.SwaggerGen;
using System;
using System.Linq;
using Xunit;

namespace BanallyMe.ValidationAdapter.AspNetCore.Swashbuckle.UnitTests.DependencyInjection
{
    public class SwashbuckleFilterLoaderTests
    {
        [Fact]
        public void AddValidationAdapterToSwashbuckle_AddsAutoValidationOperationFilter()
        {
            var fakeServiceCollection = new ServiceCollection();

            fakeServiceCollection.AddValidationAdapterToSwashbuckle();

            var configuredFilterAction = GetSwaggerConfigurationFromServices(fakeServiceCollection);
            configuredFilterAction.Should().NotBeNull();
            var configurationMethod = GetConfigurationMethodFromService(configuredFilterAction);
            AssertConfigurationMethodAddsAutoValidationOperationFilter(configurationMethod);
        }

        private ServiceDescriptor GetSwaggerConfigurationFromServices(IServiceCollection services)
            => services.FirstOrDefault(service => service.ServiceType == typeof(IConfigureOptions<SwaggerGenOptions>));

        private Action<SwaggerGenOptions> GetConfigurationMethodFromService(ServiceDescriptor service)
            => ((ConfigureNamedOptions<SwaggerGenOptions>)service.ImplementationInstance).Action;

        private void AssertConfigurationMethodAddsAutoValidationOperationFilter(Action<SwaggerGenOptions> configurationMethod)
        {
            var dummyOptions = new SwaggerGenOptions();
            configurationMethod.Invoke(dummyOptions);
            AssertSwaggerOptionsContainAutoValidationFilter(dummyOptions);
        }

        private void AssertSwaggerOptionsContainAutoValidationFilter(SwaggerGenOptions options)
        {
            options.OperationFilterDescriptors.Where(descriptor => descriptor.Type == typeof(AutoValidationOperationFilter)).Should().HaveCount(1);
        }
    }
}
