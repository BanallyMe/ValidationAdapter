using BanallyMe.ValidationAdapter.Adapters;
using BanallyMe.ValidationAdapter.ValidationResults;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;

namespace BanallyMe.ValidationAdapter.AspNetCore.Adapters
{
    /// <summary>
    /// Implementation of IValidationAdapter for the ASP.NET Core framework.
    /// </summary>
    public class AspNetCoreValidationAdapter : IValidationAdapter
    {
        private readonly IHttpContextAccessor contextAccessor;

        public AspNetCoreValidationAdapter(IHttpContextAccessor contextAccessor)
        {
            this.contextAccessor = contextAccessor;
        }

        /// <inheritdoc />
        public IEnumerable<ValidationError> GetAllValidationErrors()
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc />
        public IEnumerable<ValidationError> GetValidationErrorsAtPath(string errorPath)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc />
        public bool HasValidationErrors()
        {
            throw new NotImplementedException();
        }
    }
}
