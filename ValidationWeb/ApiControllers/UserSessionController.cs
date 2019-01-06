namespace ValidationWeb.ApiControllers
{
    using System.Web.Http;
    using System.Web.Mvc;

    using ValidationWeb.Services;

    [System.Web.Http.RoutePrefix("api/user-session")]
    public class UserSessionController : ApiController
    {
        private readonly IAppUserService _appUserService;

        public UserSessionController(IAppUserService appUserService)
        {
            _appUserService = appUserService;
        }

        [System.Web.Http.Route("ed-org/select")]
        [System.Web.Http.HttpPost]
        public void SelectEdOrg([FromBody] string edOrgId)
        {
            _appUserService.UpdateFocusedEdOrg(edOrgId);
        }

        [System.Web.Http.Route("ed-org/get")]
        public IHttpActionResult GetEdOrgId()
        {
            var result = _appUserService.GetSession().FocusedEdOrgId;
            return Json(new { id = result });
        }

        [System.Web.Http.Route("school-year/select")]
        [System.Web.Http.HttpPost]
        public void SelectSchoolYear([FromBody] string schoolYearId)
        {
            _appUserService.UpdateFocusedSchoolYear(int.Parse(schoolYearId));
        }
    }
}
