namespace AggregationService.Domain.Services
{
    using System;

    public interface ITransaction : IDisposable
    {
        void Commit();
    }
}
