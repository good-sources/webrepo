namespace AggregationService.Controllers
{
    using System;
    using System.Web.Http;
    using AggregationService.Domain.Services;

    [RoutePrefix("api/contents")]
    public class ContentsController : ApiController
    {
        private IContentService Service { get; }

        public ContentsController() :
            this(new ContentService())
        { }

        public ContentsController(IContentService service)
        {
            Service = service;
        }

        [Route("bycollection/{id:guid}")]
        public IHttpActionResult GetByCollection(Guid id)
        {
            try
            {
                using (Service)
                {
                    return Json(Service.GetByCollection(id));
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