namespace AggregationService.Controllers
{
    using System;
    using System.Threading.Tasks;
    using System.Web.Http;
    using NLog;
    using AggregationService.Domain.Services;

    [Authorize]
    [RoutePrefix("api/contents")]
    public class ContentsController : ApiController
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();
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
                Logger.Error(ex, "Failed to retrieve contents for collection {CollectionId}", id);
                return InternalServerError(ex);
            }
        }
    }
}
