namespace AggregationService.Controllers
{
    using System;
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
        public IHttpActionResult GetByCollection(Guid id)
        {
            try
            {
                return Json(_contentService.GetByCollection(id));
            }
            catch (Exception ex)
            {
                //log
                return InternalServerError(ex);
            }
        }
    }
}