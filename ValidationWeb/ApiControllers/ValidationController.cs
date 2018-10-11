using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.ModelBinding;
using ValidationWeb.Services;

namespace ValidationWeb.ApiControllers
{
    [RoutePrefix("api/validation")]
    public class ValidationController : ApiController
    {
        public readonly IValidationResultsService _validationResultsService;

        public ValidationController(IValidationResultsService validationResultsService)
        {
            _validationResultsService = validationResultsService;
        }

        [Route("error-summaries/{mode}")]
        [HttpPost]
        public FilteredValidationErrors GetValidationErrorSummaries(ValidationErrorFilter filter)
        {
            return _validationResultsService.GetFilteredValidationErrorTableData(filter);
        }

        [Route("error-summaries/autocomplete")]
        [HttpPost]
        public List<string> GetValidationErrorAutocomplete(ValidationErrorFilter filter)
        {
            return _validationResultsService.AutocompleteErrorFilter(filter);
        }
    }
}
