namespace AggregationService.Tests.Integration
{
    using System;
    using System.Web.Http;
    using Microsoft.Owin;
    using Microsoft.Owin.Cors;
    using Microsoft.Owin.Security.OAuth;
    using Owin;
    using AggregationService.Auth;
    using AggregationService.Domain.Services;
    using AggregationService.Formatting;

    public class TestStartup
    {
        private readonly ICollectionService _collectionService;
        private readonly IContentService _contentService;
        private readonly ISourceService _sourceService;

        public TestStartup(
            ICollectionService collectionService,
            IContentService contentService,
            ISourceService sourceService)
        {
            _collectionService = collectionService;
            _contentService = contentService;
            _sourceService = sourceService;
        }

        public void Configuration(IAppBuilder app)
        {
            app.UseCors(CorsOptions.AllowAll);

            var oAuthOptions = new OAuthAuthorizationServerOptions
            {
                TokenEndpointPath = new PathString("/api/auth/token"),
                Provider = new SimpleAuthorizationServerProvider(),
                AccessTokenExpireTimeSpan = TimeSpan.FromHours(2),
                AllowInsecureHttp = true
            };

            app.UseOAuthAuthorizationServer(oAuthOptions);
            app.UseOAuthBearerAuthentication(new OAuthBearerAuthenticationOptions());

            var config = new HttpConfiguration();
            config.DependencyResolver = new TestDependencyResolver(
                _collectionService, _contentService, _sourceService);
            config.MapHttpAttributeRoutes();
            config.Formatters.JsonFormatter.SerializerSettings.Converters.Add(
                new PolymorphicSourceConverter());

            app.UseWebApi(config);
        }
    }
}
