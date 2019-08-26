namespace ValidationWeb.Database
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class OrgLevelReport : DbMigration
    {
        public override void Up()
        {
            AddColumn("validation.DynamicReportDefinition", "IsOrgLevelReport", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("validation.DynamicReportDefinition", "IsOrgLevelReport");
        }
    }
}
