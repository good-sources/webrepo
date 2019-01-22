namespace AggregationService.Domain.Services
{
    using System;
    using System.Collections.Generic;
    using AggregationService.Domain.Models;

    public interface IContentService : IDisposable
    {       
        IEnumerable<Content> GetByCollection(int collectionId);
    }
}