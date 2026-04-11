namespace AggregationService.Controllers
{
    using System;
    using System.Threading.Tasks;
    using System.Web.Http;
    using NLog;
    using AggregationService.Domain.Models;
    using System.Data.Entity.Infrastructure;
    using AggregationService.Domain.Services;

    [Authorize]
    [RoutePrefix("api/sources")]
    public class SourcesController : ApiController
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();
        private readonly ISourceService _sourceService;

        public SourcesController(ISourceService sourceService)
        {
            _sourceService = sourceService;
        }

        [Route("~/api/supportedsourcetypes")]
        public IHttpActionResult GetSupportedTypes()
        {
            try
            {
                return Json(_sourceService.GetSupportedTypes());
            }
            catch (Exception ex)
            {
                Logger.Error(ex, "Failed to retrieve supported source types");
                return InternalServerError(ex);
            }
        }

        [Route("")]
        public async Task<IHttpActionResult> Post(Source source)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                return Json(await _sourceService.AddAsync(source));
            }
            catch (DbUpdateException ex)
            {
                Logger.Warn(ex, "Duplicate source conflict on POST");
                return Conflict();
            }
            catch (Exception ex)
            {
                Logger.Error(ex, "Failed to create source");
                return InternalServerError(ex);
            }
        }
    }
}
