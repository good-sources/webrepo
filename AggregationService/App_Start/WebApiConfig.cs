namespace AggregationService
{
    using System.Web.Http;
    using AggregationService.Formatting;

    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            // Web API configuration and services

            // Web API routes
            config.MapHttpAttributeRoutes();          

            config.Formatters.JsonFormatter.SerializerSettings.Converters.Add(
                new PolymorphicSourceConverter()
            );
        }
    }
}
