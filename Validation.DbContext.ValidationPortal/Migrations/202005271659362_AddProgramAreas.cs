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
                        Id = c.Int(nullable: false),
                        CodeValue = c.String(),
                        Description = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
        }
        
        public override void Down()
        {
            DropTable("validation.ProgramArea");
        }
    }
}
