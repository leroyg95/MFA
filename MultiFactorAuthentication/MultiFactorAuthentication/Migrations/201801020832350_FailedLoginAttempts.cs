namespace MultiFactorAuthentication.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class FailedLoginAttempts : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Authentications", "FailedSmsAttemptCount", c => c.Int(nullable: false));
            AddColumn("dbo.Authentications", "FailedTotpAttemptCount", c => c.Int(nullable: false));
            AddColumn("dbo.Authentications", "FailedHotpAttemptCount", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Authentications", "FailedHotpAttemptCount");
            DropColumn("dbo.Authentications", "FailedTotpAttemptCount");
            DropColumn("dbo.Authentications", "FailedSmsAttemptCount");
        }
    }
}
