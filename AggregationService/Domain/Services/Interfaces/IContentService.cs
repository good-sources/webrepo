namespace AggregationService.Domain.Services
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using AggregationService.Domain.Models;

    public interface IContentService : IDisposable
    {
        Task<IEnumerable<Content>> GetByCollectionAsync(Guid collectionId);
    }
}
