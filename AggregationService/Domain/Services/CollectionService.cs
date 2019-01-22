namespace AggregationService.Domain.Services
{    
    using System.Linq;
    using System.Collections.Generic;
    using AggregationService.Domain.Models;

    public class CollectionService : Service<Collection>, ICollectionService
    {
        public CollectionService() :
            base(new AggregationServiceContext())
        { }

        public IEnumerable<Collection> Get() => Read().Include("Sources").ToList<Collection>();

        public int Add(Collection collection)
        {
            try
            {
                return Create(collection).Id;
            }
            catch
            {
                throw;
            }
        }
    }
}