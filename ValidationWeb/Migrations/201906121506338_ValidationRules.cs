namespace ValidationWeb.Database
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ValidationRules : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "validation.DynamicReportDefinition",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(),
                        Description = c.String(),
                        Enabled = c.Boolean(nullable: false),
                        SchoolYearId = c.Int(nullable: false),
                        ValidationRulesViewId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("validation.RulesView", t => t.ValidationRulesViewId, cascadeDelete: true)
                .ForeignKey("validation.SchoolYear", t => t.SchoolYearId, cascadeDelete: true)
                .Index(t => t.SchoolYearId)
                .Index(t => t.ValidationRulesViewId);
            
            CreateTable(
                "validation.DynamicReportField",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Description = c.String(),
                        Enabled = c.Boolean(nullable: false),
                        ValidationRulesFieldId = c.Int(nullable: false),
                        DynamicReportDefinition_Id = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("validation.RulesField", t => t.ValidationRulesFieldId, cascadeDelete: true)
                .ForeignKey("validation.DynamicReportDefinition", t => t.DynamicReportDefinition_Id)
                .Index(t => t.ValidationRulesFieldId)
                .Index(t => t.DynamicReportDefinition_Id);
            
            CreateTable(
                "validation.RulesField",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(),
                        Enabled = c.Boolean(nullable: false),
                        RulesViewId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("validation.RulesView", t => t.RulesViewId, cascadeDelete: true)
                .Index(t => t.RulesViewId);
            
            CreateTable(
                "validation.RulesView",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Schema = c.String(),
                        Name = c.String(),
                        Enabled = c.Boolean(nullable: false),
                        SchoolYearId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
        }
        
        public override void Down()
        {
            DropForeignKey("validation.DynamicReportDefinition", "SchoolYearId", "validation.SchoolYear");
            DropForeignKey("validation.DynamicReportDefinition", "ValidationRulesViewId", "validation.RulesView");
            DropForeignKey("validation.DynamicReportField", "DynamicReportDefinition_Id", "validation.DynamicReportDefinition");
            DropForeignKey("validation.DynamicReportField", "ValidationRulesFieldId", "validation.RulesField");
            DropForeignKey("validation.RulesField", "RulesViewId", "validation.RulesView");
            DropIndex("validation.RulesField", new[] { "RulesViewId" });
            DropIndex("validation.DynamicReportField", new[] { "DynamicReportDefinition_Id" });
            DropIndex("validation.DynamicReportField", new[] { "ValidationRulesFieldId" });
            DropIndex("validation.DynamicReportDefinition", new[] { "ValidationRulesViewId" });
            DropIndex("validation.DynamicReportDefinition", new[] { "SchoolYearId" });
            DropTable("validation.RulesView");
            DropTable("validation.RulesField");
            DropTable("validation.DynamicReportField");
            DropTable("validation.DynamicReportDefinition");
        }
    }
}
