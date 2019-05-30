namespace ValidationWeb.Database.Queries
{
    public class EdOrgQuery
    {
        public const string AllEdOrgQuery =
@"SELECT
    Id = lea.LocalEducationAgencyId,
    OrganizationShortName = eo.ShortNameOfInstitution,
    OrganizationName = eo.NameOfInstitution,
    StateOrganizationId = eo.EducationOrganizationId,
    ParentId = lea.ParentLocalEducationAgencyId,
    StateLevelOrganizationId = lea.StateEducationAgencyId,
    OrgTypeCodeValue = d.CodeValue,
    OrgTypeShortDescription = d.ShortDescription
FROM edfi.LocalEducationAgency lea
LEFT OUTER JOIN edfi.EducationOrganization eo ON eo.EducationOrganizationId = lea.LocalEducationAgencyId
JOIN edfi.LocalEducationAgencyCategoryDescriptor leacd on leacd.LocalEducationAgencyCategoryDescriptorId=lea.LocalEducationAgencyCategoryDescriptorId
JOIN edfi.Descriptor d on d.DescriptorId=leacd.LocalEducationAgencyCategoryDescriptorId";

        public const string SingleEdOrgsQuery =
@"SELECT 
    Id = lea.LocalEducationAgencyId,
    OrganizationShortName = eo.ShortNameOfInstitution,
    OrganizationName = eo.NameOfInstitution,
    StateOrganizationId = eo.EducationOrganizationId,
    ParentId = lea.ParentLocalEducationAgencyId,
    StateLevelOrganizationId = lea.StateEducationAgencyId,
    OrgTypeCodeValue = d.CodeValue,
    OrgTypeShortDescription = d.ShortDescription
FROM edfi.LocalEducationAgency lea
LEFT OUTER JOIN edfi.EducationOrganization eo ON eo.EducationOrganizationId = lea.LocalEducationAgencyId
LEFT OUTER JOIN edfi.LocalEducationAgencyCategoryDescriptor leacd on leacd.LocalEducationAgencyCategoryDescriptorId = lea.LocalEducationAgencyCategoryDescriptorId
JOIN edfi.Descriptor d on d.DescriptorId=leacd.LocalEducationAgencyCategoryDescriptorId
WHERE lea.LocalEducationAgencyId = @lea_id;";

        public const string IdColumnName = "Id";
        public const string OrganizationShortNameColumnName = "OrganizationShortName";
        public const string OrganizationNameColumnName = "OrganizationName";
        public const string StateOrganizationIdColumnName = "StateOrganizationId";
        public const string ParentIdColumnName = "ParentId";
        public const string StateLevelOrganizationIdColumnName = "StateLevelOrganizationId";

        public int Id { get; set; }

        public string ShortOrganizationName { get; set; }
        
        public string OrganizationName { get; set; }
        
        public string StateOrganizationId { get; set; }
        
        public string ParentId { get; set; }
        
        public string StateLevelOrganizationId { get; set; }
        
        public string OrgTypeCodeValue { get; set; }
        
        public string OrgTypeShortDescription { get; set; }
    }
}
