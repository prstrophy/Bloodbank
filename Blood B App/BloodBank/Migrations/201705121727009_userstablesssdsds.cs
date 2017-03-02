namespace BloodBank.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class userstablesssdsds : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Cities",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Description = c.String(),
                        DistrictId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Districts", t => t.DistrictId, cascadeDelete: true)
                .Index(t => t.DistrictId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Cities", "DistrictId", "dbo.Districts");
            DropIndex("dbo.Cities", new[] { "DistrictId" });
            DropTable("dbo.Cities");
        }
    }
}
