namespace AggregationService.Domain.Services
{
    using System;
    using System.Collections.Generic;
    using AggregationService.Domain.Models;

    public interface ISourceService : IDisposable
    {       
        IDictionary<string, int> GetSupportedTypes();
        Guid Add(Source source);        
    }
}