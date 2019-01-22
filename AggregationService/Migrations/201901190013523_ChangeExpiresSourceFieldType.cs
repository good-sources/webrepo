namespace AggregationService.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ChangeExpiresSourceFieldType : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.RssFeeds", "Expires", c => c.DateTime());
        }
        
        public override void Down()
        {
            AlterColumn("dbo.RssFeeds", "Expires", c => c.DateTimeOffset(precision: 7));
        }
    }
}
