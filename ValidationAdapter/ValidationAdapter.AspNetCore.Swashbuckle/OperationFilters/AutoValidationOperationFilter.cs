using BanallyMe.ValidationAdapter.AspNetCore.ActionFilters.AutomaticValidation;
using BanallyMe.ValidationAdapter.AspNetCore.ControllerOutput;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using System.Collections.Generic;
using System.Linq;

namespace BanallyMe.ValidationAdapter.AspNetCore.Swashbuckle.OperationFilters
{
    /// <summary>
    /// Swashbuckle operation filter for adding validation response to all actions which are decorated
    /// with the <see cref="AutoValidateAttribute"/>
    /// </summary>
    public class AutoValidationOperationFilter : IOperationFilter
    {
        /// <summary>
        /// Adds possibility of 422 response to all actions decorated with <see cref="AutoValidateAttribute"/>
        /// </summary>
        /// <param name="operation">The swashbuckle operation that is being documented.</param>
        /// <param name="context">Context in which this filter has been called.</param>
        public void Apply(OpenApiOperation operation, OperationFilterContext context)
        {
            if (operation is null || context is null) return;

            if (ContextHasAutoValidateAttributes(context))
            {
                var validationErrorSchema = GetSchemaForValidationResultArrayFromContext(context);
                AddValidationResponseToOperationWithSchema(operation, validationErrorSchema);
            }
        }

        private void AddValidationResponseToOperationWithSchema(OpenApiOperation operation, OpenApiSchema schema)
        {
            if (operation is null || operation.Responses.ContainsKey("422")) return;

            operation.Responses.Add("422", new OpenApiResponse { Description = "Validation of the model failed.", Content = new Dictionary<string, OpenApiMediaType> { ["application/json"] = new OpenApiMediaType { Schema = schema } } });
        }

        private bool ContextHasAutoValidateAttributes(OperationFilterContext context)
            => GetAutoValidateAttributesFromContext(context).Any();

        private IEnumerable<AutoValidateAttribute> GetAutoValidateAttributesFromContext(OperationFilterContext context)
            => context.MethodInfo.ReflectedType.GetCustomAttributes(true)
                .Union(context.MethodInfo.GetCustomAttributes(true))
                .OfType<AutoValidateAttribute>();

        private OpenApiSchema GetSchemaForValidationResultArrayFromContext(OperationFilterContext context)
        {
            var validationResultSchema = context.SchemaGenerator.GenerateSchema(typeof(SerializableValidationResult), context.SchemaRepository);
            var arraySchema = new OpenApiSchema { Type = "array", Items = validationResultSchema };

            return arraySchema;
        }
    }
}
