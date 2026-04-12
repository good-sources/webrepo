namespace AggregationService.Domain.Services
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using AggregationService.Domain.Models;

    public interface IReader
    {
        Task<IEnumerable<Content>> PullAsync(Source source);
        Task<IEnumerable<Content>> ValidateAsync(Source source);
    }
}
