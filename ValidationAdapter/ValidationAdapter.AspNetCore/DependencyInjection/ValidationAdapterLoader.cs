using BanallyMe.ValidationAdapter.Adapters;
using BanallyMe.ValidationAdapter.AspNetCore.Adapters;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace BanallyMe.ValidationAdapter.AspNetCore.DependencyInjection
{
    /// <summary>
    /// Contains expansion methods for adding an ASP.NET Core Validation Adapter to the standard
    /// ServiceCollection for building an ASP.NET Core dependency injection container.
    /// </summary>
    public static class ValidationAdapterLoader
    {
        /// <summary>
        /// Adds an implementation of IValidationAdapter for the ASP.NET Core framework
        /// to the service collection of the dependency injection container.
        /// </summary>
        /// <param name="services">Collection of services for building a dependency injection container.</param>
        public static void AddAspNetCoreValidationAdapter(this IServiceCollection services)
        {
            services.TryAddTransient<IActionContextAccessor, ActionContextAccessor>();
            services.TryAddTransient<IValidationAdapter, AspNetCoreValidationAdapter>();
        }
    }
}
