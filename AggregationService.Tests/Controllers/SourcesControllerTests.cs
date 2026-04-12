namespace AggregationService.Tests.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Data.Entity.Infrastructure;
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
    public class SourcesControllerTests
    {
        private Mock<ISourceService> _mockService;
        private SourcesController _controller;

        [SetUp]
        public void SetUp()
        {
            _mockService = new Mock<ISourceService>();
            _controller = new SourcesController(_mockService.Object)
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
        public void GetSupportedTypes_ReturnsJsonResult_WithDictionary()
        {
            var types = new Dictionary<string, int> { { "RSS", 0 } };
            _mockService.Setup(s => s.GetSupportedTypes()).Returns(types);

            var result = _controller.GetSupportedTypes();

            Assert.That(result, Is.TypeOf<JsonResult<IDictionary<string, int>>>());
        }

        [Test]
        public void GetSupportedTypes_ReturnsInternalServerError_WhenServiceThrows()
        {
            _mockService.Setup(s => s.GetSupportedTypes()).Throws(new Exception("Error"));

            var result = _controller.GetSupportedTypes();

            Assert.That(result, Is.TypeOf<ExceptionResult>());
        }

        [Test]
        public async Task Post_ReturnsJsonResult_WithGuid_WhenModelIsValid()
        {
            var expectedId = Guid.NewGuid();
            _mockService.Setup(s => s.AddAsync(It.IsAny<Source>())).ReturnsAsync(expectedId);

            var source = new RssFeed { Uri = "https://example.com/feed", CollectionId = Guid.NewGuid() };
            var result = await _controller.Post(source);

            Assert.That(result, Is.TypeOf<JsonResult<Guid>>());
            var jsonResult = (JsonResult<Guid>)result;
            Assert.That(jsonResult.Content, Is.EqualTo(expectedId));
        }

        [Test]
        public async Task Post_ReturnsBadRequest_WhenModelStateIsInvalid()
        {
            _controller.ModelState.AddModelError("Uri", "Required");

            var result = await _controller.Post(new RssFeed());

            Assert.That(result, Is.TypeOf<InvalidModelStateResult>());
        }

        [Test]
        public async Task Post_ReturnsConflict_WhenDbUpdateExceptionThrown()
        {
            _mockService.Setup(s => s.AddAsync(It.IsAny<Source>()))
                .ThrowsAsync(new DbUpdateException("Duplicate"));

            var source = new RssFeed { Uri = "https://example.com/feed", CollectionId = Guid.NewGuid() };
            var result = await _controller.Post(source);

            Assert.That(result, Is.TypeOf<ConflictResult>());
        }

        [Test]
        public async Task Post_ReturnsInternalServerError_WhenGeneralExceptionThrown()
        {
            _mockService.Setup(s => s.AddAsync(It.IsAny<Source>()))
                .ThrowsAsync(new Exception("Unexpected error"));

            var source = new RssFeed { Uri = "https://example.com/feed", CollectionId = Guid.NewGuid() };
            var result = await _controller.Post(source);

            Assert.That(result, Is.TypeOf<ExceptionResult>());
        }
    }
}
