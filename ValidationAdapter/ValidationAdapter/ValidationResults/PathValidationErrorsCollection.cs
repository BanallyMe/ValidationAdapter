using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace BanallyMe.ValidationAdapter.ValidationResults
{
    /// <summary>
    /// Container for the results of the validation of a specific path to a model's property.
    /// </summary>
    public class PathValidationErrorsCollection : IEnumerable<string>
    {
        private PathValidationErrorsCollection(string path, string[] initialErrors)
        {
            Path = path;
            ErrorMessages = initialErrors;
        }

        /// <summary>
        /// Collection of all error messages for the path of this validation result.
        /// </summary>
        private string[] ErrorMessages { get; }

        /// <summary>
        /// Path to the model's property that this result is referring to.
        /// </summary>
        public string Path { get; }

        /// <summary>
        /// Count of Errors in this collection.
        /// </summary>
        public int Count => ErrorMessages.Length;

        /// <summary>
        /// Creates an error collection based on the passed collection of error messages for a specific model path.
        /// </summary>
        /// <param name="errorMessages">Collection of error messages that should be added for this path.</param>
        /// <param name="path">Path to the model's property that this collection of errors refers to.</param>
        /// <returns>The requested collection of errors for the path.</returns>
        public static PathValidationErrorsCollection CreateWithErrorsAtPath(IEnumerable<string> errorMessages, string path)
            => new PathValidationErrorsCollection(path, errorMessages.ToArray());

        /// <summary>
        /// Gets the error message at a specific index of the collection.
        /// </summary>
        /// <param name="index">Index for which the error message is requested.</param>
        /// <returns>Error message at this index.</returns>
        public string this[int index]
        {
            get { return ErrorMessages[index]; }
        }

        public IEnumerator<string> GetEnumerator()
        {
            foreach (var errorMessage in ErrorMessages)
            {
                yield return errorMessage;
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
