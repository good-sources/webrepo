namespace AggregationService.Controllers
{
    using System;
    using System.Threading.Tasks;
    using System.Web.Http;
    using AggregationService.Domain.Models;
    using AggregationService.Domain.Services;

    [Authorize]
    [RoutePrefix("api/collections")]
    public class CollectionsController : ApiController
    {
        private readonly ICollectionService _collectionService;

        public CollectionsController(ICollectionService collectionService)
        {
            _collectionService = collectionService;
        }

        [Route("")]
        public async Task<IHttpActionResult> Get()
        {
            try
            {
                return Json(await _collectionService.GetAsync());
            }
            catch (Exception ex)
            {
                //log
                return InternalServerError(ex);
            }
        }

        [Route("")]
        public async Task<IHttpActionResult> Post(Collection collection)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                return Json(await _collectionService.AddAsync(collection));
            }
            catch (Exception ex)
            {
                //log
                return InternalServerError(ex);
            }
        }
    }
}
