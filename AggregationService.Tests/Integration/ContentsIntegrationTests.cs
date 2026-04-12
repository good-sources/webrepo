namespace AggregationService.Tests.Integration
{
    using System;
    using System.Collections.Generic;
    using System.Net;
    using System.Threading.Tasks;
    using Moq;
    using Newtonsoft.Json;
    using NUnit.Framework;
    using AggregationService.Domain.Models;

    [TestFixture]
    public class ContentsIntegrationTests : IntegrationTestBase
    {
        [Test]
        public async Task GetByCollection_ValidGuid_ReturnsContents()
        {
            var collectionId = Guid.NewGuid();
            var contents = new List<Content>
            {
                new RssItem
                {
                    Id = Guid.NewGuid(),
                    Title = "Article 1",
                    Link = "https://example.com/1",
                    Published = DateTime.UtcNow,
                    SourceId = Guid.NewGuid(),
                    Description = "Test description"
                }
            };
            ContentServiceMock
                .Setup(s => s.GetByCollectionAsync(collectionId))
                .ReturnsAsync(contents);

            var client = await CreateAuthenticatedClientAsync();
            var response = await client.GetAsync($"/api/contents/bycollection/{collectionId}");

            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
            var json = await response.Content.ReadAsStringAsync();
            Assert.That(json, Does.Contain("Article 1"));
        }

        [Test]
        public async Task GetByCollection_InvalidGuid_ReturnsNotFound()
        {
            var client = await CreateAuthenticatedClientAsync();
            var response = await client.GetAsync("/api/contents/bycollection/not-a-guid");

            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.NotFound));
        }
    }
}
