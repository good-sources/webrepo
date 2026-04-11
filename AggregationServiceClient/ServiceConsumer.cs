namespace AggregationServiceClient
{
    using System;
    using System.Linq;
    using Newtonsoft.Json;
    using System.Net.Http;
    using System.Net.Http.Formatting;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using AggregationService.Domain.Models;

    class ServiceConsumer
    {
        readonly HttpClient _client;
        readonly Uri baseUri;

        public ServiceConsumer(string uri)
        {
            _client = new HttpClient();
            _client.DefaultRequestHeaders.ConnectionClose = true;

            this.baseUri = new Uri(uri);
        }

        public async Task AuthenticateAsync(string username, string password)
        {
            var tokenUri = baseUri.AddSegment("auth/token");

            var content = new FormUrlEncodedContent(new[]
            {
                new KeyValuePair<string, string>("grant_type", "password"),
                new KeyValuePair<string, string>("username", username),
                new KeyValuePair<string, string>("password", password)
            });

            using (HttpResponseMessage response = await _client.PostAsync(tokenUri, content))
            {
                response.EnsureSuccessStatusCode();
                string json = await response.Content.ReadAsStringAsync();
                var tokenResponse = JsonConvert.DeserializeObject<TokenResponse>(json);

                _client.DefaultRequestHeaders.Authorization =
                    new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", tokenResponse.AccessToken);
            }
        }

        public async Task<IEnumerable<Collection>> GetCollectionsAsync()
        {
            try
            {
                using (HttpResponseMessage message = await _client.GetAsync(baseUri.AddSegment("collections")))
                {
                    message.EnsureSuccessStatusCode();
                    return await message.Content.ReadAsAsync<IEnumerable<Collection>>();
                }
            }
            catch
            {
                throw;
            }
        }

        public async Task<bool> CreateCollectionAsync(string collectionName)
        {
            Collection collection = new Collection()
            {
                Name = collectionName
            };

            try
            {
                using (HttpResponseMessage message = await _client.PostAsJsonAsync(baseUri.AddSegment("collections"), collection))
                {
                    message.EnsureSuccessStatusCode();
                    return true;
                }
            }
            catch
            {
                throw;
            }
        }

        public async Task<bool> CreateSourceAsync(string input)
        {
            Source source;
            Collection collection;
            IEnumerable<Collection> collections;
            string[] sourceFields = input.Split(',');
            string uri, collectionName;

            try
            {
                if (sourceFields.Length != 3)
                {
                    throw new ArgumentException("One or more arguments missed");
                }

                foreach (string field in sourceFields)
                {
                    if (string.IsNullOrEmpty(field))
                    {
                        throw new ArgumentException("One or more arguments missed");
                    }
                }

                if (!Int32.TryParse(sourceFields[0].Trim(), out int type))
                {
                    throw new ArgumentException("Invalid source type");
                }

                uri = sourceFields[1].Trim();
                collectionName = sourceFields[2].Trim();

                collections = await GetCollectionsAsync();
                collection = collections.Where(x => x.Name == collectionName).FirstOrDefault();

                if (collection == null)
                {
                    throw new ArgumentException("Invalid collection name");
                }

                switch ((SourceType)type)
                {
                    case SourceType.RSS:
                        source = new RssFeed()
                        {
                            Uri = uri,
                            CollectionId = collection.Id
                        };
                        break;
                    default:
                        throw new ArgumentException("Invalid source type");
                }

                using (HttpResponseMessage message = await _client.PostAsJsonAsync(baseUri.AddSegment("sources"), source))
                {
                    message.EnsureSuccessStatusCode();
                    return true;
                }
            }
            catch
            {
                throw;
            }
        }

        public async Task<IDictionary<string, int>> GetSupportedSourceTypesAsync()
        {
            try
            {
                using (HttpResponseMessage message = await _client.GetAsync(baseUri.AddSegment("supportedsourcetypes")))
                {
                    message.EnsureSuccessStatusCode();
                    return await message.Content.ReadAsAsync<IDictionary<string, int>>();
                }
            }
            catch
            {
                throw;
            }
        }

        public async Task<IEnumerable<Content>> GetContentsByCollectionAsync(string collectionName)
        {
            IEnumerable<Collection> collections;
            Collection collection;

            try
            {
                collections = await GetCollectionsAsync();
                collection = collections.Where(x => x.Name == collectionName).FirstOrDefault();

                if (collection == null)
                {
                    throw new ArgumentException("Invalid collection name");
                }

                using (HttpResponseMessage message = await _client.GetAsync(baseUri.AddSegment($"contents/bycollection/{collection.Id}")))
                {
                    message.EnsureSuccessStatusCode();

                    var formatter = new JsonMediaTypeFormatter
                    {
                        SerializerSettings = { TypeNameHandling = TypeNameHandling.Auto }
                    };

                    return await message.Content.ReadAsAsync<IEnumerable<Content>>(new[] { formatter });
                }
            }
            catch
            {
                throw;
            }
        }
    }

    static class UriExtensions
    {
        public static Uri AddSegment(this Uri OriginalUri, string Segment)
        {
            UriBuilder ub = new UriBuilder(OriginalUri);
            ub.Path = ub.Path + ((ub.Path.EndsWith("/")) ? "" : "/") + Segment;

            return ub.Uri;
        }
    }

    class TokenResponse
    {
        [JsonProperty("access_token")]
        public string AccessToken { get; set; }

        [JsonProperty("token_type")]
        public string TokenType { get; set; }

        [JsonProperty("expires_in")]
        public int ExpiresIn { get; set; }
    }
}
