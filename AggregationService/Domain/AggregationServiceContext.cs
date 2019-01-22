namespace AggregationService
{
    using System.Data.Entity;
    using AggregationService.Domain.Models;
    
    public class AggregationServiceContext : DbContext
    {
        public virtual DbSet<Collection> Collections { get; set; }
        public virtual DbSet<Source> Sources { get; set; }
        public virtual DbSet<Content> Contents { get; set; }

        public AggregationServiceContext() : base("Strorage")
        {
            Configuration.LazyLoadingEnabled = false;
        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<RssFeed>().Map(m =>
            {
                m.MapInheritedProperties();
                m.ToTable("RssFeeds");
            });

            modelBuilder.Entity<RssItem>().Map(m =>
            {
                m.MapInheritedProperties();
                m.ToTable("RssItems");
            });
        }
    }
}