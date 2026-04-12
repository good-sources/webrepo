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
    public class ContentServiceTests
    {
        private Mock<IReader> _mockReader;
        private Mock<DbContext> _mockContext;
        private TestableContentService _service;
        private Mock<ITransaction> _mockTransaction;

        [SetUp]
        public void SetUp()
        {
            _mockReader = new Mock<IReader>();
            _mockContext = new Mock<DbContext>();
            _mockTransaction = new Mock<ITransaction>();

            _service = new TestableContentService(_mockContext.Object, _mockReader.Object, _mockTransaction.Object);
        }

        [Test]
        public async Task GetByCollectionAsync_CallsValidateAsync_ForEachSource()
        {
            var collectionId = Guid.NewGuid();
            var source1 = CreateSource(collectionId);
            var source2 = CreateSource(collectionId);

            SetupSourceDbSet(new List<Source> { source1, source2 });
            SetupContentDbSet(new List<Content>());

            _mockReader.Setup(r => r.ValidateAsync(It.IsAny<Source>()))
                .ReturnsAsync(new List<Content>());

            await _service.GetByCollectionAsync(collectionId);

            _mockReader.Verify(r => r.ValidateAsync(source1), Times.Once);
            _mockReader.Verify(r => r.ValidateAsync(source2), Times.Once);
        }

        [Test]
        public async Task GetByCollectionAsync_WhenNewContentReturned_SavesChangesMultipleTimes()
        {
            var collectionId = Guid.NewGuid();
            var sourceId = Guid.NewGuid();
            var oldContent = new RssItem { Id = Guid.NewGuid(), SourceId = sourceId };
            var source = CreateSource(collectionId, sourceId, new List<Content> { oldContent });
            var newContent = new RssItem { Title = "New Article" };

            SetupSourceDbSet(new List<Source> { source });
            SetupContentDbSet(new List<Content>());

            _mockReader.Setup(r => r.ValidateAsync(source))
                .ReturnsAsync(new List<Content> { newContent });

            await _service.GetByCollectionAsync(collectionId);

            // Delete old + create new = at least 2 SaveChanges
            _mockContext.Verify(c => c.SaveChangesAsync(), Times.AtLeast(2));
            _mockTransaction.Verify(t => t.Commit(), Times.Once);
        }

        [Test]
        public async Task GetByCollectionAsync_WhenNoNewContent_CommitsTransaction()
        {
            var collectionId = Guid.NewGuid();
            var source = CreateSource(collectionId);

            SetupSourceDbSet(new List<Source> { source });
            SetupContentDbSet(new List<Content>());

            _mockReader.Setup(r => r.ValidateAsync(source))
                .ReturnsAsync(new List<Content>());

            await _service.GetByCollectionAsync(collectionId);

            _mockContext.Verify(c => c.SaveChangesAsync(), Times.AtLeastOnce);
            _mockTransaction.Verify(t => t.Commit(), Times.Once);
        }

        [Test]
        public async Task GetByCollectionAsync_SetsSourceId_OnNewContentItems()
        {
            var collectionId = Guid.NewGuid();
            var sourceId = Guid.NewGuid();
            var source = CreateSource(collectionId, sourceId);
            var newContent = new RssItem { Title = "New" };

            SetupSourceDbSet(new List<Source> { source });
            SetupContentDbSet(new List<Content>());

            _mockReader.Setup(r => r.ValidateAsync(source))
                .ReturnsAsync(new List<Content> { newContent });

            await _service.GetByCollectionAsync(collectionId);

            Assert.That(newContent.SourceId, Is.EqualTo(sourceId));
        }

        private RssFeed CreateSource(Guid collectionId, Guid? sourceId = null, ICollection<Content> contents = null)
        {
            return new RssFeed
            {
                Id = sourceId ?? Guid.NewGuid(),
                Uri = "https://example.com/feed",
                CollectionId = collectionId,
                Contents = contents ?? new List<Content>()
            };
        }

        private void SetupSourceDbSet(List<Source> sources)
        {
            var mockSet = MockDbSetHelper.CreateMockDbSet(sources);
            mockSet.Setup(s => s.Include(It.IsAny<string>())).Returns(mockSet.Object);
            _mockContext.Setup(c => c.Set<Source>()).Returns(mockSet.Object);
        }

        private void SetupContentDbSet(List<Content> contents)
        {
            var mockSet = MockDbSetHelper.CreateMockDbSet(contents);
            _mockContext.Setup(c => c.Set<Content>()).Returns(mockSet.Object);
        }
    }

    internal class TestableContentService : ContentService
    {
        private readonly ITransaction _transaction;

        public TestableContentService(DbContext context, IReader reader, ITransaction transaction)
            : base(context, reader)
        {
            _transaction = transaction;
        }

        protected override ITransaction BeginTransaction() => _transaction;
    }
}
