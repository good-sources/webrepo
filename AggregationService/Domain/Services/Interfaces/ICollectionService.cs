namespace AggregationService.Domain.Services
{
    using System;
    using System.Collections.Generic;
    using AggregationService.Domain.Models;

    public interface ICollectionService : IDisposable
    {
        IEnumerable<Collection> Get();
        Guid Add(Collection collection);
    }
}