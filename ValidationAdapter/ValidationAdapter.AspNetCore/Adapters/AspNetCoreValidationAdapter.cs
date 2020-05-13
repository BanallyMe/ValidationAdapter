using BanallyMe.ValidationAdapter.Adapters;
using BanallyMe.ValidationAdapter.ValidationResults;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using System.Collections.Generic;
using System.Linq;

namespace BanallyMe.ValidationAdapter.AspNetCore.Adapters
{
    /// <summary>
    /// Implementation of IValidationAdapter for the ASP.NET Core framework.
    /// </summary>
    public class AspNetCoreValidationAdapter : IValidationAdapter
    {
        private readonly IActionContextAccessor contextAccessor;

        public AspNetCoreValidationAdapter(IActionContextAccessor contextAccessor)
        {
            this.contextAccessor = contextAccessor;
        }

        /// <inheritdoc />
        public IEnumerable<ValidationError> GetAllValidationErrors()
            => contextAccessor.ActionContext.ModelState.SelectMany(ReadValidationErrorsFromModelStateEntry);

        /// <inheritdoc />
        public IEnumerable<ValidationError> GetValidationErrorsAtPath(string errorPath)
            => contextAccessor.ActionContext.ModelState
                .Where(stateEntry => ModelStateEntryIsAtPath(stateEntry, errorPath))
                .SelectMany(ReadValidationErrorsFromModelStateEntry);

        /// <inheritdoc />
        public bool HasValidationErrors() => !contextAccessor.ActionContext.ModelState.IsValid;

        private static IEnumerable<ValidationError> ReadValidationErrorsFromModelStateEntry(KeyValuePair<string, ModelStateEntry> modelStateEntry)
            => modelStateEntry.Value.Errors.Select(error => ValidationError.CreateErrorAtPath(error.ErrorMessage, modelStateEntry.Key));

        private static bool ModelStateEntryIsAtPath(KeyValuePair<string, ModelStateEntry> modelStateEntry, string path)
            => modelStateEntry.Key == path;

    }
}
