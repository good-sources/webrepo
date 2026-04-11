namespace AggregationService.Domain.Services
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using AggregationService.Domain.Models;

    public interface ISourceService : IDisposable
    {
        IDictionary<string, int> GetSupportedTypes();
        Task<Guid> AddAsync(Source source);
    }
}
