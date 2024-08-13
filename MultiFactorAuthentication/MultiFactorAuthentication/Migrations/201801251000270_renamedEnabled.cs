namespace MultiFactorAuthentication.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class renamedEnabled : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Authentications", "Disabled", c => c.Boolean(nullable: false));
            DropColumn("dbo.Authentications", "Enabled");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Authentications", "Enabled", c => c.Boolean(nullable: false));
            DropColumn("dbo.Authentications", "Disabled");
        }
    }
}
