namespace AggregationService.Tests.Domain.Services
{
    using System;
    using System.Collections.Generic;
    using System.Data.Entity;
    using System.Threading.Tasks;
    using Moq;
    using NUnit.Framework;
    using AggregationService.Domain.Models;
    using AggregationService.Domain.Services;
    using AggregationService.Tests.TestHelpers;

    [TestFixture]
    public class SourceServiceTests
    {
        private Mock<IReader> _mockReader;
        private Mock<DbContext> _mockContext;
        private TestableSourceService _service;
        private Mock<ITransaction> _mockTransaction;

        [SetUp]
        public void SetUp()
        {
            _mockReader = new Mock<IReader>();
            _mockContext = new Mock<DbContext>();
            _mockTransaction = new Mock<ITransaction>();

            _service = new TestableSourceService(_mockContext.Object, _mockReader.Object, _mockTransaction.Object);

            SetupSourceDbSet();
            SetupContentDbSet();
        }

        [Test]
        public async Task AddAsync_CallsPullAsync_OnSource()
        {
            var source = new RssFeed { Uri = "https://example.com/feed", CollectionId = Guid.NewGuid() };
            _mockReader.Setup(r => r.PullAsync(source)).ReturnsAsync(new List<Content>());

            await _service.AddAsync(source);

            _mockReader.Verify(r => r.PullAsync(source), Times.Once);
        }

        [Test]
        public async Task AddAsync_WhenContentReturned_SetsSourceIdOnContent()
        {
            var source = new RssFeed { Uri = "https://example.com/feed", CollectionId = Guid.NewGuid() };
            var content = new RssItem { Title = "Test" };
            _mockReader.Setup(r => r.PullAsync(source)).ReturnsAsync(new List<Content> { content });

            await _service.AddAsync(source);

            Assert.That(content.SourceId, Is.EqualTo(source.Id));
        }

        [Test]
        public async Task AddAsync_ReturnsSourceId()
        {
            var source = new RssFeed { Uri = "https://example.com/feed", CollectionId = Guid.NewGuid() };
            _mockReader.Setup(r => r.PullAsync(source)).ReturnsAsync(new List<Content>());

            var result = await _service.AddAsync(source);

            Assert.That(result, Is.EqualTo(source.Id));
        }

        [Test]
        public async Task AddAsync_CommitsTransaction()
        {
            var source = new RssFeed { Uri = "https://example.com/feed", CollectionId = Guid.NewGuid() };
            _mockReader.Setup(r => r.PullAsync(source)).ReturnsAsync(new List<Content>());

            await _service.AddAsync(source);

            _mockTransaction.Verify(t => t.Commit(), Times.Once);
        }

        [Test]
        public void GetSupportedTypes_ReturnsDictionaryWithRss()
        {
            var result = _service.GetSupportedTypes();

            Assert.That(result, Contains.Key("RSS"));
            Assert.That(result["RSS"], Is.EqualTo(0));
        }

        [Test]
        public void GetSupportedTypes_ReturnsAllEnumValues()
        {
            var result = _service.GetSupportedTypes();
            var enumValues = Enum.GetValues(typeof(SourceType));

            Assert.That(result.Count, Is.EqualTo(enumValues.Length));
        }

        private void SetupSourceDbSet()
        {
            var mockSet = MockDbSetHelper.CreateMockDbSet(new List<Source>());
            _mockContext.Setup(c => c.Set<Source>()).Returns(mockSet.Object);
        }

        private void SetupContentDbSet()
        {
            var mockSet = MockDbSetHelper.CreateMockDbSet(new List<Content>());
            _mockContext.Setup(c => c.Set<Content>()).Returns(mockSet.Object);
        }
    }

    internal class TestableSourceService : SourceService
    {
        private readonly ITransaction _transaction;

        public TestableSourceService(DbContext context, IReader reader, ITransaction transaction)
            : base(context, reader)
        {
            _transaction = transaction;
        }

        protected override ITransaction BeginTransaction() => _transaction;
    }
}
