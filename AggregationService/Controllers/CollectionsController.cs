namespace AggregationService.Controllers
{
    using System;
    using System.Web.Http;
    using AggregationService.Domain.Models;
    using AggregationService.Domain.Services;

    [RoutePrefix("api/collections")]
    public class CollectionsController : ApiController
    {
        private ICollectionService Service { get; }

        public CollectionsController(ICollectionService service)
        {
            Service = service;
        }

        [Route("")]
        public IHttpActionResult Get()
        {
            try
            {
                return Json(Service.Get());
            }
            catch (Exception ex)
            {
                //log
                return InternalServerError(ex);
            }
        }

        [Route("")]
        public IHttpActionResult Post(Collection collection)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                return Json(Service.Add(collection));
            }
            catch (Exception ex)
            {
                //log
                return InternalServerError(ex);
            }
        }
    }
}