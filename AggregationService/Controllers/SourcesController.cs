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
        private ISourceService Service { get; }

        public SourcesController() :
            this(new SourceService())
        { }

        public SourcesController(SourceService service)
        {
            Service = service;
        }

        [Route("~/api/supportedsourcetypes")]
        public IHttpActionResult GetSupportedTypes()
        {
            try
            {
                using (Service)
                {
                    return Json(Service.GetSupportedTypes());
                }
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
                    return BadRequest(ModelState);

                using (Service)
                {
                    return Json(Service.Add(source));
                }
            }
            catch (DbUpdateException ex)
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
