namespace ValidationWeb.ApiControllers
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Web.Http;

    using MoreLinq;

    using ValidationWeb.Services;

    [RoutePrefix("api/validation")]
    public class ValidationController : ApiController
    {
        public readonly IValidationResultsService ValidationResultsService;

        private readonly IEdOrgService _edOrgService;

        public ValidationController(IValidationResultsService validationResultsService, IEdOrgService edOrgService)
        {
            ValidationResultsService = validationResultsService;
            _edOrgService = edOrgService;
        }

        [Route("error-summaries/{mode}")]
        [HttpPost]
        public FilteredValidationErrors GetValidationErrorSummaries(ValidationErrorFilter filter)
        {
            return ValidationResultsService.GetFilteredValidationErrorTableData(filter);
        }

        [Route("error-summaries/autocomplete")]
        [HttpPost]
        public List<string> GetValidationErrorAutocomplete(ValidationErrorFilter filter)
        {
            return ValidationResultsService.AutocompleteErrorFilter(filter);
        }

        [Route("edorgs/autocomplete")]
        [HttpGet]
        public IHttpActionResult GetEdOrgs()
        {
            var edOrgs = _edOrgService.GetAuthorizedEdOrgs()
                .DistinctBy(x => x.OrganizationName)
                .OrderBy(x => x.OrganizationName);

            return Json(edOrgs);
        }

    }
}
