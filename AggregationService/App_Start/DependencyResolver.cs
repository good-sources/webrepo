namespace AggregationService
{
    using System;
    using System.Collections.Generic;
    using System.Web.Http.Dependencies;

    public class DependencyResolver : IDependencyResolver
    {
        public IDependencyScope BeginScope()
        {
            return new DependencyScope();
        }

        public object GetService(Type serviceType) => null;

        public IEnumerable<object> GetServices(Type serviceType) => new List<object>();

        public void Dispose() { }
    }
}
