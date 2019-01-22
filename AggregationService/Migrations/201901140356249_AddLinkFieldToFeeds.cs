namespace AggregationService.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddLinkFieldToFeeds : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.RssFeeds", "Link", c => c.String(maxLength: 500));
            AlterColumn("dbo.RssFeeds", "Title", c => c.String(nullable: false, maxLength: 300));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.RssFeeds", "Title", c => c.String(nullable: false, maxLength: 100));
            DropColumn("dbo.RssFeeds", "Link");
        }
    }
}
