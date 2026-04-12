namespace AggregationService.Tests.Integration
{
    using System;
    using System.Collections.Generic;
    using System.Web.Http.Dependencies;
    using AggregationService.Controllers;
    using AggregationService.Domain.Services;

    public class TestDependencyResolver : IDependencyResolver
    {
        private readonly ICollectionService _collectionService;
        private readonly IContentService _contentService;
        private readonly ISourceService _sourceService;

        public TestDependencyResolver(
            ICollectionService collectionService,
            IContentService contentService,
            ISourceService sourceService)
        {
            _collectionService = collectionService;
            _contentService = contentService;
            _sourceService = sourceService;
        }

        public IDependencyScope BeginScope()
        {
            return new TestDependencyScope(_collectionService, _contentService, _sourceService);
        }

        public object GetService(Type serviceType) => null;

        public IEnumerable<object> GetServices(Type serviceType) => new List<object>();

        public void Dispose() { }
    }

    public class TestDependencyScope : IDependencyScope
    {
        private readonly ICollectionService _collectionService;
        private readonly IContentService _contentService;
        private readonly ISourceService _sourceService;

        public TestDependencyScope(
            ICollectionService collectionService,
            IContentService contentService,
            ISourceService sourceService)
        {
            _collectionService = collectionService;
            _contentService = contentService;
            _sourceService = sourceService;
        }

        public object GetService(Type serviceType)
        {
            switch (serviceType)
            {
                case Type t when t == typeof(ICollectionService):
                    return _collectionService;
                case Type t when t == typeof(IContentService):
                    return _contentService;
                case Type t when t == typeof(ISourceService):
                    return _sourceService;
                case Type t when t == typeof(CollectionsController):
                    return new CollectionsController(_collectionService);
                case Type t when t == typeof(ContentsController):
                    return new ContentsController(_contentService);
                case Type t when t == typeof(SourcesController):
                    return new SourcesController(_sourceService);
                default:
                    return null;
            }
        }

        public IEnumerable<object> GetServices(Type serviceType) => new List<object>();

        public void Dispose() { }
    }
}
