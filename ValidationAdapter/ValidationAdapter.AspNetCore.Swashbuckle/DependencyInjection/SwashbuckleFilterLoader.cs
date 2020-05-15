using BanallyMe.ValidationAdapter.AspNetCore.Swashbuckle.OperationFilters;
using Microsoft.Extensions.DependencyInjection;

namespace BanallyMe.ValidationAdapter.AspNetCore.Swashbuckle.DependencyInjection
{
    /// <summary>
    /// Contains extension methods to add a 422 response to automatically validated responses.
    /// </summary>
    public static class SwashbuckleFilterLoader
    {
        /// <summary>
        /// Adds swashbuckle filters to add a 422 response to every automatically validated operation.
        /// </summary>
        /// <param name="services">Collection of services used to build the ASP.NET Core dependency injection container.</param>
        public static void AddValidationAdapterToSwashbuckle(this IServiceCollection services)
        {
            services.ConfigureSwaggerGen(options => options.OperationFilter<AutoValidationOperationFilter>());
        }
    }
}
