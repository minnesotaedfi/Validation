namespace ValidationWeb.Database
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class SoftDeleteSchoolYears : DbMigration
    {
        public override void Up()
        {
            AddColumn(
                "validation.SchoolYear", 
                "Visible", 
                c => c.Boolean(nullable: false, defaultValue: true));
        }   
        
        public override void Down()
        {
            DropColumn("validation.SchoolYear", "Visible");
        }
    }
}
