namespace ValidationWeb
{
    public class SingleEdOrgByIdQuery
    {
        public static string EdOrgQuery =
@"select 
	Id = eo.EducationOrganizationId,
	OrganizationShortName = eo.ShortNameOfInstitution,
	OrganizationName = eo.NameOfInstitution,
	StateOrganizationId = eo.StateOrganizationId
	from edfi.EducationOrganization eo
	where eo.EducationOrganizationId = @edOrgId;";
        
        public const string IdColumnName = "Id";
        public const string OrganizationShortNameColumnName = "OrganizationShortName";
        public const string OrganizationNameColumnName = "OrganizationName";
        public const string StateOrganizationIdColumnName = "StateOrganizationId";

        public int Id { get; set; }

        public string ShortOrganizationName { get; set; }

        public string OrganizationName { get; set; }

        public string StateOrganizationId { get; set; }
    }
}
