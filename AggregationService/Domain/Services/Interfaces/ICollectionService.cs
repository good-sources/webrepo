namespace AggregationService.Domain.Services
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using AggregationService.Domain.Models;

    public interface ICollectionService : IDisposable
    {
        Task<IEnumerable<Collection>> GetAsync();
        Task<Guid> AddAsync(Collection collection);
    }
}
