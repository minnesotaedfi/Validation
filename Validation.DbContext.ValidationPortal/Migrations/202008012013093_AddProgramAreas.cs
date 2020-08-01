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
            
            AddColumn("validation.Announcement", "ProgramAreaId", c => c.Int());
            AddColumn("validation.AppUserSession", "FocusedProgramAreaId", c => c.Int());
            AddColumn("validation.DynamicReportDefinition", "ProgramAreaId", c => c.Int());
            AddColumn("validation.SubmissionCycle", "ProgramAreaId", c => c.Int());
            CreateIndex("validation.Announcement", "ProgramAreaId");
            CreateIndex("validation.DynamicReportDefinition", "ProgramAreaId");
            CreateIndex("validation.SubmissionCycle", "ProgramAreaId");
            AddForeignKey("validation.Announcement", "ProgramAreaId", "validation.ProgramArea", "Id");
            AddForeignKey("validation.DynamicReportDefinition", "ProgramAreaId", "validation.ProgramArea", "Id");
            AddForeignKey("validation.SubmissionCycle", "ProgramAreaId", "validation.ProgramArea", "Id");
        }
        
        public override void Down()
        {
            DropForeignKey("validation.SubmissionCycle", "ProgramAreaId", "validation.ProgramArea");
            DropForeignKey("validation.DynamicReportDefinition", "ProgramAreaId", "validation.ProgramArea");
            DropForeignKey("validation.Announcement", "ProgramAreaId", "validation.ProgramArea");
            DropIndex("validation.SubmissionCycle", new[] { "ProgramAreaId" });
            DropIndex("validation.DynamicReportDefinition", new[] { "ProgramAreaId" });
            DropIndex("validation.Announcement", new[] { "ProgramAreaId" });
            DropColumn("validation.SubmissionCycle", "ProgramAreaId");
            DropColumn("validation.DynamicReportDefinition", "ProgramAreaId");
            DropColumn("validation.AppUserSession", "FocusedProgramAreaId");
            DropColumn("validation.Announcement", "ProgramAreaId");
            DropTable("validation.ProgramArea");
        }
    }
}
