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
    public class ContentsControllerTests
    {
        private Mock<IContentService> _mockService;
        private ContentsController _controller;

        [SetUp]
        public void SetUp()
        {
            _mockService = new Mock<IContentService>();
            _controller = new ContentsController(_mockService.Object)
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
        public async Task GetByCollection_ReturnsJsonResult_WithContents()
        {
            var collectionId = Guid.NewGuid();
            var contents = new List<Content>
            {
                new RssItem { Title = "Article 1" }
            };
            _mockService.Setup(s => s.GetByCollectionAsync(collectionId)).ReturnsAsync(contents);

            var result = await _controller.GetByCollection(collectionId);

            Assert.That(result, Is.TypeOf<JsonResult<IEnumerable<Content>>>());
        }

        [Test]
        public async Task GetByCollection_ReturnsInternalServerError_WhenServiceThrows()
        {
            var collectionId = Guid.NewGuid();
            _mockService.Setup(s => s.GetByCollectionAsync(collectionId))
                .ThrowsAsync(new Exception("DB error"));

            var result = await _controller.GetByCollection(collectionId);

            Assert.That(result, Is.TypeOf<ExceptionResult>());
        }

        [Test]
        public async Task GetByCollection_PassesCorrectGuid_ToService()
        {
            var collectionId = Guid.NewGuid();
            _mockService.Setup(s => s.GetByCollectionAsync(collectionId))
                .ReturnsAsync(new List<Content>());

            await _controller.GetByCollection(collectionId);

            _mockService.Verify(s => s.GetByCollectionAsync(collectionId), Times.Once);
        }
    }
}
