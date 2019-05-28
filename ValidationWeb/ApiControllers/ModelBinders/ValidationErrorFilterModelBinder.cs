using System.Web.Http.Controllers;
using System.Web.Http.ModelBinding;
using Newtonsoft.Json;
using ValidationWeb.Models;

namespace ValidationWeb.ApiControllers.ModelBinders
{
    public class ValidationErrorFilterModelBinder : IModelBinder
    {
        /// <inheritdoc cref="IModelBinder"/>
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