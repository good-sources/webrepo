namespace AggregationService.Migrations
{
    using System;
    using System.Linq;
    using System.Data.Entity;   
    using System.Data.Entity.Migrations;
    using AggregationService.Domain;

    internal sealed class Configuration : DbMigrationsConfiguration<AggregationServiceContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = false;
        }

        protected override void Seed(AggregationServiceContext context)
        {
            //  This method will be called after migrating to the latest version.

            //  You can use the DbSet<T>.AddOrUpdate() helper extension method 
            //  to avoid creating duplicate seed data.

            /*context.SourceTypes.AddOrUpdate(x => x.Id,
                new SourceType() { Id = 1, Name = "RSS" },
                new SourceType() { Id = 2, Name = "Atom" }
                );*/
        }
    }
}