namespace ValidationWeb.Database
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class RulesFieldUpdate : DbMigration
    {
        public override void Up()
        {
            AddColumn("validation.RulesField", "DisplayOrder", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("validation.RulesField", "DisplayOrder");
        }
    }
}
