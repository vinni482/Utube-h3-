namespace Utube.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Video : DbMigration
    {
        public override void Up()
        {
            //AddColumn("dbo.Comments", "VideoId", c => c.Int(nullable: false));
            //DropColumn("dbo.Comments", "VidioId");
        }
        
        public override void Down()
        {
            //AddColumn("dbo.Comments", "VidioId", c => c.Int(nullable: false));
            //DropColumn("dbo.Comments", "VideoId");
        }
    }
}
