namespace ValidationWeb.Services.Interfaces
{
    using System.Collections.Generic;

    using ValidationWeb.Models;

    public interface IRecordsRequestService
    {
        RecordsRequest GetRecordsRequestData(
            int schoolYearId, 
            int edOrgId, 
            string studentId);

        IEnumerable<RecordsRequest> GetAllRecordsRequests();

        void SaveRecordsRequest(
            int schoolYearId, 
            RecordsRequestFormData recordsRequest);

        void SaveRecordsResponse(
            int schoolYearId, 
            RecordsResponseFormData recordsResponse);

        void UpdateRecordsRequestStatus(RecordsRequest recordsRequest);
    }
}
