namespace AggregationService
{
    using System.Web.Http.ExceptionHandling;
    using NLog;

    public class NLogExceptionLogger : ExceptionLogger
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        public override void Log(ExceptionLoggerContext context)
        {
            Logger.Error(context.Exception, "Unhandled exception on {Method} {Path}",
                context.Request.Method, context.Request.RequestUri);
        }
    }
}
