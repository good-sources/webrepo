namespace AggregationService
{
    using System;
    using System.Collections.Generic;
    using System.Web.Http.Dependencies;
    using AggregationService.Domain.Services;

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
            switch (serviceType)
            {
                case Type t when t == typeof(ICollectionService):
                    return new CollectionService(Context);
                case Type t when t == typeof(IContentService):
                    return new ContentService(Context);
                case Type t when t == typeof(ISourceService):
                    return new SourceService(Context);
                case Type t when t == typeof(Controllers.CollectionsController):
                    return new Controllers.CollectionsController(new CollectionService(Context));
                case Type t when t == typeof(Controllers.ContentsController):
                    return new Controllers.ContentsController(new ContentService(Context));
                case Type t when t == typeof(Controllers.SourcesController):
                    return new Controllers.SourcesController(new SourceService(Context));
                default:
                    return null;
            }
        }

        public IEnumerable<object> GetServices(Type serviceType) => new List<object>();

        public void Dispose()
        {
            if (_context != null)
            {
                _context.Dispose();
            }
        }
    }
}
