namespace AggregationService.Controllers
{
    using System;
    using System.Threading.Tasks;
    using System.Web.Http;
    using AggregationService.Domain.Services;

    [RoutePrefix("api/contents")]
    public class ContentsController : ApiController
    {
        private readonly IContentService _contentService;

        public ContentsController(IContentService contentService)
        {
            _contentService = contentService;
        }

        [Route("bycollection/{id:guid}")]
        public async Task<IHttpActionResult> GetByCollection(Guid id)
        {
            try
            {
                return Json(await _contentService.GetByCollectionAsync(id));
            }
            catch (Exception ex)
            {
                //log
                return InternalServerError(ex);
            }
        }
    }
}
