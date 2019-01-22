namespace AggregationService.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ChangeLastlyUpdatedSourceFieldName : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.RssFeeds", "LastlyModified", c => c.DateTime());
            DropColumn("dbo.RssFeeds", "LastlyUpdated");
        }
        
        public override void Down()
        {
            AddColumn("dbo.RssFeeds", "LastlyUpdated", c => c.DateTime());
            DropColumn("dbo.RssFeeds", "LastlyModified");
        }
    }
}
