using System.Text;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace EvoPdf_Next_AspNetDemo.Models
{
    public static class ModelStateHelper
    {
        public static string GetModelErrors(ModelStateDictionary modelState)
        {
            var sb = new StringBuilder();
            bool first = true;

            foreach (var entry in modelState.Values)
            {
                foreach (var error in entry.Errors)
                {
                    var msg = !string.IsNullOrWhiteSpace(error.ErrorMessage)
                        ? error.ErrorMessage
                        : (error.Exception != null ? error.Exception.Message : string.Empty);

                    if (string.IsNullOrWhiteSpace(msg))
                        continue;

                    if (!first)
                        sb.Append("; ");

                    sb.Append(msg);
                    first = false;
                }
            }

            return sb.ToString();
        }
    }
}
