namespace MultiFactorAuthentication.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class SendSmsCodeCountAndFailedPhoneAttemptCount : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Authentications", "SendSmsCodeCount", c => c.Int(nullable: false));
            AddColumn("dbo.Authentications", "FailedPhoneAttemptCount", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Authentications", "FailedPhoneAttemptCount");
            DropColumn("dbo.Authentications", "SendSmsCodeCount");
        }
    }
}
