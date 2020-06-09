namespace ValidationWeb.Database.Queries
{
    public class SingleEdOrgByIdQuery
    {
        public const string EdOrgQuery =
@"select 
    Id = eo.EducationOrganizationId,
    OrganizationShortName = eo.ShortNameOfInstitution,
    OrganizationName = eo.NameOfInstitution
    from edfi.EducationOrganization eo
    where eo.EducationOrganizationId = @edOrgId;";
        
        public const string IdColumnName = "Id";
        public const string OrganizationShortNameColumnName = "OrganizationShortName";
        public const string OrganizationNameColumnName = "OrganizationName";

        public int Id { get; set; }
        public string ShortOrganizationName { get; set; }
        public string OrganizationName { get; set; }
    }
}
