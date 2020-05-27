namespace ValidationWeb.Database
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class SetupProgramAreaForeignKeys : DbMigration
    {
        public override void Up()
        {
            AddColumn("validation.Announcement", "ProgramAreaId", c => c.Int(nullable: false));
            AddColumn("validation.DynamicReportDefinition", "ProgramAreaId", c => c.Int(nullable: false));
            AddColumn("validation.SubmissionCycle", "ProgramAreaId", c => c.Int(nullable: false));
            CreateIndex("validation.Announcement", "ProgramAreaId");
            CreateIndex("validation.DynamicReportDefinition", "ProgramAreaId");
            CreateIndex("validation.SubmissionCycle", "ProgramAreaId");
            AddForeignKey("validation.Announcement", "ProgramAreaId", "validation.ProgramArea", "Id", cascadeDelete: true);
            AddForeignKey("validation.DynamicReportDefinition", "ProgramAreaId", "validation.ProgramArea", "Id", cascadeDelete: true);
            AddForeignKey("validation.SubmissionCycle", "ProgramAreaId", "validation.ProgramArea", "Id", cascadeDelete: true);
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
            DropColumn("validation.Announcement", "ProgramAreaId");
        }
    }
}
