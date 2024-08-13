namespace MultiFactorAuthentication.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class FailedPhoneNumberAttemptCount : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Authentications", "FailedPhoneNumberAttemptCount", c => c.Int(nullable: false));
            DropColumn("dbo.Authentications", "FailedPhoneAttemptCount");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Authentications", "FailedPhoneAttemptCount", c => c.Int(nullable: false));
            DropColumn("dbo.Authentications", "FailedPhoneNumberAttemptCount");
        }
    }
}
