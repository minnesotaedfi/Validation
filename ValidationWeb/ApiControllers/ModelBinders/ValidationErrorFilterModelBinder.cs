using System;
using System.Web.Http.Controllers;
using System.Web.Http.ModelBinding;
using Newtonsoft.Json;

namespace ValidationWeb.ApiControllers
{
    /// <summary>
    /// Converts information in an HTTP query string to a standard .NET object representation of the OneRoster query.
    /// </summary>
    /// <param name="actionContext">Provides information about the HTTP request.</param>
    /// <param name="bindingContext">Holds the object representing the validator error filter parameters in its Model property.</param>
    /// <returns></returns>
    public class ValidationErrorFilterModelBinder : IModelBinder
    {
        public bool BindModel(HttpActionContext actionContext, ModelBindingContext bindingContext)
        {
            #region Check preconditions
            if (bindingContext.ModelType != typeof(ValidationErrorFilter))
            {
                return false;
            }
            #endregion Check preconditions

            var jsonString = actionContext.Request.Content.ReadAsStringAsync().Result;
            var filterModel = JsonConvert.DeserializeObject<ValidationErrorFilter>(jsonString);
            bindingContext.Model = filterModel;
            return true;
        }
    }
}