namespace AggregationService.Controllers
{
    using System;
    using System.Web.Http;
    using AggregationService.Domain.Models;
    using AggregationService.Domain.Services;

    [RoutePrefix("api/collections")]
    public class CollectionsController : ApiController
    {
        private readonly ICollectionService _collectionService;

        public CollectionsController(ICollectionService collectionService)
        {
            _collectionService = collectionService;
        }

        [Route("")]
        public IHttpActionResult Get()
        {
            try
            {
                return Json(_collectionService.Get());
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
                {
                    return BadRequest(ModelState);
                }

                return Json(_collectionService.Add(collection));
            }
            catch (Exception ex)
            {
                //log
                return InternalServerError(ex);
            }
        }
    }
}