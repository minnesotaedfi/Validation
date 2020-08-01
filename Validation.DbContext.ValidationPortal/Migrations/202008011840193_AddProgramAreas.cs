namespace ValidationWeb.Database
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddProgramAreas : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                    "validation.ProgramArea",
                    c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Description = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            AddColumn("validation.Announcement", "ProgramAreaId", c => c.Int(nullable: true));
            AddColumn("validation.DynamicReportDefinition", "ProgramAreaId", c => c.Int(nullable: true));
            AddColumn("validation.SubmissionCycle", "ProgramAreaId", c => c.Int(nullable: true));
            AddForeignKey("validation.Announcement", "ProgramAreaId", "validation.ProgramArea", "Id", cascadeDelete: true);
            AddForeignKey("validation.DynamicReportDefinition", "ProgramAreaId", "validation.ProgramArea", "Id", cascadeDelete: true);
            AddForeignKey("validation.SubmissionCycle", "ProgramAreaId", "validation.ProgramArea", "Id", cascadeDelete: true);
            AddColumn("validation.AppUserSession", "FocusedProgramAreaId", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropForeignKey("validation.SubmissionCycle", "ProgramAreaId", "validation.ProgramArea");
            DropForeignKey("validation.DynamicReportDefinition", "ProgramAreaId", "validation.ProgramArea");
            DropForeignKey("validation.Announcement", "ProgramAreaId", "validation.ProgramArea");
            DropColumn("validation.Announcement", "ProgramAreaId");
            DropColumn("validation.DynamicReportDefinition", "ProgramAreaId");
            DropColumn("validation.SubmissionCycle", "ProgramAreaId");
            DropTable("validation.ProgramArea");
            DropColumn("validation.AppUserSession", "FocusedProgramAreaId");
        }
    }
}
