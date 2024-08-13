namespace MultiFactorAuthentication.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addedEnabled : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Authentications", "Enabled", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Authentications", "Enabled");
        }
    }
}
