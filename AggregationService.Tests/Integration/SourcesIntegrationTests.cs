namespace AggregationService.Tests.Integration
{
    using System;
    using System.Collections.Generic;
    using System.Data.Entity.Infrastructure;
    using System.Net;
    using System.Net.Http;
    using System.Text;
    using System.Threading.Tasks;
    using Moq;
    using Newtonsoft.Json;
    using NUnit.Framework;
    using AggregationService.Domain.Models;

    [TestFixture]
    public class SourcesIntegrationTests : IntegrationTestBase
    {
        [Test]
        public async Task GetSupportedTypes_ReturnsTypes()
        {
            var types = new Dictionary<string, int> { { "RSS", 0 } };
            SourceServiceMock.Setup(s => s.GetSupportedTypes()).Returns(types);

            var client = await CreateAuthenticatedClientAsync();
            var response = await client.GetAsync("/api/supportedsourcetypes");

            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
            var json = await response.Content.ReadAsStringAsync();
            Assert.That(json, Does.Contain("RSS"));
        }

        [Test]
        public async Task Post_ValidSource_ReturnsId()
        {
            var newId = Guid.NewGuid();
            SourceServiceMock
                .Setup(s => s.AddAsync(It.IsAny<Source>()))
                .ReturnsAsync(newId);

            var client = await CreateAuthenticatedClientAsync();
            var body = JsonConvert.SerializeObject(new
            {
                Uri = "https://example.com/rss",
                Type = 0,
                CollectionId = Guid.NewGuid()
            });
            var content = new StringContent(body, Encoding.UTF8, "application/json");

            var response = await client.PostAsync("/api/sources", content);

            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
            var json = await response.Content.ReadAsStringAsync();
            Assert.That(json, Does.Contain(newId.ToString()));
        }

        [Test]
        public async Task Post_DuplicateSource_ReturnsConflict()
        {
            SourceServiceMock
                .Setup(s => s.AddAsync(It.IsAny<Source>()))
                .ThrowsAsync(new DbUpdateException("Duplicate"));

            var client = await CreateAuthenticatedClientAsync();
            var body = JsonConvert.SerializeObject(new
            {
                Uri = "https://example.com/rss",
                Type = 0,
                CollectionId = Guid.NewGuid()
            });
            var content = new StringContent(body, Encoding.UTF8, "application/json");

            var response = await client.PostAsync("/api/sources", content);

            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.Conflict));
        }
    }
}
