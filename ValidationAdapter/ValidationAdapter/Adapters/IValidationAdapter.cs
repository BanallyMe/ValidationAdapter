using BanallyMe.ValidationAdapter.ValidationResults;
using System.Collections.Generic;

namespace BanallyMe.ValidationAdapter.Adapters
{
    /// <summary>
    /// Provides an interface for standardising validating functionalities.
    /// </summary>
    public interface IValidationAdapter
    {
        /// <summary>
        /// Checks if the validation adapter has found any validation errors and returns the result.
        /// </summary>
        /// <returns>True, if validation errors has been found, false otherwise.</returns>
        public bool HasValidationErrors();

        /// <summary>
        /// Gets and returns all validation errors found by the validation adapter.
        /// </summary>
        /// <returns>Collection of all validation errors that could be found.</returns>
        public IEnumerable<ValidationError> GetAllValidationErrors();

        /// <summary>
        /// Gets and returns all validation errors found by the validation adapter for a specific error path.
        /// </summary>
        /// <param name="path">Path to the property of the model, whose validation errors are requested.</param>
        /// <returns>All errors that could be found for the passed error path.</returns>
        public IEnumerable<ValidationError> GetValidationErrorsAtPath(string errorPath);
    }
}
