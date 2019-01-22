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

        public CollectionsController() :
            this(new CollectionService())
        { }

        public CollectionsController(CollectionService service)
        {
            Service = service;
        }

        [Route("")]
        public IHttpActionResult Get()
        {
            try
            {
                using (Service)
                {
                    return Json(Service.Get());
                }
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

                using (Service)
                {
                    return Json(Service.Add(collection));
                }
            }
            catch (Exception ex)
            {
                //log
                return InternalServerError(ex);
            }
        }
    }
}