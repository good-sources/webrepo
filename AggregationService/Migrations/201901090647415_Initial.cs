namespace AggregationService.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Initial : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Collections",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(nullable: false, maxLength: 30),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.RssFeeds",
                c => new
                    {
                        Id = c.Guid(nullable: false, identity: true),
                        Uri = c.String(nullable: false, maxLength: 50),
                        LastlyUpdated = c.DateTime(),
                        Expires = c.DateTime(),
                        CollectionId = c.Int(nullable: false),
                        Title = c.String(nullable: false, maxLength: 100),
                        Description = c.String(nullable: false, maxLength: 1000),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Collections", t => t.CollectionId, cascadeDelete: true)
                .Index(t => t.CollectionId);
            
            CreateTable(
                "dbo.RssItems",
                c => new
                    {
                        Id = c.Guid(nullable: false, identity: true),
                        SourceId = c.Guid(nullable: false),
                        Title = c.String(maxLength: 200),
                        Link = c.String(maxLength: 500),
                        Author = c.String(maxLength: 70),
                        Published = c.DateTime(),
                        Description = c.String(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.RssFeeds", t => t.SourceId)
                .Index(t => t.SourceId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.RssItems", "SourceId", "dbo.RssFeeds");
            DropForeignKey("dbo.RssFeeds", "CollectionId", "dbo.Collections");
            DropIndex("dbo.RssItems", new[] { "SourceId" });
            DropIndex("dbo.RssFeeds", new[] { "CollectionId" });
            DropTable("dbo.RssItems");
            DropTable("dbo.RssFeeds");
            DropTable("dbo.Collections");
        }
    }
}
