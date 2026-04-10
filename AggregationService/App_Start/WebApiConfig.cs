namespace AggregationService
{
    using System.Web.Http;
    using AggregationService.Formatting;

    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            // Web API configuration and services
            config.DependencyResolver = new DependencyResolver();

            // Web API routes
            config.MapHttpAttributeRoutes();          

            config.Formatters.JsonFormatter.SerializerSettings.Converters.Add(
                new PolymorphicSourceConverter()
            );
        }
    }
}
