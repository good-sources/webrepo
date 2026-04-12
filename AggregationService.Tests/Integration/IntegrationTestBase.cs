namespace AggregationService.Tests.Integration
{
    using System.Collections.Generic;
    using System.Configuration;
    using System.Net.Http;
    using System.Net.Http.Headers;
    using System.Security.Cryptography;
    using System.Text;
    using System.Threading.Tasks;
    using Microsoft.Owin.Testing;
    using Moq;
    using Newtonsoft.Json;
    using NUnit.Framework;
    using AggregationService.Domain.Services;

    public abstract class IntegrationTestBase
    {
        protected const string TestUsername = "testuser";
        protected const string TestPassword = "testpassword";

        protected TestServer Server;
        protected Mock<ICollectionService> CollectionServiceMock;
        protected Mock<IContentService> ContentServiceMock;
        protected Mock<ISourceService> SourceServiceMock;

        [SetUp]
        public void SetUp()
        {
            ConfigurationManager.AppSettings["auth:username"] = TestUsername;
            ConfigurationManager.AppSettings["auth:passwordHash"] = ComputeSha256(TestPassword);

            CollectionServiceMock = new Mock<ICollectionService>();
            ContentServiceMock = new Mock<IContentService>();
            SourceServiceMock = new Mock<ISourceService>();

            var startup = new TestStartup(
                CollectionServiceMock.Object,
                ContentServiceMock.Object,
                SourceServiceMock.Object);

            Server = TestServer.Create(app => startup.Configuration(app));
        }

        [TearDown]
        public void TearDown()
        {
            Server?.Dispose();
        }

        protected async Task<string> GetAuthTokenAsync()
        {
            var tokenRequest = new FormUrlEncodedContent(new[]
            {
                new KeyValuePair<string, string>("grant_type", "password"),
                new KeyValuePair<string, string>("username", TestUsername),
                new KeyValuePair<string, string>("password", TestPassword)
            });

            var response = await Server.HttpClient.PostAsync("/api/auth/token", tokenRequest);
            response.EnsureSuccessStatusCode();

            var json = await response.Content.ReadAsStringAsync();
            var token = JsonConvert.DeserializeObject<Dictionary<string, string>>(json);
            return token["access_token"];
        }

        protected async Task<HttpClient> CreateAuthenticatedClientAsync()
        {
            var token = await GetAuthTokenAsync();
            var client = Server.HttpClient;
            client.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", token);
            return client;
        }

        private static string ComputeSha256(string input)
        {
            using (var sha = SHA256.Create())
            {
                byte[] bytes = sha.ComputeHash(Encoding.UTF8.GetBytes(input));
                var sb = new StringBuilder();
                foreach (byte b in bytes)
                {
                    sb.Append(b.ToString("x2"));
                }
                return sb.ToString();
            }
        }
    }
}
