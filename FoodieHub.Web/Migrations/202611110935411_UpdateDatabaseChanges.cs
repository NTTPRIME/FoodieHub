namespace FoodieHub.Web.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class UpdateDatabaseChanges : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Orders", "Phone", c => c.String(nullable: false));
            AddColumn("dbo.Orders", "Email", c => c.String(nullable: false));
            AddColumn("dbo.Users", "Phone", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Users", "Phone");
            DropColumn("dbo.Orders", "Email");
            DropColumn("dbo.Orders", "Phone");
        }
    }
}
