namespace AggregationService.Tests.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Net.Http;
    using System.Threading.Tasks;
    using System.Web.Http;
    using System.Web.Http.Results;
    using Moq;
    using NUnit.Framework;
    using AggregationService.Controllers;
    using AggregationService.Domain.Models;
    using AggregationService.Domain.Services;

    [TestFixture]
    public class CollectionsControllerTests
    {
        private Mock<ICollectionService> _mockService;
        private CollectionsController _controller;

        [SetUp]
        public void SetUp()
        {
            _mockService = new Mock<ICollectionService>();
            _controller = new CollectionsController(_mockService.Object)
            {
                Configuration = new HttpConfiguration(),
                Request = new HttpRequestMessage()
            };
        }

        [TearDown]
        public void TearDown()
        {
            _controller.Dispose();
        }

        [Test]
        public async Task Get_ReturnsJsonResult_WithCollections()
        {
            var collections = new List<Collection>
            {
                new Collection { Id = Guid.NewGuid(), Name = "Test" }
            };
            _mockService.Setup(s => s.GetAsync()).ReturnsAsync(collections);

            var result = await _controller.Get();

            Assert.That(result, Is.TypeOf<JsonResult<IEnumerable<Collection>>>());
        }

        [Test]
        public async Task Get_ReturnsInternalServerError_WhenServiceThrows()
        {
            _mockService.Setup(s => s.GetAsync()).ThrowsAsync(new Exception("DB error"));

            var result = await _controller.Get();

            Assert.That(result, Is.TypeOf<ExceptionResult>());
        }

        [Test]
        public async Task Post_ReturnsJsonResult_WithGuid_WhenModelIsValid()
        {
            var expectedId = Guid.NewGuid();
            _mockService.Setup(s => s.AddAsync(It.IsAny<Collection>())).ReturnsAsync(expectedId);

            var result = await _controller.Post(new Collection { Name = "New" });

            Assert.That(result, Is.TypeOf<JsonResult<Guid>>());
            var jsonResult = (JsonResult<Guid>)result;
            Assert.That(jsonResult.Content, Is.EqualTo(expectedId));
        }

        [Test]
        public async Task Post_ReturnsBadRequest_WhenModelStateIsInvalid()
        {
            _controller.ModelState.AddModelError("Name", "Required");

            var result = await _controller.Post(new Collection());

            Assert.That(result, Is.TypeOf<InvalidModelStateResult>());
        }

        [Test]
        public async Task Post_ReturnsInternalServerError_WhenServiceThrows()
        {
            _mockService.Setup(s => s.AddAsync(It.IsAny<Collection>()))
                .ThrowsAsync(new Exception("DB error"));

            var result = await _controller.Post(new Collection { Name = "Test" });

            Assert.That(result, Is.TypeOf<ExceptionResult>());
        }
    }
}
