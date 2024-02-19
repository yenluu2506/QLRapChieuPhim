namespace CinemeBooking.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class update2 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Ve", "TrangThai", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Ve", "TrangThai");
        }
    }
}
