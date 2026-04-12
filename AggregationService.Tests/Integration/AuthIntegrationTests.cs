namespace AggregationService.Tests.Integration
{
    using System.Collections.Generic;
    using System.Net;
    using System.Net.Http;
    using System.Threading.Tasks;
    using Moq;
    using NUnit.Framework;

    [TestFixture]
    public class AuthIntegrationTests : IntegrationTestBase
    {
        [Test]
        public async Task Token_ValidCredentials_ReturnsAccessToken()
        {
            var request = new FormUrlEncodedContent(new[]
            {
                new KeyValuePair<string, string>("grant_type", "password"),
                new KeyValuePair<string, string>("username", TestUsername),
                new KeyValuePair<string, string>("password", TestPassword)
            });

            var response = await Server.HttpClient.PostAsync("/api/auth/token", request);

            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
            var content = await response.Content.ReadAsStringAsync();
            Assert.That(content, Does.Contain("access_token"));
        }

        [Test]
        public async Task Token_InvalidCredentials_ReturnsBadRequest()
        {
            var request = new FormUrlEncodedContent(new[]
            {
                new KeyValuePair<string, string>("grant_type", "password"),
                new KeyValuePair<string, string>("username", "wrong"),
                new KeyValuePair<string, string>("password", "wrong")
            });

            var response = await Server.HttpClient.PostAsync("/api/auth/token", request);

            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
        }

        [Test]
        public async Task GetCollections_WithoutToken_ReturnsUnauthorized()
        {
            var response = await Server.HttpClient.GetAsync("/api/collections");

            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.Unauthorized));
        }

        [Test]
        public async Task GetCollections_WithValidToken_ReturnsOk()
        {
            CollectionServiceMock
                .Setup(s => s.GetAsync())
                .ReturnsAsync(new List<global::AggregationService.Domain.Models.Collection>());

            var client = await CreateAuthenticatedClientAsync();

            var response = await client.GetAsync("/api/collections");

            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
        }
    }
}
