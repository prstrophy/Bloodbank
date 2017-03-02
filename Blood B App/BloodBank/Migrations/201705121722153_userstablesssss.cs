namespace BloodBank.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class userstablesssss : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.RegisterViewModels", "BloodGroupId", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.RegisterViewModels", "BloodGroupId");
        }
    }
}
