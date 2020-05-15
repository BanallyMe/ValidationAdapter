using Microsoft.AspNetCore.Mvc.Filters;
using System;

namespace BanallyMe.ValidationAdapter.AspNetCore.ActionFilters.AutomaticValidation
{
    /// <summary>
    /// The model of actions decorated with this ActionFilter will automatically be validated.
    /// If the model is invalid the response will be altered to a 422 response and return
    /// all found error messages.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = false)]
    public sealed class AutoValidateAttribute : Attribute, IFilterMetadata
    { }
}
