namespace AggregationService.Domain.Services
{    
    using System;
    using System.Linq;
    using System.Data.Entity;
    using System.Collections.Generic;
    using AggregationService.Domain.Models;

    public class CollectionService : Service<Collection>, ICollectionService
    {
        public CollectionService(DbContext context) : base(context) { }

        public IEnumerable<Collection> Get() => Read().Include(c => c.Sources).ToList<Collection>();

        public Guid Add(Collection collection)
        {
            return Create(collection).Id;
        }
    }
}