using System;
using System.Collections.Generic;

namespace BanallyMe.ValidationAdapter.AspNetCore.ControllerOutput
{
    /// <summary>
    /// A serializable validation result for use as controller output. This way the output can be
    /// easily processed by the receiving software.
    /// </summary>
    public class SerializableValidationResult
    {
        /// <summary>
        /// Path to the model's property, this result is referring to.
        /// </summary>
        public string Path { get; set; } = "";

        /// <summary>
        /// Collection of all error messages, that the validation of this path produced.
        /// </summary>
        public IEnumerable<string> ErrorMessages { get; set; } = Array.Empty<string>();
    }
}
