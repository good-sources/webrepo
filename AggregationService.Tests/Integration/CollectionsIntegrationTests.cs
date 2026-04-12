namespace AggregationService.Tests.Integration
{
    using System;
    using System.Collections.Generic;
    using System.Net;
    using System.Net.Http;
    using System.Text;
    using System.Threading.Tasks;
    using Moq;
    using Newtonsoft.Json;
    using NUnit.Framework;
    using AggregationService.Domain.Models;

    [TestFixture]
    public class CollectionsIntegrationTests : IntegrationTestBase
    {
        [Test]
        public async Task Get_ReturnsCollectionsAsJson()
        {
            var collections = new List<Collection>
            {
                new Collection { Id = Guid.NewGuid(), Name = "Tech News" },
                new Collection { Id = Guid.NewGuid(), Name = "Science" }
            };
            CollectionServiceMock.Setup(s => s.GetAsync()).ReturnsAsync(collections);

            var client = await CreateAuthenticatedClientAsync();
            var response = await client.GetAsync("/api/collections");

            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
            var json = await response.Content.ReadAsStringAsync();
            var result = JsonConvert.DeserializeObject<List<Collection>>(json);
            Assert.That(result.Count, Is.EqualTo(2));
            Assert.That(result[0].Name, Is.EqualTo("Tech News"));
        }

        [Test]
        public async Task Post_ValidCollection_ReturnsCreatedId()
        {
            var newId = Guid.NewGuid();
            CollectionServiceMock
                .Setup(s => s.AddAsync(It.IsAny<Collection>()))
                .ReturnsAsync(newId);

            var client = await CreateAuthenticatedClientAsync();
            var content = new StringContent(
                JsonConvert.SerializeObject(new { Name = "New Collection" }),
                Encoding.UTF8, "application/json");

            var response = await client.PostAsync("/api/collections", content);

            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
            var json = await response.Content.ReadAsStringAsync();
            Assert.That(json, Does.Contain(newId.ToString()));
        }

        [Test]
        public async Task Post_ServiceThrows_ReturnsInternalServerError()
        {
            CollectionServiceMock
                .Setup(s => s.AddAsync(It.IsAny<Collection>()))
                .ThrowsAsync(new InvalidOperationException("DB error"));

            var client = await CreateAuthenticatedClientAsync();
            var content = new StringContent(
                JsonConvert.SerializeObject(new { Name = "Fail" }),
                Encoding.UTF8, "application/json");

            var response = await client.PostAsync("/api/collections", content);

            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.InternalServerError));
        }
    }
}
