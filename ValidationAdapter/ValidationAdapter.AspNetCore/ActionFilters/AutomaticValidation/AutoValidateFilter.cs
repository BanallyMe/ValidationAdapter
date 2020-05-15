using BanallyMe.ValidationAdapter.Adapters;
using BanallyMe.ValidationAdapter.AspNetCore.ControllerOutput;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Collections.Generic;
using System.Linq;

namespace BanallyMe.ValidationAdapter.AspNetCore.ActionFilters.AutomaticValidation
{
    public class AutoValidateFilter : IActionFilter
    {
        private readonly IValidationAdapter validationAdapter;

        public AutoValidateFilter(IValidationAdapter validationAdapter)
        {
            this.validationAdapter = validationAdapter;
        }

        public void OnActionExecuting(ActionExecutingContext context)
        {
            if (context != null && ActionHasAutoValidateAttribute(context) && validationAdapter.HasValidationErrors())
            {
                var outputBody = ConvertValidationResultToControllerOutput();
                context.Result = new UnprocessableEntityObjectResult(outputBody);
            }
        }

        public void OnActionExecuted(ActionExecutedContext context) { }

        private bool ActionHasAutoValidateAttribute(ActionExecutingContext context)
            => context.ActionDescriptor.FilterDescriptors.Any(descriptor => descriptor.Filter is AutoValidateAttribute);

        private IEnumerable<SerializableValidationResult> ConvertValidationResultToControllerOutput()
        {
            var validationResult = validationAdapter.Validate();

            return validationResult.ValidationErrors.Select(error => new SerializableValidationResult { Path = error.Path, ErrorMessages = error });
        }
    }
}
