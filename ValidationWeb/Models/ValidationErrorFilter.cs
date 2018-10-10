using System;
using ValidationWeb.ApiControllers;
using System.Web.Http.ModelBinding;

namespace ValidationWeb
{
    [Serializable]
    [ModelBinder(typeof(ValidationErrorFilterModelBinder))]
    public class ValidationErrorFilter
    {
        public int reportDetailsId { get; set; }
        public string[] filterColumns { get; set; }
        public string[] filterTexts { get; set; }
        public string[] sortColumns { get; set; }
        public string[] sortDirections { get; set; }
        public int pageStartingOffset { get; set; }
        public int pageSize { get; set; }
    }
}