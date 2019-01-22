namespace AggregationService.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class IncreaseSourceUriFieldLength : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.RssFeeds", "Uri", c => c.String(nullable: false, maxLength: 500));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.RssFeeds", "Uri", c => c.String(nullable: false, maxLength: 50));
        }
    }
}
