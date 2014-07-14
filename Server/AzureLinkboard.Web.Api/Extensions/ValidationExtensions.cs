using System.Collections.Generic;
using System.Web.Http.ModelBinding;
using AzureLinkboard.Domain.Validation;

namespace AzureLinkboard.Web.Extensions
{
    public static class ValidationExtensions
    {
        public static void AddToModelState(this IEnumerable<ValidationError> errors, ModelStateDictionary modelState)
        {
            foreach (ValidationError error in errors)
            {
                modelState.AddModelError(error.Key, error.Message);
            }
        }

        public static void AddToModelState(this ValidationResult result, ModelStateDictionary modelState)
        {
            AddToModelState(result.Errors, modelState);
        }
    }
}