namespace AggregationService.Tests.Integration
{
    using System.Net;
    using System.Net.Http;
    using System.Threading.Tasks;
    using Moq;
    using NUnit.Framework;

    [TestFixture]
    public class CorsIntegrationTests : IntegrationTestBase
    {
        [Test]
        public async Task PreflightRequest_ReturnsAllowOriginHeader()
        {
            var request = new HttpRequestMessage(HttpMethod.Options, "/api/collections");
            request.Headers.Add("Origin", "https://example.com");
            request.Headers.Add("Access-Control-Request-Method", "GET");

            var response = await Server.HttpClient.SendAsync(request);

            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
            Assert.That(
                response.Headers.Contains("Access-Control-Allow-Origin"),
                Is.True,
                "Response should contain Access-Control-Allow-Origin header");
        }

        [Test]
        public async Task ActualRequest_ReturnsAllowOriginHeader()
        {
            CollectionServiceMock
                .Setup(s => s.GetAsync())
                .ReturnsAsync(new System.Collections.Generic.List<global::AggregationService.Domain.Models.Collection>());

            var client = await CreateAuthenticatedClientAsync();
            client.DefaultRequestHeaders.Add("Origin", "https://example.com");

            var response = await client.GetAsync("/api/collections");

            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
            Assert.That(
                response.Headers.Contains("Access-Control-Allow-Origin"),
                Is.True,
                "Response should contain Access-Control-Allow-Origin header");
        }
    }
}
