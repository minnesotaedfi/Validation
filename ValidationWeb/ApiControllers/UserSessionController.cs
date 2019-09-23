using System.Web.Http;

using ValidationWeb.Services.Interfaces;

namespace ValidationWeb.ApiControllers
{

    [RoutePrefix("api/user-session")]
    public class UserSessionController : ApiController
    {
        private readonly IAppUserService _appUserService;

        public UserSessionController(IAppUserService appUserService)
        {
            _appUserService = appUserService;
        }

        [Route("ed-org/select")]
        [HttpPost]
        public void SelectEdOrg([FromBody] string edOrgId)
        {
            _appUserService.UpdateFocusedEdOrg(edOrgId);
        }

        [Route("ed-org/get")]
        public IHttpActionResult GetEdOrgId()
        {
            var session = _appUserService.GetSession();

            if (session != null)
            {
                var result = session.FocusedEdOrgId;
                return Json(new { id = result });
            }

            return Json(new {});
        }

        [Route("school-year/select")]
        [HttpPost]
        public void SelectSchoolYear([FromBody] string schoolYearId)
        {
            _appUserService.UpdateFocusedSchoolYear(int.Parse(schoolYearId));
        }
    }
}
