namespace AggregationService
{
    using System;
    using System.Collections.Generic;
    using System.Web.Http.Dependencies;
    using AggregationService.Domain.Services;

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

    public class DependencyScope : IDependencyScope
    {
        private AggregationServiceContext _context;

        private AggregationServiceContext Context
        {
            get
            {
                if (_context == null)
                    _context = new AggregationServiceContext();

                return _context;
            }
        }

        public object GetService(Type serviceType)
        {
            if (serviceType == typeof(ICollectionService))
                return new CollectionService(Context);

            if (serviceType == typeof(IContentService))
                return new ContentService(Context);

            if (serviceType == typeof(ISourceService))
                return new SourceService(Context);

            if (serviceType == typeof(Controllers.CollectionsController))
                return new Controllers.CollectionsController(new CollectionService(Context));

            if (serviceType == typeof(Controllers.ContentsController))
                return new Controllers.ContentsController(new ContentService(Context));

            if (serviceType == typeof(Controllers.SourcesController))
                return new Controllers.SourcesController(new SourceService(Context));

            return null;
        }

        public IEnumerable<object> GetServices(Type serviceType) => new List<object>();

        public void Dispose()
        {
            if (_context != null)
                _context.Dispose();
        }
    }
}
