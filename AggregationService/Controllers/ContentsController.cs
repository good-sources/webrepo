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

        public ContentsController(ContentService service)
        {
            Service = service;
        }

        [Route("bycollection/{id:min(1)}")]
        public IHttpActionResult GetByCollection(int id)
        {
            try
            {
                using (Service)
                {
                    var config = new HttpConfiguration();
                    config.Formatters.JsonFormatter.SerializerSettings.TypeNameHandling = Newtonsoft.Json.TypeNameHandling.Auto;

                    return Json(Service.GetByCollection(id), config.Formatters.JsonFormatter.SerializerSettings);
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