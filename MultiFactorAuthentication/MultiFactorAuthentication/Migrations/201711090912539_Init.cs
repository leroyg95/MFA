namespace MultiFactorAuthentication.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Init : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Authentications",
                c => new
                    {
                        UserId = c.Guid(nullable: false),
                        Username = c.String(),
                        PhoneNumber = c.String(),
                        SmsSecret = c.String(),
                        SmsIssued = c.DateTime(),
                        TotpSecret = c.String(),
                        TimeWindowUsed = c.Long(nullable: false),
                        SmsStatus = c.Int(nullable: false),
                        TotpStatus = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.UserId);
            
            CreateTable(
                "dbo.AuthDevices",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        DeviceToken = c.String(),
                        ExpiryDate = c.DateTime(nullable: false),
                        Authentication_UserId = c.Guid(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Authentications", t => t.Authentication_UserId)
                .Index(t => t.Authentication_UserId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.AuthDevices", "Authentication_UserId", "dbo.Authentications");
            DropIndex("dbo.AuthDevices", new[] { "Authentication_UserId" });
            DropTable("dbo.AuthDevices");
            DropTable("dbo.Authentications");
        }
    }
}
