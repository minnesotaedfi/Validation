namespace ValidationWeb.Database
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ValidationReports : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "rules.RuleValidationDetail",
                c => new
                    {
                        RuleValidationId = c.Long(nullable: false),
                        Id = c.Long(nullable: false),
                        RuleId = c.String(nullable: false, maxLength: 50),
                        IsError = c.Boolean(nullable: false),
                        Message = c.String(),
                    })
                .PrimaryKey(t => new { t.RuleValidationId, t.Id, t.RuleId })
                .ForeignKey("rules.RuleValidation", t => t.RuleValidationId, cascadeDelete: true)
                .Index(t => t.RuleValidationId);
            
            CreateTable(
                "rules.RuleValidation",
                c => new
                    {
                        RuleValidationId = c.Long(nullable: false, identity: true),
                        CollectionId = c.String(maxLength: 50),
                        RunDateTime = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.RuleValidationId);
            
            CreateTable(
                "rules.RuleValidationRuleComponent",
                c => new
                    {
                        RuleValidationId = c.Long(nullable: false),
                        RulesetId = c.String(nullable: false, maxLength: 50),
                        RuleId = c.String(nullable: false, maxLength: 50),
                        Component = c.String(nullable: false, maxLength: 50),
                    })
                .PrimaryKey(t => new { t.RuleValidationId, t.RulesetId, t.RuleId, t.Component })
                .ForeignKey("rules.RuleValidation", t => t.RuleValidationId, cascadeDelete: true)
                .Index(t => t.RuleValidationId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("rules.RuleValidationDetail", "RuleValidationId", "rules.RuleValidation");
            DropForeignKey("rules.RuleValidationRuleComponent", "RuleValidationId", "rules.RuleValidation");
            DropIndex("rules.RuleValidationRuleComponent", new[] { "RuleValidationId" });
            DropIndex("rules.RuleValidationDetail", new[] { "RuleValidationId" });
            DropTable("rules.RuleValidationRuleComponent");
            DropTable("rules.RuleValidation");
            DropTable("rules.RuleValidationDetail");
        }
    }
}
