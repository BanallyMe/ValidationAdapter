using System;
using System.Collections.Generic;
using System.Linq;

namespace BanallyMe.ValidationAdapter.ValidationResults
{
    /// <summary>
    /// Container for the result of a complete validation.
    /// </summary>
    public class ValidationResult
    {
        private ValidationResult(IEnumerable<PathValidationErrorsCollection> errors)
        {
            ValidationErrors = errors;
        }

        /// <summary>
        /// Collection of all validation errors in this validation result.
        /// </summary>
        public IEnumerable<PathValidationErrorsCollection> ValidationErrors { get; }

        /// <summary>
        /// True if there are no errors in this validation result, false otherwise.
        /// </summary>
        public bool IsValid => !ValidationErrors.Any();

        /// <summary>
        /// Total count of all errors that have occured in the validation represented by this validation result.
        /// </summary>
        public int CountErrors => ValidationErrors.Sum(pathErrors => pathErrors.Count);

        /// <summary>
        /// Creates a validation result for a valid outcome of a validation.
        /// </summary>
        /// <returns>The valid result.</returns>
        public static ValidationResult CreateValidResult()
            => new ValidationResult(Array.Empty<PathValidationErrorsCollection>());

        /// <summary>
        /// Creates a validation result from a collection of validation errors.
        /// </summary>
        /// <param name="validationErrors">Errors that should be contained in this validation result.</param>
        /// <returns>The validation result containing all passed errors.</returns>
        /// <exception cref="ArgumentNullException">Thrown if parameter validationErrors is null.</exception>
        public static ValidationResult CreateInvalidResultFromValidationErrors(IEnumerable<ValidationError> validationErrors)
        {
            if (validationErrors is null)
                throw new ArgumentNullException(nameof(validationErrors));

            var pathErrors = validationErrors.GroupBy(error => error.ErrorPath)
                .Select(errorGroup => PathValidationErrorsCollection.CreateWithErrorsAtPath(errorGroup.Select(error => error.ErrorMessage), errorGroup.Key));

            return new ValidationResult(pathErrors);
        }
    }
}
