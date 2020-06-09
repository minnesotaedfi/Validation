namespace ValidationWeb.Database
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class UpdateAppUserSession : DbMigration
    {
        public override void Up()
        {
            AddColumn("validation.AppUserSession", "FocusedProgramAreaId", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("validation.AppUserSession", "FocusedProgramAreaId");
        }
    }
}
