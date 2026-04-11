namespace AggregationService.Domain.Services
{
    using System;
    using System.Linq;
    using System.Data.Entity;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using AggregationService.Domain.Models;

    public class CollectionService : Service<Collection>, ICollectionService
    {
        public CollectionService(DbContext context) : base(context) { }

        public async Task<IEnumerable<Collection>> GetAsync()
        {
            return await Read().Include(c => c.Sources).ToListAsync();
        }

        public async Task<Guid> AddAsync(Collection collection)
        {
            await CreateAsync(collection);
            return collection.Id;
        }
    }
}
