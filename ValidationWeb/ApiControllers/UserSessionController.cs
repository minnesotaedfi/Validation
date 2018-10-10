using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using ValidationWeb.Services;

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

        [Route("school-year/select")]
        [HttpPost]
        public void SelectSchoolYear([FromBody] string schoolYearId)
        {
            _appUserService.UpdateFocusedSchoolYear(int.Parse(schoolYearId));
        }
    }
}
