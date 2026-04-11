namespace AggregationService
{
    using System.Web.Http;
    using System.Web.Http.ExceptionHandling;
    using AggregationService.Formatting;

    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            // Web API configuration and services
            config.DependencyResolver = new DependencyResolver();
            config.Services.Add(typeof(IExceptionLogger), new NLogExceptionLogger());

            // Web API routes
            config.MapHttpAttributeRoutes();          

            config.Formatters.JsonFormatter.SerializerSettings.Converters.Add(
                new PolymorphicSourceConverter()
            );
        }
    }
}
