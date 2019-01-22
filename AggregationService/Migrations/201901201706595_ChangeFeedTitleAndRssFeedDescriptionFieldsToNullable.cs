namespace AggregationService.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ChangeFeedTitleAndRssFeedDescriptionFieldsToNullable : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.RssFeeds", "Title", c => c.String(maxLength: 300));
            AlterColumn("dbo.RssFeeds", "Description", c => c.String(maxLength: 1000));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.RssFeeds", "Description", c => c.String(nullable: false, maxLength: 1000));
            AlterColumn("dbo.RssFeeds", "Title", c => c.String(nullable: false, maxLength: 300));
        }
    }
}
