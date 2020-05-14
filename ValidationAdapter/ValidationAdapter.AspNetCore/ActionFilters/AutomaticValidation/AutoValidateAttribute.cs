using BanallyMe.ValidationAdapter.Adapters;
using BanallyMe.ValidationAdapter.AspNetCore.ControllerOutput;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.Collections.Generic;
using System.Linq;

namespace BanallyMe.ValidationAdapter.AspNetCore.ActionFilters.AutomaticValidation
{
    /// <summary>
    /// The model of actions decorated with this ActionFilter will automatically be validated.
    /// If the model is invalid the response will be altered to a 422 response and return
    /// all found error messages.
    /// </summary>
    public sealed class AutoValidateAttribute : ActionFilterAttribute
    {
        private readonly IValidationAdapter validationAdapter;

        public AutoValidateAttribute(IValidationAdapter validationAdapter)
        {
            this.validationAdapter = validationAdapter;
        }

        public override void OnActionExecuting(ActionExecutingContext context)
        {
            if (context != null && validationAdapter.HasValidationErrors())
            {
                var outputBody = ConvertValidationResultToControllerOutput();
                context.Result = new UnprocessableEntityObjectResult(outputBody);
            }
        }

        private IEnumerable<SerializableValidationResult> ConvertValidationResultToControllerOutput()
        {
            var validationResult = validationAdapter.Validate();

            return validationResult.ValidationErrors.Select(error => new SerializableValidationResult { Path = error.Path, ErrorMessages = error });
        }
    }
}
