namespace ValidationWeb.Database
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ValidationRules : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "validation.RulesField",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(),
                        Alias = c.String(),
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
                .PrimaryKey(t => t.Id)
                .ForeignKey("validation.SchoolYear", t => t.SchoolYearId, cascadeDelete: true)
                .Index(t => t.SchoolYearId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("validation.RulesField", "RulesViewId", "validation.RulesView");
            DropForeignKey("validation.RulesView", "SchoolYearId", "validation.SchoolYear");
            DropIndex("validation.RulesView", new[] { "SchoolYearId" });
            DropIndex("validation.RulesField", new[] { "RulesViewId" });
            DropTable("validation.RulesView");
            DropTable("validation.RulesField");
        }
    }
}
