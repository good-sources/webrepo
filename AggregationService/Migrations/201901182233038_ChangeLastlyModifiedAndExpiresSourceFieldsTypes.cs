namespace AggregationService.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ChangeLastlyModifiedAndExpiresSourceFieldsTypes : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.RssFeeds", "LastlyModified", c => c.DateTimeOffset(precision: 7));
            AlterColumn("dbo.RssFeeds", "Expires", c => c.DateTimeOffset(precision: 7));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.RssFeeds", "Expires", c => c.DateTime());
            AlterColumn("dbo.RssFeeds", "LastlyModified", c => c.DateTime());
        }
    }
}
