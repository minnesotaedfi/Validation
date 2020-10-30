using System.Web.Mvc;
using DataTables.AspNet.Core;

namespace ValidationWeb.Services.Interfaces
{
    public interface IEdFiApiLogService
    {
        JsonResult GetIdentityIssues(IDataTablesRequest request, string districtId, string year);
        JsonResult GetApiErrors(IDataTablesRequest request, string districtId, string year);
    }
}