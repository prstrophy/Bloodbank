namespace BloodBank.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class userstablessss : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Districts",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Description = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.RegisterViewModels",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Email = c.String(nullable: false),
                        FirstName = c.String(nullable: false),
                        LastName = c.String(nullable: false),
                        Password = c.String(nullable: false, maxLength: 100),
                        ConfirmPassword = c.String(),
                        Address = c.String(),
                        Age = c.Int(nullable: false),
                        Number = c.Int(nullable: false),
                        Gender = c.String(),
                        DistrictId = c.Int(nullable: false),
                        CityId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            AddColumn("dbo.Users", "BloodGroupId", c => c.Int(nullable: false));
            AlterColumn("dbo.BloodGroup", "Description", c => c.String(maxLength: 50));
            DropColumn("dbo.Users", "BloogGroupId");
            DropTable("dbo.AspNetUsers");
        }
        
        public override void Down()
        {
            CreateTable(
                "dbo.AspNetUsers",
                c => new
                    {
                        Id = c.String(nullable: false, maxLength: 128),
                        Email = c.String(maxLength: 256),
                        EmailConfirmed = c.Boolean(nullable: false),
                        PasswordHash = c.String(),
                        SecurityStamp = c.String(),
                        PhoneNumber = c.String(),
                        PhoneNumberConfirmed = c.Boolean(nullable: false),
                        TwoFactorEnabled = c.Boolean(nullable: false),
                        LockoutEndDateUtc = c.DateTime(),
                        LockoutEnabled = c.Boolean(nullable: false),
                        AccessFailedCount = c.Int(nullable: false),
                        UserName = c.String(nullable: false, maxLength: 256),
                    })
                .PrimaryKey(t => t.Id);
            
            AddColumn("dbo.Users", "BloogGroupId", c => c.Int(nullable: false));
            AlterColumn("dbo.BloodGroup", "Description", c => c.String(nullable: false, maxLength: 50));
            DropColumn("dbo.Users", "BloodGroupId");
            DropTable("dbo.RegisterViewModels");
            DropTable("dbo.Districts");
        }
    }
}
