namespace AggregationService.Controllers
{
    using System;
    using System.Web.Http;
    using AggregationService.Domain.Models;
    using System.Data.Entity.Infrastructure;
    using AggregationService.Domain.Services;    

    [RoutePrefix("api/sources")]
    public class SourcesController : ApiController
    {
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
                //log
                return InternalServerError(ex);
            }
        }

        [Route("")]
        public IHttpActionResult Post(Source source)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                return Json(_sourceService.Add(source));
            }
            catch (DbUpdateException)
            {
                //log
                return Conflict();
            }
            catch (Exception ex)
            {
                //log
                return InternalServerError(ex);
            }
        }
    }
}
