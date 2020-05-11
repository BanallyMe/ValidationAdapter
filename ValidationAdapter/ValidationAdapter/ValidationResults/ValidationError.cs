using System;

namespace BanallyMe.ValidationAdapter.ValidationResults
{
    /// <summary>
    /// Container for errors that occured while validating.
    /// </summary>
    public class ValidationError
    {
        private ValidationError(string errorMessage, string errorPath)
        {
            ErrorPath = errorPath;
            ErrorMessage = errorMessage;
        }

        /// <summary>
        /// The path to the model's property that has been considered as invalid.
        /// </summary>
        public string ErrorPath { get; }

        /// <summary>
        /// Validation message that expresses, while the property at path has been considered as invalid.
        /// </summary>
        public string ErrorMessage { get; }

        /// <summary>
        /// Returns true if this validation error is a global error, otherwise returns false.
        /// </summary>
        public bool IsGlobal => ErrorPath.Length == 0;

        /// <summary>
        /// Creates a validation error that cannot be assigned to a specific property of the model and is therefore considered as a global error.
        /// </summary>
        /// <param name="errorMessage">Message that describes this global error.</param>
        /// <returns>Instance of the created Error.</returns>
        public static ValidationError CreateGlobalError(string errorMessage)
        {
            if (errorMessage is null)
                throw new ArgumentNullException(nameof(errorMessage));

            return new ValidationError(errorMessage, "");
        }

        /// <summary>
        /// Created a validation error thet refers to a specific property of the model which can be found at path.
        /// </summary>
        /// <param name="errorMessage">Message that describes this specific error.</param>
        /// <param name="errorPath">Path to the model's property that has been consideres as invalid.</param>
        /// <returns>Instance of the created specific error.</returns>
        public static ValidationError CreateErrorAtPath(string errorMessage, string errorPath)
        {
            if (errorMessage is null)
                throw new ArgumentNullException(nameof(errorMessage));

            return new ValidationError(errorMessage, errorPath ?? "");
        }
    }
}
