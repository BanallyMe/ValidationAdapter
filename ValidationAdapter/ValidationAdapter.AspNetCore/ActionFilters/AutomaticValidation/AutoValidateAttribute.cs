using BanallyMe.ValidationAdapter.Adapters;
using Microsoft.AspNetCore.Mvc.Filters;

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
            if (validationAdapter.HasValidationErrors())
            {

            }
        }
    }
}
