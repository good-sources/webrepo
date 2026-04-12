namespace AggregationService
{
    using System;
    using System.Collections.Generic;
    using System.Web.Http.Dependencies;
    using AggregationService.Domain.Services;

    public class DependencyResolver : IDependencyResolver
    {
        private readonly Reader _reader = new Reader();

        public IDependencyScope BeginScope()
        {
            return new DependencyScope(_reader);
        }

        public object GetService(Type serviceType) => null;

        public IEnumerable<object> GetServices(Type serviceType) => new List<object>();

        public void Dispose() { }
    }
}
